using TfsStandalone.UI.Models.Pages;

namespace TfsStandalone.UI.Controllers
{
    using Infrastructure;
    using Service;

    public class MainController : Controller
    {
        public void Main()
        {
            var vm = new MainViewModel();
            vm.ProjectCollection = ConfigManager.ProjectCollection(0);
            Render(vm, "Views/Main.cshtml");
        }

        public void Authenticate()
        {
            // TODO replace with something else - just needs to be a TFS call to initiate authentication
            var projectCollection = ConfigManager.ProjectCollection(0);
            var shelvesets = ShelvesetDiff.GetShelvesets(projectCollection.Url, projectCollection.Username,
                projectCollection.AltCredentials?.Username, projectCollection.AltCredentials?.Password);
        }

        private void Render<T>(T model, string view)
        {
            BrowserForm.Instance.Render(model, view);
        }
    }
}
