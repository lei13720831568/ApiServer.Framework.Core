using Autofac.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core.AutofacExtras.Attributes
{
    /// <summary>
    /// 实例设置接口
    /// </summary>
    public interface IInstanceConfig
    {
        /// <summary>
        /// 设置实例的生命周期
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> SetLifeTime(IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> builder);
    }
}
