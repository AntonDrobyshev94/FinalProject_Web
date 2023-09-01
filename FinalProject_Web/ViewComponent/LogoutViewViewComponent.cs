using Microsoft.AspNetCore.Mvc;

namespace FinalProject_Web.Component
{
    public class LogoutViewViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
