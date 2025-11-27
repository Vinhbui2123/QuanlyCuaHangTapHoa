using CommunityToolkit.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting; // 👈 thêm
using Microsoft.Maui.Hosting;          // 👈 thêm
using Microsoft.Maui.Storage;          // 👈 thêm
using QuanlyCuaHangTapHoa.Data;
using QuanlyCuaHangTapHoa.Data.Repositories;
using QuanlyCuaHangTapHoa.Services;
using QuanlyCuaHangTapHoa.ViewModels;
using QuanlyCuaHangTapHoa.Views;
using System.IO; // 👈 thêm

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

        // ===== Đăng ký DbContext sử dụng SQLite (Windows + Android đều dùng đường dẫn này) =====
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "grocery_store.db");
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite($"Data Source={dbPath}");
        });

        // ===== Repositories =====
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<ISaleRepository, SaleRepository>();

        // ===== Services =====
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<ISalesService, SalesService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();

        // ===== ViewModels & Pages =====
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<LoginPage>();

        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<RegisterPage>();

        builder.Services.AddTransient<HomePage>(); // nếu sau này có HomeViewModel thì thêm vào

        builder.Services.AddTransient<ProductListViewModel>();
        builder.Services.AddTransient<ProductListPage>();

        builder.Services.AddTransient<ProductDetailViewModel>();
        builder.Services.AddTransient<ProductDetailPage>();

        builder.Services.AddTransient<PosViewModel>();
        builder.Services.AddTransient<PosPage>();

        builder.Services.AddTransient<CategoryListViewModel>();
        builder.Services.AddTransient<CategoryDetailViewModel>();
        builder.Services.AddTransient<CategoryListPage>();
        builder.Services.AddTransient<CategoryDetailPage>();

        builder.Services.AddTransient<SalesHistoryViewModel>();
        builder.Services.AddTransient<SaleDetailViewModel>();
        builder.Services.AddTransient<SalesHistoryPage>();
        builder.Services.AddTransient<SaleDetailPage>();

        builder.Services.AddTransient<HomePageViewModel>();
        builder.Services.AddTransient<HomePage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

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
