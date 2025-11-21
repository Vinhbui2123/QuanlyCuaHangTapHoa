using CommunityToolkit.Maui;
using Microsoft.EntityFrameworkCore;
using QuanlyCuaHangTapHoa.Data;
using QuanlyCuaHangTapHoa.Data.Repositories;


namespace QuanlyCuaHangTapHoa;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ===== Đăng ký DbContext sử dụng SQLite =====
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "grocery_store.db");
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite($"Data Source={dbPath}");
        });
        // Đăng ký repositories
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<ISaleRepository, SaleRepository>();


        var app = builder.Build();

        // Tự động tạo database + bảng nếu chưa có
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        }

        return app;
    }
}
