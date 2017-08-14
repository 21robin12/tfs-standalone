using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static TfsStandalone.DiffEngine.Diff;

namespace TfsStandalone.Services
{
    // TODO so many opportunities for NRE here... check all the .First() calls and other places
    public class ShelvesetDiffService
    {
        public IEnumerable<string> GetShelvesets(string projectCollectionUrl, string username)
        {
            var vcs = VCS.Get(projectCollectionUrl);
            var shelvesets = vcs.QueryShelvesets(null, username);
            return shelvesets.OrderByDescending(x => x.CreationDate).Select(x => x.Name);
        }

        public IEnumerable<string> GetShelvesetFilenames(string projectCollectionUrl, string username, string shelvesetName)
        {
            var vcs = VCS.Get(projectCollectionUrl);
            var pending = GetPendingChanges(vcs, username, shelvesetName);
            return pending.Where(x => HasChanged(x, vcs, username)).Select(x => x.ServerItem);
        }

        public IEnumerable<ResultData.DiffLine> Diff(string projectCollectionUrl, string username, string shelvesetName, string serverItem)
        {
            var vcs = VCS.Get(projectCollectionUrl);
            var pending = GetPendingChanges(vcs, username, shelvesetName);
            var shelvesetFile = pending.Where(x => x.ServerItem == serverItem).First();
            var shelvesetFileText = GetShelvesetFileText(shelvesetFile, vcs, username);
            var localFileText = GetLocalFileText(shelvesetFile, vcs, username);
            return DiffEngine.Diff.Calculate(shelvesetFileText, localFileText).Select(x => new ResultData.DiffLine
            {
                DiffType = (ResultData.DiffSectionType)x.diffType,
                Text = x.text
            });
        }

        private PendingChange[] GetPendingChanges(VersionControlServer vcs, string username, string shelvesetName)
        {
            var shelveset = vcs.QueryShelvesets(shelvesetName, username).First();
            var change = vcs.QueryShelvedChanges(shelveset).First();
            return change.PendingChanges;
        }

        private bool HasChanged(PendingChange shelvesetFile, VersionControlServer vcs, string username)
        {
            var shelvesetFileText = GetShelvesetFileText(shelvesetFile, vcs, username);
            var localFileText = GetLocalFileText(shelvesetFile, vcs, username);
            return shelvesetFileText != localFileText;
        }

        private string GetShelvesetFileText(PendingChange shelvesetFile, VersionControlServer vcs, string username)
        {
            var stream = shelvesetFile.DownloadShelvedFile();
            using (var reader = new StreamReader(stream))
            {
                var shelvesetText = reader.ReadToEnd();
                return shelvesetText;
            }
        }

        private string GetLocalFileText(PendingChange shelvesetFile, VersionControlServer vcs, string username)
        {
            var machineName = Environment.MachineName;
            var workspace = vcs.GetWorkspace(machineName, username);
            var localFile = workspace.GetLocalItemForServerItem(shelvesetFile.ServerItem);
            var localText = File.ReadAllText(localFile);
            return localText;
        }
    }
}
