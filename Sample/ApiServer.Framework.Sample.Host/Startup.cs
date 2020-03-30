using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using ApiServer.Framework.Core.AutofacExtras;
using ApiServer.Framework.Core.Json;
using ApiServer.Framework.Core.Settings;
using ApiServer.Framework.Core.Web.Middleware;
using ApiServer.Framework.Core.Web.Permission;
using ApiServer.Framework.Sample.Entity.Models;
using ApiServer.Framework.Sample.Service;
using Arch.EntityFrameworkCore.UnitOfWork;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace ApiServer.Framework.Sample.Host
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private NLog.ILogger logger = NLog.LogManager.GetCurrentClassLogger();

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //配置参数注入
            services.AddOptions();
            services.Configure<ServerSettings>(Configuration.GetSection("ServerSettings"));
            services.Configure<JWTSettings>(Configuration.GetSection("JWT"));

            var settings = Configuration.GetSection("ServerSettings").Get<ServerSettings>();
            //注入http上下文
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //注入当前用户提供者
            services.AddScoped<ICurrentUserProvider, JWTCurrentUserProvider>();
            //注入用户资源获取服务
            services.AddScoped<IUserResourceProvider, UserService>();

            //注入权限验证类
            services.AddScoped<IAuthorizationHandler, AuthCodeHandler>();

            //覆盖controller activator
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

            //注入数据库上下文
            services.AddDbContextPool<ApplicationDbContext>(options => options.UseMySql(settings.ConnectionStrings.GetValueOrDefault("sample")))
                .AddUnitOfWork<ApplicationDbContext>();

            var mvcBuilder = services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddJsonOptions(options =>
                {
                    //格式化日期时间格式
                    options.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter("yyyy-MM-dd HH:mm:ss"));
                    options.JsonSerializerOptions.Converters.Add(new DatetimeNullConverter("yyyy-MM-dd HH:mm:ss"));
                    //数据格式首字母小写
                    options.JsonSerializerOptions.PropertyNamingPolicy =JsonNamingPolicy.CamelCase;
                    //取消Unicode编码
                    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                    //忽略空值
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                    //允许额外符号
                    options.JsonSerializerOptions.AllowTrailingCommas = true;
                    //反序列化过程中属性名称是否使用不区分大小写的比较
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                }
            );
            #if DEBUG
                        mvcBuilder.AddControllersAsServices();
            #endif

            //启用session
            services.AddSession();
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;//这里要改为false，默认是true，true的时候session无效
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //跨域处理
            services.AddCors(options => {
                options.AddPolicy("any", opt =>
                {
                    opt.AllowAnyOrigin() //允许任何来源的主机访问
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();//指定处理cookie
                });
            });

            //hsts处理
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(5000);
            });

            //查找并加载动态组件
            List<Assembly> assemblies = AutoModule.FindAssemblys("ApiServer.Framework.Sample.");
            foreach (var c in assemblies)
            {
                mvcBuilder.AddApplicationPart(c);
            }

            //配置swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo{ Title = "API", Version = "v1" });
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };
                foreach (var assembly in assemblies)
                {
                    try
                    {
                        //加载xml
                        c.IncludeXmlComments(assembly.Location.Replace(".dll", ".xml"));
                    }
                    catch (Exception ex)
                    {
                        logger.Warn($"加载程序集xml文件失败:{ex.Message}");
                    }
                }
            });

            services.AddMvc(options => options.EnableEndpointRouting = false);

            //使用autofac替换容器管理，并注册所有类型
            var builder = new ContainerBuilder();
            AutoModule qm = new AutoModule(assemblies);
            builder.RegisterModule(qm);
            builder.Populate(services);
            var applicationContainer = builder.Build();
            return new AutofacServiceProvider(applicationContainer);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseAuthentication();
            app.UseSession();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                c.RoutePrefix = "doc";
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
