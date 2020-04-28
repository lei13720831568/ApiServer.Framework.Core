using ApiServer.Framework.Core.Exceptions;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Framework.Sample.Id4.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ConnectController: ControllerBase
    {

        private readonly IClientSecretValidator _clientValidator;
        private readonly ITokenRequestValidator _requestValidator;
        private readonly ITokenResponseGenerator _responseGenerator;
        private IHttpContextAccessor _accessor;
        private readonly ILogger<ConnectController> _logger;
        private readonly IEventService _events;

        public ConnectController(IClientSecretValidator clientValidator, ITokenRequestValidator requestValidator, ITokenResponseGenerator responseGenerator, IHttpContextAccessor accessor, ILogger<ConnectController> logger, IEventService events)
        {
            _clientValidator = clientValidator;
            _requestValidator = requestValidator;
            _responseGenerator = responseGenerator;
            _accessor = accessor;
            _logger = logger;
            _events = events;
        }

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Register()
        {
            _logger.LogTrace("Processing token request.");

            var context = _accessor.HttpContext;
            // validate HTTP
            if (!HttpMethods.IsPost(context.Request.Method) || !context.Request.HasFormContentType)
            {
                _logger.LogWarning("Invalid HTTP request for token endpoint");
                return StatusCode(400, Error(OidcConstants.TokenErrors.InvalidRequest,"http method must be post. request has form content"));
            }

            // validate client
            var clientResult = await _clientValidator.ValidateAsync(_accessor.HttpContext);

            if (clientResult.Client == null)
            {
                return StatusCode(400,Error(OidcConstants.TokenErrors.InvalidClient));
            }

            // validate request
            var form = (await _accessor.HttpContext.Request.ReadFormAsync()).AsNameValueCollection();
            _logger.LogTrace("Calling into token request validator: {type}", _requestValidator.GetType().FullName);
            var requestResult = await _requestValidator.ValidateRequestAsync(form, clientResult);

            if (requestResult.IsError)
            {
                await _events.RaiseAsync(new TokenIssuedFailureEvent(requestResult));

                return StatusCode(400,Error(requestResult.Error, requestResult.ErrorDescription, requestResult.CustomResponse));
            }

            // create response
            _logger.LogTrace("Calling into token request response generator: {type}", _responseGenerator.GetType().FullName);
            var response = await _responseGenerator.ProcessAsync(requestResult);

            await _events.RaiseAsync(new TokenIssuedSuccessEvent(response, requestResult));
            // return result
            _logger.LogDebug("Token request success.");
            return StatusCode(200,response);
        }

        private TokenErrorResponse Error(string error, string errorDescription = null, Dictionary<string, object> custom = null)
        {
            return new TokenErrorResponse
            {
                Error = error,
                ErrorDescription = errorDescription,
                Custom = custom
            };
        }
    }
}
