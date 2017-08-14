using System.Linq;

namespace TfsStandalone.UI.Controllers
{
    using Infrastructure;
    using Services;

    public class ShelvesetDiffController : Controller
    {
        private readonly ShelvesetDiffService _shelvesetDiffService;

        public ShelvesetDiffController(ShelvesetDiffService shelvesetDiffService)
        {
            _shelvesetDiffService = shelvesetDiffService;
        }

        public string GetShelvesets()
        {
            var projectCollection = ConfigManager.ProjectCollection(0);
            
            var shelvesets = _shelvesetDiffService.GetShelvesets(projectCollection.Url, projectCollection.Username);

            return Json(shelvesets);
        }

        public string GetShelvesetFilenames(string shelvesetName)
        {
            // TODO configmanager into config proj. use in F# project to avoid all of these
            var projectCollection = ConfigManager.ProjectCollection(0);

            var filenames = _shelvesetDiffService.GetShelvesetFilenames(projectCollection.Url, projectCollection.Username, shelvesetName);

            return Json(filenames);
        }

        public string GetDiff(string shelvesetName, string serverItem)
        {
            var projectCollection = ConfigManager.ProjectCollection(0);

            var diffLines = _shelvesetDiffService.Diff(projectCollection.Url, projectCollection.Username, shelvesetName, serverItem).ToList();

            return Json(diffLines);
        }
    }
}
