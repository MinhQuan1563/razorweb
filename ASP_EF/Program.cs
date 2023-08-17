using ASP_EF.Models;
using ASP_EF.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

/*
    - CREATE, READ, UPDATE, DELETE (CRUD)

    - Identity
        - Authentication: Xác định danh tính -> Login, Logout, ...

        - Authorization: Xác định quyền truy cập
            - Role-based authorization: Xác định quyền theo vai trò
            - Role (vai trò): Admin, Editer, Manage, Member, ...

        /Areas/Admin/Pages/Role
        Index
        Create
        Edit
        Delete

        - Quản lý user: Signup, User, role, ...
 
    /Identity/Account/Login
    /Identity/Account/Manage

    Them tat ca cac file razor can thiet vao identity
        -> dotnet aspnet-codegenerator identity -dc ASP_EF.Models.MyBlogContext

    - Google: https://localhost:7239/dang-nhap-tu-google
        + client ID : 590732606937-gqr5n361be7eu9l24u99e7c3585sjtd4.apps.googleusercontent.com
        + Client Secret: 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
*/

namespace ASP_EF
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var services = builder.Services;

            // Add services to the container.
            services.AddRazorPages();

            // DbContext
            services.AddDbContext<MyBlogContext>(options =>
            {
                string connectString = builder.Configuration.GetConnectionString("MyBlogContext");
                options.UseSqlServer(connectString);
            });

            // Đăng ký Identity
            services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<MyBlogContext>()
                    .AddDefaultTokenProviders();

            /*services.AddDefaultIdentity<AppUser>()
                            .AddEntityFrameworkStores<MyBlogContext>()
                            .AddDefaultTokenProviders();*/

            // Truy cập IdentityOptions
            services.Configure<IdentityOptions>(options => {
                // Thiết lập về Password
                options.Password.RequireDigit = false; // Không bắt phải có số
                options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
                options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
                options.Password.RequireUppercase = false; // Không bắt buộc chữ in
                options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
                options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

                // Cấu hình Lockout - khóa user
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
                options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 3 lần thì khóa
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình về User.
                options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;  // Email là duy nhất

                // Cấu hình đăng nhập.
                options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
                options.SignIn.RequireConfirmedAccount = false;
            });

            // Đăng ký dịch vụ Mail
            services.AddOptions();
            var mailsetting = builder.Configuration.GetSection("MailSettings");
            builder.Services.Configure<MailSettings>(mailsetting);
            builder.Services.AddSingleton<IEmailSender, SendMailService>();

            // 
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/login/";
                options.LogoutPath = "/logout/";
                options.AccessDeniedPath = "/truycapbituchoi.html/";
            });

            // Đăng ký dịch vụ Google
            services.AddAuthentication()
                    .AddGoogle(options =>
                    {
                        var ggconfig = builder.Configuration.GetSection("Authentication:Google");
                        options.ClientId = ggconfig["ClientId"];
                        options.ClientSecret = ggconfig["ClientSecret"];
                        // https://localhost:7239/dang-nhap-tu-google
                        options.CallbackPath = "/dang-nhap-tu-google";
                    })
                    .AddFacebook(options =>
                    {
                        var fbconfig = builder.Configuration.GetSection("Authentication:Facebook");
                        options.AppId = fbconfig["AppId"];
                        options.AppSecret = fbconfig["AppSecret"];
                        // https://localhost:7239/dang-nhap-tu-facebook
                        options.CallbackPath = "/dang-nhap-tu-facebook";
                    });

            // Đăng ký dịch vụ AppIdentityErrorDescriber để nạp chồng IdentityErrorDescriber
            services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}