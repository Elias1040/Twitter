using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Twitter19.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly Session session;
        public IndexModel(ILogger<IndexModel> logger, Session session)
        {
            _logger = logger;
            this.session = session;
        }

        public IActionResult OnGet()
        {
            return session.LoggedIn();
        }
    }
}
