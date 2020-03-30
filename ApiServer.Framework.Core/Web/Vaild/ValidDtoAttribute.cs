using System;
using ApiServer.Framework.Core.Mvc;
using ApiServer.Framework.Core.Mvc.Vaild;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiServer.Framework.Core.Web.Vaild
{
    /// <summary>
    /// 模型验证特性
    /// </summary>
    public class ValidDtoAttribute: ActionFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ApiServer.Framework.Core.Mvc.ValidModelAttribute"/> class.
        /// </summary>
        public ValidDtoAttribute()
        {
        }
        /// <summary>
        /// Ons the action executing.
        /// </summary>
        /// <param name="context">Context.</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (!context.ModelState.IsValid)
            {
                context.Result = new ActionResult<BadValidationResponse>(new BadValidationResponse(context.ModelState)).Result;
            }
        }
    }
}
