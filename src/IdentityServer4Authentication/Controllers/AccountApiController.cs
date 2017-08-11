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


namespace IdentityServer4Authentication.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AccountApiController : Controller
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "Important Info for people in low offices #1",
                "Important Info for people in low offices #2" };
        }
    }
}