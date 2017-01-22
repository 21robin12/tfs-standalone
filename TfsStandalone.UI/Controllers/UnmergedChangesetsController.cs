using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TfsStandalone.Service;
using TfsStandalone.UI.Models.Blocks;

namespace TfsStandalone.UI.Controllers
{
    using Infrastructure;

    public class UnmergedChangesetsController : Controller
    {
        public string Load()
        {
            var projectCollection = ConfigManager.ProjectCollection(0);
            var project = projectCollection.Projects.FirstOrDefault();
            return Json(project);
        }

        public string GetUnmergedChanges(string fromBranch, string toBranch, string projectId)
        {
            var project = ConfigManager.Project(projectId);
            var projectCollection = ConfigManager.ProjectCollection(project);

            var changes = UnmergedChanges.Get(projectCollection.Url, projectCollection.Username, fromBranch, toBranch, project.IgnoredChangesets);

            var mapped = changes.Select(x => new UnmergedChangeset
            {
                ChangesetId = x.ChangesetId,
                Comment = x.Comment,
                CreationDate = x.CreationDate.ToString("dd/MM/yyyy"),
                WorkItemTitle = x.WorkItemTitle
            }).OrderByDescending(x => x.ChangesetId);

            return Json(mapped);
        }

        public void Merge(int changesetId, string projectId, string from, string to)
        {
            var project = ConfigManager.Project(projectId);

            // is this silly? couldn't get it working without the physical batch file
            var bat = new StringBuilder();
            bat.AppendLine($"call \"{project.DeveloperCmdPath}\" x86_amd64");
            bat.AppendLine($"cd \"{project.WorkspacePath}\"");
            bat.AppendLine($"tf merge /recursive /version:{changesetId}~{changesetId} {from} {to}");
            bat.AppendLine("pause");
            bat.AppendLine("exit");

            var path = TempFilePath();
            File.WriteAllText(path, bat.ToString());

            var process = new Process();
            var startinfo = new ProcessStartInfo("cmd.exe", "/K " + path)
            {
                UseShellExecute = false
            };

            process.StartInfo = startinfo;
            process.Start();
            process.WaitForExit();

            File.Delete(path);
        }

        private string TempFilePath()
        {
            Func<string, string> combine = (name) => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{name}.bat");
            var filename = "_temp";
            while (File.Exists(combine(filename)))
            {
                filename = "_" + filename;
            }

            return combine(filename);
        }
    }
}
