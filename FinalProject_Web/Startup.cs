using FinalProject_Web.AuthFinalProjectApp;
using FinalProject_Web.Data;
using Microsoft.AspNetCore.Identity;
using FinalProject_Web.Interfaces;

namespace FinalProject_Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Метод, который служит для добавления необходимых сервисов 
        /// в контейнер.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IApplicationData, ApplicationDataApi>();

            services.AddControllers();
            services.AddControllersWithViews();

            #region //

            services.AddIdentity<User, IdentityRole>()
                .AddDefaultTokenProviders().AddRoles<IdentityRole>();

            #endregion
        }

        /// <summary>
        /// Метод настройки конфигурации
        /// </summary>
        /// <param name="app"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Web}/{action=Index}/{id?}");
            });
        }
    }
}
