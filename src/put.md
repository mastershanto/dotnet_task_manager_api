Listed directory BuildingBlocks
Listed directory Web
Listed directory Security
Listed directory Api
Viewed Program.cs:1-95
Listed directory Configuration
Viewed SecurityConfiguration.cs:1-50
Listed directory UpdateProfile
Viewed UpdateProfileCommand.cs:1-13
Viewed UpdateProfileCommandHandler.cs:1-49
Viewed UpdateProfileCommandValidator.cs:1-18
Listed directory Abstractions
Listed directory Behaviors
Viewed ValidationBehavior.cs:1-56

আপনার এই প্রজেক্টের **Clean Architecture + CQRS** আর্কিটেকচারে একটি রিকোয়েস্ট ক্লায়েন্ট থেকে শুরু করে ডাটাবেস ঘুরে কীভাবে আবার ইউজারের কাছে ফেরত যায়, তার সম্পূর্ণ এন্ড-টু-এন্ড (End-to-End) ফ্লো নিচে বিস্তারিত তুলে ধরা হলো।

উদাহরণ হিসেবে আমরা এই সিস্টেমের একটি রিয়েল অথোরাইজড ফিচার—**ইউজার প্রোফাইল আপডেট (`PUT /auth/profile`)**—এর পুরো লাইফসাইকেলটি বেছে নিয়েছি।

---

### 🗺️ পুরো ফ্লো-এর ভিজ্যুয়াল সিকোয়েন্স ডায়াগ্রাম

```mermaid
sequenceDiagram
    autonumber
    actor Client as Client / Frontend
    participant MW as Security & Exception Middlewares
    participant AuthMW as Authentication & Authorization
    participant Endpoint as Minimal API (AuthEndpoints)
    participant Pipe as MediatR Pipeline (ValidationBehavior)
    participant Handler as CQRS Handler (UpdateProfileCommandHandler)
    participant Repo as Repository (EfAuthRepository)
    participant DB as Database (PostgreSQL)

    Client->>MW: HTTP PUT /auth/profile + Bearer Token
    Note over MW: CorrelationId, SecurityHeaders, ExceptionHandling
    MW->>AuthMW: Request Forwarded
    Note over AuthMW: JWT Signature, Expiry ও AuthPolicies.ApiUser যাচাই
    alt টোকেন ইনভ্যালিড বা নেই
        AuthMW-->>Client: 401 Unauthorized
    end
    AuthMW->>Endpoint: ClaimsPrincipal সহ রিকোয়েস্ট পাঠানো
    Endpoint->>Endpoint: Claims থেকে UserId বের করে Command তৈরি
    Endpoint->>Pipe: MediatR sender.Send(command)
    Note over Pipe: FluentValidation (UpdateProfileCommandValidator)
    alt ইনপুট ইনভ্যালিড (যেমন Name ফাঁকা)
        Pipe-->>Endpoint: Result.Failure(ValidationErrors)
        Endpoint-->>Client: 400 Bad Request
    end
    Pipe->>Handler: Handle(command)
    Handler->>Repo: GetUserByIdAsync(userId)
    Repo->>DB: SELECT * FROM Users WHERE Id = @Id
    DB-->>Repo: UserModel
    Handler->>Handler: বিজনেস লজিক: নাম ট্রিম ও UpdatedAt সেট
    Handler->>Repo: UpdateUserAsync(updatedUser)
    Repo->>DB: UPDATE Users SET ... & SaveChangesAsync
    DB-->>Repo: Saved UserModel
    Repo-->>Handler: Return Updated Entity
    Handler-->>Pipe: Result<UserProfileResponse>.Success(profile)
    Pipe-->>Endpoint: Result
    Endpoint-->>Client: 200 OK (JSON Data)
```

---

### 🔍 স্টেপ-বাই-স্টেপ সম্পূর্ণ ফ্লো-এর ব্যাখ্যা

#### ১. ক্লায়েন্ট রিকোয়েস্ট ও সিকিউরিটি মিডলওয়্যার (Middlewares)
ইউজার ক্লায়েন্ট (যেমন Postman বা Frontend) থেকে রিকোয়েস্ট পাঠায়:
```http
PUT /auth/profile
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI...
Content-Type: application/json

{
  "name": "Shanto Dev"
}
```

রিকোয়েস্টটি [.NET HTTP Pipeline](file:///Users/masterShanto/developments/c#_development/dotnet_task_manager_api/src/Api/Program.cs#L61-L66)-এ প্রবেশ করে ক্রমান্বয়ে ৩টি কাস্টম মিডলওয়্যার অতিক্রম করে:
1. [CorrelationIdMiddleware.cs](file:///Users/masterShanto/developments/c#_development/dotnet_task_manager_api/src/BuildingBlocks/Web/CorrelationIdMiddleware.cs): প্রতিটি রিকোয়েস্ট ট্র্যাক করার জন্য একটি ইউনিক `X-Correlation-Id` হেডার যুক্ত করে।
2. [SecurityHeadersMiddleware.cs](file:///Users/masterShanto/developments/c#_development/dotnet_task_manager_api/src/BuildingBlocks/Web/SecurityHeadersMiddleware.cs): নিরাপত্তা নিশ্চিত করতে `X-Frame-Options: DENY`, `X-Content-Type-Options: nosniff`, `X-XSS-Protection` ইত্যাদি সিকিউরিটি হেডার রেসপন্সে বসায়।
3. [ExceptionHandlingMiddleware.cs](file:///Users/masterShanto/developments/c#_development/dotnet_task_manager_api/src/BuildingBlocks/Web/ExceptionHandlingMiddleware.cs): পুরো রিকোয়েস্টকে `try-catch` ব্লকে রাখে। কোডের ভেতরে কোনো আনহ্যান্ডল্ড এক্সেপশন ঘটলে তা ক্লায়েন্টকে নিরাপদ RFC 7807 ProblemDetails বা 500 JSON রেসপন্স ফেরত দেয়।

---

#### ২. প্রমাণীকরণ ও পারমিশন যাচাই (Authentication & Authorization)
- [SecurityConfiguration.cs](file:///Users/masterShanto/developments/c#_development/dotnet_task_manager_api/src/Api/Configuration/SecurityConfiguration.cs)-এ কনফিগার করা `app.UseAuthentication()` রান হয়:
  - এটি রিকোয়েস্টের `Authorization: Bearer <token>` হেডার থেকে JWT টোকেনটি পড়ে।
  - সার্ভারের `SigningKey`, `Issuer`, `Audience` এবং টোকেনের মেয়াদ (`ValidateLifetime`) ভ্যালিডেট করে।
  - টোকেন সঠিক হলে টোকেনের ভেতরের ক্লেইমগুলো (`sub`, `email`, `role`) নিয়ে একটি `ClaimsPrincipal` অবজেক্ট তৈরি করে `HttpContext.User`-এ সেট করে দেয়।
- এরপর `app.UseAuthorization()` রান হয়:
  - এন্ডপয়েন্টে যেহেতু `.RequireAuthorization(AuthPolicies.ApiUser)` পলিসি দেওয়া আছে, এটি চেক করে ইউজার সফলভাবে লগইন করা কিনা।
  - যদি টোকেন না থাকে বা টোকেন মেয়াদোত্তীর্ণ হয়, তবে রিকোয়েস্ট এখানেই থামিয়ে দিয়ে **`401 Unauthorized`** রিটার্ন করে।

---

#### ৩. প্রেজেন্টেশন লেয়ার (Minimal API Endpoint)
রিকোয়েস্টটি পৌঁছায় [AuthEndpoints.cs](file:///Users/masterShanto/developments/c#_development/dotnet_task_manager_api/src/modules/auth/presentation/AuthEndpoints.cs#L137-L150)-এ:
```csharp
protectedGroup.MapPut("/profile", async (
    [FromBody] UpdateProfileRequest request, 
    ClaimsPrincipal userPrincipal, 
    ISender sender, 
    CancellationToken cancellationToken) =>
{
    // ১. টোকেন থেকে বর্তমান ইউজারের আইডি বের করা
    var userId = GetCurrentUserId(userPrincipal);
    if (userId is null) return Results.Unauthorized();

    // ২. Application Layer-এর জন্য CQRS Command তৈরি
    var command = new UpdateProfileCommand(userId.Value, request.Name);

    // ৩. MediatR-এর মাধ্যমে কমান্ড পাঠানো
    var result = await sender.Send(command, cancellationToken);

    // ৪. রেজাল্ট রিটার্ন
    return result.IsSuccess
        ? Results.Ok(result.Value)
        : Results.BadRequest(new { errors = result.Errors });
});
```
> **ক্লিন আর্কিটেকচার নীতি:** এন্ডপয়েন্টে কোনো ডাটাবেস কুয়েরি বা বিজনেস লজিক থাকবে না; এর কাজ কেবল ইনপুট গ্রহণ করা, `UserId` এক্সট্র্যাক্ট করা এবং MediatR-এর কাছে কাজ ডেলিগেট করা।

---

#### ৪. অ্যাপ্লিকেশন মিডলওয়্যার (MediatR Pipeline Behavior)
MediatR হ্যান্ডলারে যাওয়ার আগে [ValidationBehavior.cs](file:///Users/masterShanto/developments/c#_development/dotnet_task_manager_api/src/BuildingBlocks/Abstractions/Behaviors/ValidationBehavior.cs)-এর মধ্য দিয়ে যায়:
- এটি স্বয়ংক্রিয়ভাবে [UpdateProfileCommandValidator.cs](file:///Users/masterShanto/developments/c#_development/dotnet_task_manager_api/src/modules/auth/application/Features/Auth/Commands/UpdateProfile/UpdateProfileCommandValidator.cs) রান করায়:
  ```csharp
  RuleFor(x => x.UserId).NotEmpty();
  RuleFor(x => x.Name).NotEmpty().MinimumLength(2).MaximumLength(100);
  ```
- ইনপুট যদি ইনভ্যালিড হয় (যেমন নাম ফাঁকা), হ্যান্ডলার রানই হবে না! পাইপলাইন সরাসরি `Result<UserProfileResponse>.Failure(...)` বানিয়ে ফিরিয়ে দেবে এবং এন্ডপয়েন্ট ক্লায়েন্টকে **`400 Bad Request`** রেসপন্স পাঠিয়ে দেবে।

---

#### ৫. অ্যাপ্লিকেশন হ্যান্ডলার (CQRS Command Handler)
ভ্যালিডেশন সফল হলে এক্সিকিউট হয় [UpdateProfileCommandHandler.cs](file:///Users/masterShanto/developments/c#_development/dotnet_task_manager_api/src/modules/auth/application/Features/Auth/Commands/UpdateProfile/UpdateProfileCommandHandler.cs):
```csharp
public async Task<Result<UserProfileResponse>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
{
    // ১. ডাটাবেস থেকে ইউজার আনা
    var user = await _authRepository.GetUserByIdAsync(request.UserId, cancellationToken);
    if (user is null)
        return Result<UserProfileResponse>.Failure("User profile not found.");

    // ২. বিজনেস রুলস অনুযায়ী অবজেক্ট আপডেট
    var updatedUser = user with
    {
        Name = request.Name.Trim(),
        UpdatedAt = DateTime.UtcNow
    };

    // ৩. ডাটাবেসে সেভ করা
    var saved = await _authRepository.UpdateUserAsync(updatedUser, cancellationToken);

    // ৪. রেসপন্স DTO ম্যাপ করে Success রিটার্ন করা
    var profile = new UserProfileResponse(
        saved.Id, saved.Name, saved.Email, saved.Role, saved.IsEmailVerified, saved.CreatedAt, saved.UpdatedAt
    );

    return Result<UserProfileResponse>.Success(profile);
}
```

---

#### ৬. ডাটাবেস ও ইনফ্রাস্ট্রাকচার লেয়ার (Data Layer)
`_authRepository` ইন্টারফেসের আসল ইমপ্লিমেন্টেশন হলো [EfAuthRepository.cs](file:///Users/masterShanto/developments/c#_development/dotnet_task_manager_api/src/modules/auth/data/EfAuthRepository.cs):
- এটি Entity Framework Core-এর মাধ্যমে SQL কোয়েরি জেনারেট করে ডাটাবেসে এক্সিকিউট করে এবং `SaveChangesAsync` কল করে ডাটা সেভ করে।

---

#### ৭. ক্লায়েন্টে রেসপন্স ব্যাক করা
1. `EfAuthRepository` ডাটা সেভ করে হ্যান্ডলারকে ডাটা ফেরত দেয়।
2. হ্যান্ডলার `Result<UserProfileResponse>.Success(profile)` রিটার্ন করে।
3. এন্ডপয়েন্টে `result.IsSuccess` সত্য হওয়ায় এটি `Results.Ok(result.Value)` তৈরি করে।
4. ক্লায়েন্ট পায় **HTTP 200 OK**:
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Shanto Dev",
  "email": "user@example.com",
  "role": "user",
  "isEmailVerified": true,
  "createdAt": "2026-03-01T10:00:00Z",
  "updatedAt": "2026-03-08T15:45:00Z"
}
```

---

### 🛠️ নিজের হাতে নতুন কোনো ফিচার বানাতে চাইলে কী কী বানাবেন (Cheat Sheet)

ভবিষ্যতে যেকোনো নতুন ফিচার (যেমন: `Task` তৈরি বা `Category` আপডেট) বানাতে এই ধারাবাহিকতা অনুসরণ করবেন:

| ক্রম | লেয়ার | ফাইল / কম্পোনেন্ট | কী করতে হবে? |
| :--- | :--- | :--- | :--- |
| **১** | **Domain** | `Entity` & `IRepository` | ডোমেন মডেল এবং প্রয়োজনীয় মেথডের ইন্টারফেস লিখবেন (যেমন: `ITaskRepository.cs`)। |
| **২** | **Application** | `Command` বা `Query` | রিকোয়েস্টের প্যারামিটার দিয়ে `record CreateTaskCommand(...) : ICommand<TaskResponse>;` ডিফাইন করবেন। |
| **৩** | **Application** | `Validator` | `AbstractValidator<CreateTaskCommand>` লিখে ইনপুট ভ্যালিডেশন রুলস সেট করবেন। |
| **৪** | **Application** | `Handler` | `ICommandHandler<CreateTaskCommand, TaskResponse>` লিখে বিজনেস লজিক হ্যান্ডেল করবেন। |
| **৫** | **Data** | `Repository Implementation` | EF Core দিয়ে রিপোজিটরি ইন্টারফেস ইমপ্লিমেন্ট করবেন। |
| **৬** | **Presentation** | `Endpoints.cs` | `MapPost`, `MapGet` ইত্যাদি দিয়ে রাউট করবেন এবং প্রয়োজনে `.RequireAuthorization()` পলিসি যুক্ত করবেন। |