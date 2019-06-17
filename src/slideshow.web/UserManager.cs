using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using slideshow.web.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace slideshow.web
{
    public class UserManager
    {

        public UserManager()
        {
        }

        public async Task SignInAsync(HttpContext httpContext, LogInViewModel user, bool isPersistent = false)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
            {
                var salt = Environment.GetEnvironmentVariable("USERMANAGER_SALT") ?? throw new ArgumentNullException("env salt missing");
                var expected = Environment.GetEnvironmentVariable($"USERMANAGER_{user.Name.ToUpper()}_HASH") ?? throw new ArgumentNullException("env hash missing for " + user.Name);
                var actual = CreateHash(user.Pass, salt);

                if (!Validate(user.Pass, salt, expected))
                {
                    throw new UnauthorizedAccessException("password missmatch");
                }
            }

            var identity = new ClaimsIdentity(this.GetUserClaims(user), CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            var props = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = true,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddDays(14),
            };
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);
        }

        public async Task SignOutAsync(HttpContext httpContext)
        {
            await httpContext.SignOutAsync();
        }

        private IEnumerable<Claim> GetUserClaims(LogInViewModel user)
        {
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Name.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.Name));
            claims.Add(new Claim(ClaimTypes.Email, user.Name + "@domain.tld"));
            claims.AddRange(this.GetRoleClaims(user));
            return claims;
        }

        private IEnumerable<Claim> GetRoleClaims(LogInViewModel user)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Role, user.Name));
            return claims;
        }

        // https://tahirnaushad.com/2017/09/09/hashing-in-asp-net-core-2-0/
        private static string CreateHash(string value, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                                password: value,
                                salt: Encoding.UTF8.GetBytes(salt),
                                prf: KeyDerivationPrf.HMACSHA512,
                                iterationCount: 10000,
                                numBytesRequested: 256 / 8);
            return String.Join(",", valueBytes);
            //return Convert.ToBase64String(valueBytes);
        }

        private static bool Validate(string value, string salt, string hash)
        {
            return CreateHash(value, salt) == hash;
        }

        private static string CreateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return String.Join(",", randomBytes);
                //return Convert.ToBase64String(randomBytes);
            }
        }
    }
}
