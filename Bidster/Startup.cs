using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bidster.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Bidster.Entities.Users;
using Microsoft.AspNetCore.Identity.UI.Services;
using Bidster.Services;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Bidster.Configuration;
using Amazon.S3;
using Bidster.Services.FileStorage;
using Bidster.Auth;
using Microsoft.AspNetCore.Authorization;
using Bidster.Hubs;
using Bidster.Services.Notifications;
using Bidster.Services.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Amazon.SimpleEmail;
using Bidster.Services.Tenants;
using Swashbuckle.AspNetCore.Swagger;

namespace Bidster
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<BidsterDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
                    
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<BidsterDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(opts =>
            {
                opts.LoginPath = "/Identity/Account/Login";
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddLogging(builder => builder
                .AddConfiguration(Configuration)
                .AddConsole());

            services.AddKendo();
            services.AddSignalR();
            services.AddMemoryCache();

            // TODO: Can't inject any scoped services into auth handlers, so need to figure out how to get this to work
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.Admin, policy => policy.RequireRole(Roles.Admin));
                //options.AddPolicy("EventAdmin", policy => policy.AddRequirements(new EventAdminRequirement()));
            });
            //services.AddSingleton<IAuthorizationHandler, EventAdminHandler>();

            services.AddSwaggerGen(opts =>
            {
                opts.SwaggerDoc("v1", new Info { Title = "Bidster API", Version = "1" });
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.Configure<EmailConfig>(Configuration.GetSection("Email"));
            services.Configure<UserConfig>(Configuration.GetSection("Users"));
            services.Configure<FileStorageConfig>(Configuration.GetSection("FileStorage"));

            services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<ITenantContext, TenantContext>();
            services.AddScoped<ITenantUserService, TenantUserService>();
            services.AddSingleton<IEmailSender, AmazonSesEmailSender>();
            services.AddTransient<IViewRenderer, ViewRenderer>();
            services.AddScoped<IBidService, BidService>();
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();

            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());

            if (Configuration["FileStorage:storageType"].ToLower() == "amazons3")
            {
                services.AddAWSService<IAmazonS3>();
                services.AddTransient<IFileService, AmazonS3FileService>();
            }

            services.AddAWSService<IAmazonSimpleEmailService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseSignalR(routes =>
            {
                routes.MapHub<BidsterHub>("/bidster");
            });

            app.UseSwagger();
            app.UseSwaggerUI(ui =>
            {
                ui.SwaggerEndpoint("/swagger/v1/swagger.json", "Bidster API");
                ui.RoutePrefix = "api-docs";
            });

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
