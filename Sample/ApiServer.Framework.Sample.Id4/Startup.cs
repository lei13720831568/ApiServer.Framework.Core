using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using ApiServer.Framework.Core.AppIdentity;
using ApiServer.Framework.Core.AppIdentity.Grant;
using ApiServer.Framework.Core.AppIdentity.Grant.Validator;
using ApiServer.Framework.Core.AppIdentity.Store;
using ApiServer.Framework.Core.AutofacExtras;
using ApiServer.Framework.Core.Extensions;
using ApiServer.Framework.Core.Json;
using ApiServer.Framework.Core.Settings;
using ApiServer.Framework.Core.Web.Middleware;
using ApiServer.Framework.Sample.Entity.Models;
using ApiServer.Framework.Sample.Service;
using Arch.EntityFrameworkCore.UnitOfWork;
using Autofac;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApiServer.Framework.Sample.Id4
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private NLog.ILogger logger = NLog.LogManager.GetCurrentClassLogger();

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //配置参数注入
            services.AddOptions();
            services.Configure<ServerSettings>(Configuration.GetSection("ServerSettings"));
            var settings = Configuration.GetSection("ServerSettings").Get<ServerSettings>();

            //注入数据库上下文
            services.AddDbContextPool<ApplicationDbContext>(options => options.UseMySql(settings.ConnectionStrings.GetValueOrDefault("sample")))
                .AddUnitOfWork<ApplicationDbContext>();
            services.AddApiServerFrameworkWeb("ApiServer.Framework.Sample.Controller");

            //初始化redis
            var csredis = new CSRedis.CSRedisClient(settings.ConnectionStrings["grant_redis"]);
            RedisHelper.Initialization(csredis);

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentity<AppUser, AppRole>()
                .AddClaimsPrincipalFactory<AppUserClaimsPrincipalFactory>();

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                       //.AddTestUsers(InMemoryConfiguration.GetUsers().ToList())
                       //.AddConfigurationStore(options =>
                       //{
                       //    options.ConfigureDbContext = b =>
                       //        b.UseMySql(settings.ConnectionStrings.GetValueOrDefault("id4ConfigDB"),
                       //            sql => sql.MigrationsAssembly(migrationsAssembly));
                       //})
                       .AddInMemoryClients(InMemoryConfiguration.GetClients())
                       .AddInMemoryApiResources(InMemoryConfiguration.GetApiResources())
                       .AddResourceOwnerValidator<AppResourceOwnerPasswordValidator>()
                       .AddProfileService<ProfileService>()
                       .AddExtensionGrantValidator<SMSGrantValidator>()
                       .AddOperationalStore(options =>
                       {
                           options.ConfigureDbContext = builder =>
                                builder.UseMySql(settings.ConnectionStrings.GetValueOrDefault("sample"));

                           // this enables automatic token cleanup. this is optional.
                           options.EnableTokenCleanup = false;
                           options.TokenCleanupInterval = 3600; // interval in seconds (default is 3600)
                       }).AddInMemoryIdentityResources(GetIdentityResources());

            services.AddTransient<IUserStore<AppUser>, UserService>();
            //services.AddSingleton<IPersistedGrantStore, RedisPersistedGrantStore>();
            
            //services.AddTransient<IRoleStore<AppRole>, AppRoleStore>();
            services.AddTransient<IAuthorizationCodeStore, AppAuthorizationCodeStore>();
            services.AddTransient<IReferenceTokenStore, AppReferenceTokenStore>();
            services.AddTransient<IRefreshTokenStore, AppRefreshTokenStore>();
            services.AddTransient<IUserConsentStore, AppUserConsentStore>();

            //services.add

            //services.AddControllers();
        }


        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.AddApiServerFrameWorkDIObj("ApiServer.Framework.Sample.");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            //{
            //    var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            //    //init id4_config database
            //    context.Database.Migrate();
            //}

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseIdentityServer();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
