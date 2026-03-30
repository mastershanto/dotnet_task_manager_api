using App.Features.Auth.Data;
using App.Features.Auth.Domain;
using App.Features.Auth.Presentation;
using App.Features.Payment.Data;
using App.Features.Payment.Domain;
using App.Features.Payment.Presentation;
using App.Features.Product.Data;
using App.Features.Product.Domain;
using App.Features.Product.Presentation;
using App.Features.User.Data;
using App.Features.User.Domain;
using App.Features.User.Presentation;
using App.Features.Auth.Application;
using App.Features.Payment.Application;
using App.Features.Product.Application;
using App.Features.User.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Feature wiring
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<IAuthAppService, AuthAppService>();

builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<IUserService, UserService>();

builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
builder.Services.AddSingleton<IProductService, ProductService>();

builder.Services.AddSingleton<IPaymentService, PaymentService>();
builder.Services.AddSingleton<IPaymentAppService, PaymentAppService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapAuth();
app.MapUsers();
app.MapProducts();
app.MapPayment();

app.Run();
