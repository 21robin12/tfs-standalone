using TfsStandalone.UI.Models.Pages;

namespace TfsStandalone.UI.Controllers
{
    public class MainController : Controller
    {
        public void Main()
        {
            var vm = new MainViewModel();
            Render(vm, "Views/Main.cshtml");
        }

        private void Render<T>(T model, string view)
        {
            BrowserForm.Instance.Render(model, view);
        }
    }
}
