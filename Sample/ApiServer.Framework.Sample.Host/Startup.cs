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
            //���ò���ע��
            services.AddOptions();
            services.Configure<ServerSettings>(Configuration.GetSection("ServerSettings"));
            services.Configure<JWTSettings>(Configuration.GetSection("JWT"));

            var settings = Configuration.GetSection("ServerSettings").Get<ServerSettings>();
            //ע��http������
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //ע�뵱ǰ�û��ṩ��
            services.AddScoped<ICurrentUserProvider, JWTCurrentUserProvider>();
            //ע���û���Դ��ȡ����
            services.AddScoped<IUserResourceProvider, UserService>();

            //ע��Ȩ����֤��
            services.AddScoped<IAuthorizationHandler, AuthCodeHandler>();

            //����controller activator
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

            //ע�����ݿ�������
            services.AddDbContextPool<ApplicationDbContext>(options => options.UseMySql(settings.ConnectionStrings.GetValueOrDefault("sample")))
                .AddUnitOfWork<ApplicationDbContext>();

            var mvcBuilder = services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddJsonOptions(options =>
                {
                    //��ʽ������ʱ���ʽ
                    options.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter("yyyy-MM-dd HH:mm:ss"));
                    options.JsonSerializerOptions.Converters.Add(new DatetimeNullConverter("yyyy-MM-dd HH:mm:ss"));
                    //���ݸ�ʽ����ĸСд
                    options.JsonSerializerOptions.PropertyNamingPolicy =JsonNamingPolicy.CamelCase;
                    //ȡ��Unicode����
                    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                    //���Կ�ֵ
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                    //����������
                    options.JsonSerializerOptions.AllowTrailingCommas = true;
                    //�����л����������������Ƿ�ʹ�ò����ִ�Сд�ıȽ�
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                }
            );
            #if DEBUG
                        mvcBuilder.AddControllersAsServices();
            #endif

            //����session
            services.AddSession();
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;//����Ҫ��Ϊfalse��Ĭ����true��true��ʱ��session��Ч
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //������
            services.AddCors(options => {
                options.AddPolicy("any", opt =>
                {
                    opt.AllowAnyOrigin() //�����κ���Դ����������
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();//ָ������cookie
                });
            });

            //hsts����
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(5000);
            });

            //���Ҳ����ض�̬���
            List<Assembly> assemblies = AutoModule.FindAssemblys("ApiServer.Framework.Sample.");
            foreach (var c in assemblies)
            {
                mvcBuilder.AddApplicationPart(c);
            }

            //����swagger
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
                        //����xml
                        c.IncludeXmlComments(assembly.Location.Replace(".dll", ".xml"));
                    }
                    catch (Exception ex)
                    {
                        logger.Warn($"���س���xml�ļ�ʧ��:{ex.Message}");
                    }
                }
            });

            services.AddMvc(options => options.EnableEndpointRouting = false);

            //ʹ��autofac�滻����������ע����������
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
