using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Owin;
using System;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace web_ioc
{
    public static class AuthConfig
    {
        internal static void Setup(IAppBuilder app)
        {
            var issuer = ConfigurationManager.AppSettings["issuer"];
            var validAudiences = ConfigurationManager.AppSettings["audiences"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var certificatePath = ConfigurationManager.AppSettings["certificatePath"];

            var certificate = new X509Certificate2(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, certificatePath));
            var key = new X509SecurityKey(certificate);

            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                AllowedAudiences = validAudiences,
                Realm = issuer,
                IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                {
                    new X509CertificateSecurityTokenProvider(issuer, certificate),
                },
                TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    IssuerSigningKeyResolver = (a, b, c, d) => new X509SecurityKey(certificate),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudiences = validAudiences,
                    ClockSkew = TimeSpan.Zero
                }
            });

        }
    }
}