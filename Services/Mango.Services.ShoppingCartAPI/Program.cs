using Mango.Services.ShoppingCart;
using Mango.Services.ShoppingCart.Services;
using Mango.Services.ShoppingCartAPI;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Extensions;
using Mango.Services.ShoppingCartAPI.Middlewares;
using Mango.Services.ShoppingCartAPI.Services;
using Mango.Services.ShoppingCartAPI.Utilities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

Utility.ProductApiBaseUrl = builder.Configuration.GetSection("ServiceUrls:ProductAPI").Value;
Utility.CouponApiBaseUrl = builder.Configuration.GetSection("ServiceUrls:CouponAPI").Value;

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BackendApiAuthenticationHttpClientHandler>();

builder.Services.AddHttpClient("product", client =>
{
    client.BaseAddress = new Uri(Utility.ProductApiBaseUrl);
}).AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();


builder.Services.AddHttpClient("coupon", client =>
{
    client.BaseAddress = new Uri(Utility.CouponApiBaseUrl);
}).AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();


builder.Services.AddEndpointsApiExplorer();
builder.AddSwaggerGenSecuredByToken();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<ErrorHandlingMiddleware>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

builder.AddAuthentication();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

ApplyMigrations();

app.Run();

void ApplyMigrations()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var migrations = dbContext.Database.GetPendingMigrations();

        if (migrations.Any())
        {
            dbContext.Database.Migrate();
        }
    }
}