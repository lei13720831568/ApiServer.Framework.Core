using ApiServer.Framework.Core.Settings;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ApiServer.Framework.Core.Json;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Reflection;
using ApiServer.Framework.Core.AutofacExtras;
using Autofac;
using ApiServer.Framework.Core.AppIdentity.Store;
using ApiServer.Framework.Core.AppIdentity;
using ApiServer.Framework.Core.Vaild;

namespace ApiServer.Framework.Core.Extensions
{
    public static class StartUpExtensions
    {
        /// <summary>
        /// 添加apiserverframework web 方面的支持
        /// </summary>
        /// <param name="services"></param>
        /// <param name="controllerPrefix">外部controller的assembly前缀</param>
        /// <returns></returns>
        public static IServiceCollection AddApiServerFrameworkWeb(this IServiceCollection services, params string[] controllerPrefix)
        {
            //覆盖controller activator
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());
            var mvcBuilder = services.AddMvc(options => options.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddJsonOptions(options =>
                {
                    //格式化日期时间格式
                    options.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter("yyyy-MM-dd HH:mm:ss"));
                    options.JsonSerializerOptions.Converters.Add(new DatetimeNullConverter("yyyy-MM-dd HH:mm:ss"));
                    //数据格式首字母小写
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
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

            //添加controller


            //添加controller
            mvcBuilder.AddControllersAsServices();

            List<Assembly> assemblies = Find(controllerPrefix.ToList());

            foreach (var c in assemblies)
            {
                mvcBuilder.AddApplicationPart(c);
            }

            mvcBuilder.AddControllersAsServices();
            return services;

        }

        private static List<Assembly> Find(List<string> prefix)
        {
            if (prefix == null || prefix.Count == 0) return new List<Assembly>();
            List<Assembly> assemblies = new List<Assembly>();

            foreach (var ass in prefix)
            {
                //查找并加载动态组件
                assemblies.AddRange(AutoModule.FindAssemblys(ass));
            }
            assemblies = assemblies.Distinct(new AssemblyComparer()).ToList();
            return assemblies;
        }

        /// <summary>
        /// 添加apiserverframework 依赖注入对象的支持
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="modulePrefix"></param>
        public static void AddApiServerFrameWorkDIObj(this ContainerBuilder builder, params string[] modulePrefix) {
            var ls = modulePrefix.ToList();
            List<Assembly> assemblies = Find(ls);

            AutoModule qm = new AutoModule(assemblies);
            //添加依赖注入关系
            builder.RegisterModule(qm);
        }

    }
}
