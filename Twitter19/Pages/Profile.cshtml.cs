using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Twitter19.Pages
{
    public class ProfileModel : PageModel
    {
        [BindProperty]
        public IFormFile Header { get; set; }
        [BindProperty]
        public IFormFile Profile { get; set; }
        [BindProperty]
        public string Bio { get; set; }
        public void OnGet()
        {

        }
    }
}
