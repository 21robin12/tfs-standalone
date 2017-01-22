namespace TfsStandalone.UI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CefSharp;
    using Config;
    using Infrastructure;

    public class SettingsController : Controller
    {
        public void SaveSettings(string projectCollectionUrl, string username, string workspacePath, string developerCmdPath, string branchComparisons, string changesetsToIgnore)
        {
            var projectCollection = new TfsProjectCollection
            {
                Name = "project-colllection",
                Url = projectCollectionUrl, 
                Username = username,
                Projects = new List<TfsProject>
                {
                    new TfsProject
                    {
                        Id = Guid.NewGuid().ToString(),
                        DeveloperCmdPath = developerCmdPath, 
                        Name = "project",
                        WorkspacePath = workspacePath,
                        BranchComparisons = ParseBranchComparisons(branchComparisons),
                        IgnoredChangesets = ParseIgnoredChangesets(changesetsToIgnore)
                    }
                }
            };

            ConfigManager.SaveConfig(new TfsStandaloneConfig
            {
                ProjectCollections = new List<TfsProjectCollection> {projectCollection}
            });
        }

        private IEnumerable<TfsBranchComparison> ParseBranchComparisons(string branchComparisons)
        {
            return branchComparisons.Split(',')
                .Select(comparison => comparison.Split('>'))
                .Select(split => new TfsBranchComparison {From = split[0], To = split[1]});
        }

        private IEnumerable<int> ParseIgnoredChangesets(string ignoredChangesets)
        {
            return ignoredChangesets.Split(',').Select(int.Parse);
        } 
    }
}
