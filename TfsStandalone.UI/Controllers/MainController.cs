using TfsStandalone.UI.Models.Pages;

namespace TfsStandalone.UI.Controllers
{
    using Infrastructure;

    public class MainController : Controller
    {
        public void Main()
        {
            var vm = new MainViewModel();
            vm.ProjectCollection = ConfigManager.ProjectCollection(0);
            Render(vm, "Views/Main.cshtml");
        }

        private void Render<T>(T model, string view)
        {
            BrowserForm.Instance.Render(model, view);
        }
    }
}
