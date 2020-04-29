using ApiServer.Framework.Core.Settings;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using ApiServer.Framework.Core.Json;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Reflection;
using ApiServer.Framework.Core.AutofacExtras;
using Autofac;
using ApiServer.Framework.Core.Vaild;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace ApiServer.Framework.Core.Extensions
{
    public static class StartUpExtensions
    {

        public static List<Assembly> Find(List<string> prefix)
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
