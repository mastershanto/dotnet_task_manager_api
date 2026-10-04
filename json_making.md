Viewed Program.cs:1-70

### How to Export JSON for Swagger (OpenAPI) and Postman

When the backend service is running, it automatically generates and serves the complete OpenAPI/Swagger JSON definition containing every route, model, and parameter. 

---

### Step 1: Start the Backend Service

In your project root, start the application service:

```bash
dotnet run --project src/Api/Api.csproj
```
*(The service will typically host on `http://localhost:5000` or `https://localhost:5001` as configured in your launch settings.)*

---

### Step 2: Export the Swagger (OpenAPI) JSON

The OpenAPI specification is served dynamically at the configured endpoint:
- **URL**: `http://localhost:5000/swagger/v1/swagger.json`

#### Option A: Via Terminal (cURL / PowerShell)
You can download the entire JSON specification with a single command:

**PowerShell:**
```powershell
Invoke-WebRequest -Uri "http://localhost:5000/swagger/v1/swagger.json" -OutFile "swagger.json"
```

**cURL:**
```bash
curl http://localhost:5000/swagger/v1/swagger.json -o swagger.json
```

#### Option B: Via Browser
1. Open your browser and navigate to `http://localhost:5000/swagger`.
2. Right below the API title header, click the clickable link: **`/swagger/v1/swagger.json`**.
3. Right-click anywhere on the displayed JSON page and select **Save As...** &rarr; `swagger.json`.

---

### Step 3: Import into Postman as a Full API Collection

Postman natively understands OpenAPI/Swagger JSON files and automatically generates an interactive collection with folders, parameters, and sample bodies:

1. Open **Postman**.
2. Click the **Import** button in the top-left sidebar.
3. Choose either:
   - **File Upload**: Drag and drop the downloaded `swagger.json` file.
   - **Direct URL**: Paste `http://localhost:5000/swagger/v1/swagger.json` directly into the input box and click **Import**.
4. Select **"Postman Collection"** as the import format.
5. Click **Import**.

> [!TIP]
> Once imported, set your Postman environment variable `baseUrl` to `http://localhost:5000` so all endpoints are ready for 1-click execution.

---

### Step 4: Export Postman Collection JSON (For Sharing)

If you need the Postman Collection format (`.postman_collection.json`) to share with other developers or save in your repository:

1. In Postman, locate your newly imported collection in the left sidebar.
2. Click the **three dots (`...`)** next to the collection name.
3. Click **Export**.
4. Choose **Collection v2.1 (recommended)** and click **Export**.
5. Save the file (e.g. `sunset_dance_api.postman_collection.json`).

---

### Configuration Reference

If you want the Swagger UI and JSON endpoint to remain accessible outside the local development environment, verify [program.dart](file:///c:/ajijul_hoque_files/2_structured_projects/sunset_dence_project_files/sunset_dance_manager/src/Api/Program.cs):

#### [program.dart](file:///c:/ajijul_hoque_files/2_structured_projects/sunset_dence_project_files/sunset_dance_manager/src/Api/Program.cs)
```dart
// Swagger JSON endpoint configuration
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Sunset Dance API v1");
});
```

---

### Implementation Explanation (বাংলায় সংক্ষেপ)

১. **Swagger JSON ডাউনলোড**: ব্যাকএন্ড সার্ভিসটি চালু করার পর ব্রাউজার বা টার্মিনাল দিয়ে `http://localhost:5000/swagger/v1/swagger.json` ঠিকানায় হিট করলেই সম্পূর্ণ API-এর OpenAPI JSON পাওয়া যায়।
২. **পোস্টম্যানে অটো-ইমপোর্ট**: পোস্টম্যানের **Import** বাটনে ক্লিক করে উক্ত `swagger.json` ফাইলটি সিলেক্ট করলে অথবা সরাসরি URL পেস্ট করলেই পোস্টম্যান স্বয়ংক্রিয়ভাবে সমস্ত ফোল্ডার, রিকোয়েস্ট মেথড ও প্যারামিটারসহ একটি তৈরি কালেকশন বানিয়ে দেয়।
৩. **পোস্টম্যান কালেকশন এক্সপোর্ট**: তৈরি হওয়া কালেকশনের নামের পাশে থাকা থ্রি-ডট (`...`) থেকে **Export** &rarr; **Collection v2.1** নির্বাচন করে সংরক্ষণ করলেই টিম বা ক্লায়েন্টের সাথে শেয়ারযোগ্য `.postman_collection.json` ফাইল তৈরি হয়ে যাবে।