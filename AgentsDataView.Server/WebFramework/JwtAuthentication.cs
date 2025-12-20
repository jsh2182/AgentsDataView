using AgentsDataView.Common;
using AgentsDataView.Common.Exceptions;
using AgentsDataView.Data.Contracts;
using AgentsDataView.Data.Repositories;
using AgentsDataView.Server.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace AgentsDataView.Server.WebFramework
{
    public static class JwtAuthentication
    {
        public static void AddJwtAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var secretKey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
                var encryptionKey = Encoding.UTF8.GetBytes(jwtSettings.EncryptKey);
                TokenValidationParameters validationParameters = new ()
                {
                    ClockSkew = TimeSpan.Zero,
                    RequireSignedTokens = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),

                    RequireExpirationTime = true,
                    ValidateLifetime = true,

                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,

                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,

                    TokenDecryptionKey = new SymmetricSecurityKey(encryptionKey),

                };
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = validationParameters;
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception != null)
                        {
                            throw new AppException(ApiResultStatusCode.UnAuthorized, _translateMessage("Authentication Failed"), HttpStatusCode.Unauthorized);

                        }
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = async context =>
                    {
                        IUserRepository userRepo = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

                        var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                        if (claimsIdentity?.Claims?.Any() != true)
                        {
                            context.Fail("This Token Has No Claims.");
                        }
                        int userID = claimsIdentity.GetUserId<int>();
                        bool isValid = await userRepo.UserIsValid(userID, context.HttpContext.RequestAborted);
                        
                        if (!isValid)
                        {
                            //کاربر درخواست کننده معتبر نیست
                            context.Fail("The Requesting User Is Invalid.");
                        }
                        string? controllerName = context.HttpContext.GetRouteData().Values["controller"]?.ToString();
                        if(controllerName?.ToLower() == "companies")
                        {
                            string? userName = claimsIdentity.GetUserName();
                            if(userName?.ToLower() != "super")
                            {
                                context.Fail("access denied");
                            }
                        }
                    },
                    OnChallenge = context =>
                    {
                        if (context.AuthenticateFailure != null)
                        {
                            throw new AppException(ApiResultStatusCode.UnAuthorized, _translateMessage(context.AuthenticateFailure.Message), HttpStatusCode.Unauthorized, context.AuthenticateFailure, null);
                        }
                        throw new AppException(ApiResultStatusCode.UnAuthorized, _translateMessage("You Are Unauthorized To Access This Resource."), HttpStatusCode.Unauthorized);
                    }
                };
            });
        }
        private static string _translateMessage(string? message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return "";
            }
            message = message.ToLower();
            return MessagesDictionary.Messages[message] ?? message;
        }
    }
}
