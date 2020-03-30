﻿using Autofac.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Framework.Core.AutofacExtras.Attributes
{
    /// <summary>
    /// 单例特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SingletonAttribute : Attribute, IInstanceConfig
    {
        /// <summary>
        /// 构造
        /// </summary>
        public SingletonAttribute()
        {
        }

        /// <summary>
        /// 设置生命周期
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> SetLifeTime(IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> builder)
        {
            return builder.SingleInstance();
        }
    }
}
