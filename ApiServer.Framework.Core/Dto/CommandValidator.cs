using ApiServer.Framework.Core.AutofacExtras.Attributes;
using Autofac;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ApiServer.Framework.Core.Mvc.Vaild;
using ApiServer.Framework.Core.Exceptions;

namespace ApiServer.Framework.Core.Dto
{
    public class CommandValidator:IDependency
    {
        private readonly IContainer container;

        public CommandValidator(IContainer container)
        {
            this.container = container;
        }

        public void Verfiy<T>(T cmd) where T:Command{
            var validator = container.Resolve<AbstractValidator<T>>();
            var results = validator.Validate(cmd);

            if (results.IsValid == false)
            {
                var errors = results.Errors
                    .Select(error => new ValidationError(error.PropertyName, error.ErrorMessage))
                    .ToList();
                throw new BizException("验证参数失败.", "9999", errors);
            }
        }

    }
}
