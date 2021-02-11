using GSCrm.Data;
using GSCrm.Models;
using GSCrm.Helpers;
using GSCrm.Data.Cash;
using GSCrm.Factories;
using GSCrm.Validators;
using GSCrm.Localization;
using GSCrm.Transactions;
using GSCrm.Routing.Middleware.AccessibilityMiddleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using static GSCrm.CommonConsts;

namespace GSCrm
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            services.AddSingleton<IUserContextFactory, UserContextFactory>();
            services.AddHttpContextAccessor();

            services.AddSingleton<ResManager>();
            services.AddSingleton<IResManager, ResManager>();
            services.AddSingleton<ICachService, CachService>();
            services.AddScoped<IMapFactory, MapFactory>();
            services.AddScoped<ITFFactory, TFFactory>();
            services.AddScoped<IRepositoryFactory, RepositoryFactory>();
            services.AddScoped<ITransaction, Transaction>();
            services.AddScoped<IAccessibilityHandlerFactory, AccessibilityHandlerFactory>();
            services.AddTransient<IPasswordValidator<User>, PasswordValidator>();

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.ConfigureApplicationCookie(options => options.LoginPath = "/Auth/Index");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
            {
                app.UseExceptionHandler("/Shared/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Далее важно соблюдать последовательность
            // Вызывается первым
            app.UseStatusCodePagesRedirect();
            // Вызывается вторым
            app.UseRouting();
            // Вызывается третьим
            app.UseAuthentication();
            app.UseAuthorization();
            // Вызывается четвертым
            app.UseAccessibilityMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: $"{{controller={AUTH}}}/{{action=Index}}/{{id?}}");
            });

            app.Prepare();
        }
    }
}
