using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ApiServer.Framework.Core.Web.Authentication
{
    public class HeaderAuthenticationHandler : AuthenticationHandler<HeaderAuthenticationOptions>
    {
        public IAccessTokenStore _accessTokenStore;

        public HeaderAuthenticationHandler(
            IOptionsMonitor<HeaderAuthenticationOptions> options
            , ILoggerFactory logger
            , UrlEncoder encoder
            , ISystemClock clock
            , IAccessTokenStore accessTokenStore
            ) : base(options, logger, encoder, clock)
        {
            _accessTokenStore = accessTokenStore;
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            return base.HandleChallengeAsync(properties);
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(Options.AccessTokenHeader))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }
            try
            {
                string authStr = Request.Headers[Options.AccessTokenHeader];
                authStr = authStr.Replace("Bearer ","");
                var userClaims = _accessTokenStore.GetTokenToClaims(authStr);
                var principal = new ClaimsPrincipal(new ClaimsIdentity(userClaims, Scheme.Name));

                var ticket = new AuthenticationTicket(
                    principal,
                    Scheme.Name
                );
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch (Exception ex) {
                this.Logger.LogDebug($"Authenticate Fail:{ex.Message}");
                return Task.FromResult(AuthenticateResult.Fail(ex.Message));
            }
        }
    }
}
