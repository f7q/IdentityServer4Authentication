using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using IdentityServer4Authentication.Models;
using IdentityServer4Authentication.Models.AccountViewModels;
using IdentityServer4Authentication.Services;
using IdentityServer4.AccessTokenValidation;

namespace IdentityServer4Authentication.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    public class AccountApiController : ControllerBase
    {
        private readonly Dictionary<string, string> list;
        public AccountApiController()
        {
            list = new Dictionary<string, string>();
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "Important Info for people in low offices #1",
                "Important Info for people in low offices #2" };
        }

        [HttpGet("All")]
        public Dictionary<string, string> GetAll()
        {
            return list;
        }

        [HttpPost]
        public void Post(string key, string value)
        {
            list[key] = value;
        }

        [HttpPut]
        public void Put(string key, string value)
        {
            list.Add(key, value);
        }

        [HttpDelete]
        public void Delete(string key)
        {
            list.Remove(key);
        }
    }
}