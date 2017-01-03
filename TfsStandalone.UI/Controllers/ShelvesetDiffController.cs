using System.Linq;
using TfsStandalone.Service;

namespace TfsStandalone.UI.Controllers
{
    using Infrastructure;

    public class ShelvesetDiffController : Controller
    {
        public string GetShelvesets()
        {
            var projectCollection = ConfigManager.ProjectCollection(0);
            
            var shelvesets = ShelvesetDiff.GetShelvesets(projectCollection.Url, projectCollection.Username,
                projectCollection.AltCredentials?.Username, projectCollection.AltCredentials?.Password);

            return Json(shelvesets);
        }

        public string GetShelvesetFilenames(string shelvesetName)
        {
            // TODO configmanager into config proj. use in F# project to avoid all of these
            var projectCollection = ConfigManager.ProjectCollection(0);

            var filenames = ShelvesetDiff.GetShelvesetFilenames(projectCollection.Url, projectCollection.Username,
                projectCollection.AltCredentials?.Username, projectCollection.AltCredentials?.Password, shelvesetName);

            return Json(filenames);
        }

        public string GetDiff(string shelvesetName, string serverItem)
        {
            var projectCollection = ConfigManager.ProjectCollection(0);

            var diffLines = ShelvesetDiff.Diff(projectCollection.Url, projectCollection.Username,
                projectCollection.AltCredentials?.Username, projectCollection.AltCredentials?.Password, shelvesetName,
                serverItem).ToList();

            return Json(diffLines);
        }
    }
}
