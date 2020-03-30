using Autofac.Core;
using NLog;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using Autofac;
using ApiServer.Framework.Core.AutofacExtras.Attributes;
using Microsoft.AspNetCore.Mvc;
using Autofac.Core.Registration;
using Microsoft.Extensions.DependencyModel;
using System.Runtime.Loader;

namespace ApiServer.Framework.Core.AutofacExtras
{
    /// <summary>
    /// 自动注入
    /// </summary>
    public class AutoModule : Autofac.Module
    {
        /// <summary>
        /// 注入Nlog
        /// </summary>
        /// <param name="instance"></param>
        private static void InjectLoggerProperties(object instance)
        {
            var instanceType = instance.GetType();

            //查找并注入NLog
            var properties = instanceType
             .GetProperties(BindingFlags.Public | BindingFlags.Instance)
             .Where(p => p.PropertyType == typeof(ILogger) && p.CanWrite && p.GetIndexParameters().Length == 0);

            foreach (var propToSet in properties)
            {
                propToSet.SetValue(instance, LogManager.GetLogger(instanceType.FullName), null);
            }
        }

        /// <summary>
        /// 预置NLog的logger name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnComponentPreparing(object sender, PreparingEventArgs e)
        {
            e.Parameters = e.Parameters.Union(
                new[]
                {
                    new ResolvedParameter(
                        (p, i) => p.ParameterType == typeof (ILogger),
                        (p, i) => LogManager.GetLogger(p.Member.DeclaringType.FullName))
                });
        }

        /// <summary>
        /// AttachToComponentRegistration
        /// </summary>
        /// <param name="componentRegistry"></param>
        /// <param name="registration"></param>
        protected override void AttachToComponentRegistration(IComponentRegistryBuilder componentRegistry, IComponentRegistration registration)
        {
            // Handle constructor parameters.
            registration.Preparing += OnComponentPreparing;

            // Handle properties.
            registration.Activated += (sender, e) => InjectLoggerProperties(e.Instance);
        }

        /// <summary>
        /// 属性注入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool RegisterTypeByAttribute<T>(ContainerBuilder builder, Type type) where T : Attribute, IInstanceConfig
        {
            var customAttr = type.GetCustomAttribute<T>();
            if (customAttr != null)
            {
                customAttr.SetLifeTime(builder.RegisterType(type).AsSelf().AsImplementedInterfaces()).PropertiesAutowired();
                return true;
            }

            return false;
        }


        private readonly List<Assembly> assemblies;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="assemblies"></param>
        public AutoModule(List<Assembly> assemblies)
        {
            this.assemblies = assemblies;
        }

        public static List<Assembly> FindAssemblys(params string[] includeAssemblyNamePrefixs) {
            var list = new List<Assembly>();
            var deps = DependencyContext.Default;

            var libs = deps.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type != "package" && !lib.Name.StartsWith("Microsoft") && includeAssemblyNamePrefixs.Where(p => lib.Name.StartsWith(p)).Any());
            foreach (var lib in libs)
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                list.Add(assembly);
            }
            return list;
        }

        /// <summary>
        /// 遍历assembly，注册controller和Idependency对象
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            foreach (var c in this.assemblies)
            {
                foreach (Type t in c.GetTypes())
                {
                    //抽象类和接口不注册
                    if (t.IsAbstract || t.IsInterface)
                        continue;

                    //注册Controller
                    if (t.IsAssignableTo<ControllerBase>())
                    {
                        builder.RegisterType(t).InstancePerDependency().PropertiesAutowired();
                        continue;
                    }

                    //只注册具备依赖接口的接口
                    if (t.IsAssignableTo<IDependency>())
                    {
                        //根据特性注册类型的生命周期
                        if (RegisterTypeByAttribute<SingletonAttribute>(builder, t)
                        || RegisterTypeByAttribute<PerLifetimeScopeAttribute>(builder, t)
                        || RegisterTypeByAttribute<PerMatchingLifetimeScopeAttribute>(builder, t))
                        {
                            continue;
                        }
                        else
                        {
                            builder.RegisterType(t).AsSelf().AsImplementedInterfaces().InstancePerDependency().PropertiesAutowired();
                        }
                    }

                }
            }
        }
    }
}
