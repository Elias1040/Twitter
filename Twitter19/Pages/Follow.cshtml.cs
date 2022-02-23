using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Twitter19.Repo;

namespace Twitter19.Pages
{
    public class FollowModel : PageModel
    {
        #region PrivateReadonly

        private readonly IRepo _repo;

        public FollowModel(IRepo repo)
        {
            _repo = repo;
        }

        #endregion PrivateReadonly

        public IActionResult OnGet(int id)
        {
            if (HttpContext.Session.GetString("Logged in") != "1")
                return RedirectToPage("Login");
            else if (HttpContext.Session.GetInt32("ID") == id)
                return RedirectToPage("Profile");

            _repo.Follow((int)HttpContext.Session.GetInt32("ID"), id);
            _repo.CreateRoom((int)HttpContext.Session.GetInt32("ID"), id);
            return RedirectToPage("OtherProfiles", new { id = id });
        }
    }
}