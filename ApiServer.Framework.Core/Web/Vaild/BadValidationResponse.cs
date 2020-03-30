using System;
using System.Collections.Generic;
using System.Linq;
using ApiServer.Framework.Core.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace ApiServer.Framework.Core.Mvc.Vaild

{
    /// <summary>
    /// 实体对象验证错误类
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// 失败字段
        /// </summary>
        /// <value>The field.</value>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; }

        /// <summary>
        /// 错误信息
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ApiServer.Framework.Core.Mvc.ValidationError"/> class.
        /// </summary>
        /// <param name="field">Field.</param>
        /// <param name="message">Message.</param>
        public ValidationError(string field, string message)
        {
            Field = field != string.Empty ? field : null;
            Message = message;
        }

    }

    /// <summary>
    /// 验证失败响应类型
    /// </summary>
    public class BadValidationResponse: Response<Object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ApiServer.Framework.Core.Mvc.BadValidationResponse"/> class.
        /// </summary>
        /// <param name="modelState">Model state.</param>
        public BadValidationResponse(ModelStateDictionary modelState)
        {
            Data = null;
            Errors = modelState.Keys
                    .SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                    .ToList();
        }

        /// <summary>
        /// 验证错误列表
        /// </summary>
        /// <value>The errors.</value>
        public List<ValidationError> Errors { get; }

    }
}
