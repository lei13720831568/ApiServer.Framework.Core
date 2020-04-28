using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using ApiServer.Framework.Core.AutofacExtras;
using ApiServer.Framework.Core.Json;
using ApiServer.Framework.Core.Settings;
using ApiServer.Framework.Core.Web.JWT;
using ApiServer.Framework.Core.Web.Middleware;
using ApiServer.Framework.Core.Web.Permission;
using ApiServer.Framework.Sample.Entity.Models;
using ApiServer.Framework.Sample.Service;
using Arch.EntityFrameworkCore.UnitOfWork;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace ApiServer.Framework.Sample.App

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
        public void ConfigureServices(IServiceCollection services)
        {
            //���ò���ע��
            services.AddOptions();
            services.Configure<ServerSettings>(Configuration.GetSection("ServerSettings"));
            services.Configure<JWTSettings>(Configuration.GetSection("JWT"));
            var settings = Configuration.GetSection("ServerSettings").Get<ServerSettings>();
            var jwtSettings = Configuration.GetSection("JWT").Get<JWTSettings>();

            services.AddSingleton<JwtHelpers>();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.IncludeErrorDetails = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        //��ֵ User.Identity.Name
                        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecurityKey))
                    };

                    options.SecurityTokenValidators.Clear();
                    options.SecurityTokenValidators.Add(new JWTValidator());
                });


           
            //ע��http������
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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
            services.AddSession(options=> {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
            });
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
            List<Assembly> assemblies = AutoModule.FindAssemblys("ApiServer.Framework.Sample.Controller");
            foreach (var c in assemblies)
            {
                mvcBuilder.AddApplicationPart(c);
            }

            //����swagger
            services.AddOpenApiDocument(config => {
                config.Version = "1.0.0";
                config.Title = "api�ĵ�";
                config.Description = "api�ĵ�����";
                config.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}."
                });
                config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            }); //ע��Swagger ����

            services.AddMvc(options => options.EnableEndpointRouting = false);

        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            //���Ҳ����ض�̬���
            List<Assembly> assemblies = AutoModule.FindAssemblys("ApiServer.Framework.Sample.");

            AutoModule qm = new AutoModule(assemblies);
            //�������ע���ϵ
            builder.RegisterModule(qm);
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
            app.UseSession();
            app.UseAuthentication();
            app.UseOpenApi();
            //app.UseReDoc(config=> {
            //    config.Path = "/doc";
            //});

            app.UseSwaggerUi3();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
