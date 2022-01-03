using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Twitter
{
    public class Session
    {
        private readonly HttpContext httpContext;

        public Session(IHttpContextAccessor contextAccessor)
        {
            httpContext = contextAccessor.HttpContext;
        }

        public IActionResult LoggedIn()
        {
            if (httpContext.Session.GetString("Logged in") == "1")
            {
                return new PageResult();
            }
            else
            {
                httpContext.Session.Remove("Logged in");
                return new RedirectToPageResult("Login");
            }
        }

    }
}

