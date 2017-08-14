using TfsStandalone.UI.Infrastructure;
using TfsStandalone.UI.Models.Pages;
using AuthenticationService = TfsStandalone.Services.AuthenticationService;

namespace TfsStandalone.UI.Controllers
{
    public class MainController : Controller
    {
        private readonly AuthenticationService _authenticationService;

        public MainController(AuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public void Main()
        {
            _authenticationService.ClearCachedTfsCredentials(ConfigManager.ProjectCollection(0).Url);

            var vm = new MainViewModel();
            Render(vm, "Views/Main.cshtml");
        }

        public string AuthenticateAndGetSettings()
        {
            var projectCollection = ConfigManager.ProjectCollection(0);
            _authenticationService.EnsureAuthenticated(projectCollection.Url);
            return Json(projectCollection);
        }

        private void Render<T>(T model, string view)
        {
            BrowserForm.Instance.Render(model, view);
        }
    }
}
