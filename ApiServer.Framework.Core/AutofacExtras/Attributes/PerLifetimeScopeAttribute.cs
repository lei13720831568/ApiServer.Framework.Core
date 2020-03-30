using Autofac.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core.AutofacExtras.Attributes
{
    /// <summary>
    /// PerLifetimeScope生命周期特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PerLifetimeScopeAttribute : Attribute, IInstanceConfig
    {
        /// <summary>
        /// 构造
        /// </summary>
        public PerLifetimeScopeAttribute()
        {
        }

        /// <summary>
        /// 生命周期设置
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> SetLifeTime(IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> builder)
        {
            return builder.InstancePerLifetimeScope();
        }
    }
}
