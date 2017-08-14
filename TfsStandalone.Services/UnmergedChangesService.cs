using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TfsStandalone.Services.ResultData;

namespace TfsStandalone.Services
{
    public class UnmergedChangesService
    {
        public IEnumerable<UnmergedChange> GetUnmergedChanges(string projectCollectionUrl, string username, string fromBranch, string toBranch, IList<int> ignoredChangesets)
        {
            username = username.ToLower();
            var vcs = VCS.Get(projectCollectionUrl);
            var unmergedChanges = vcs.GetMergeCandidates(fromBranch, toBranch, RecursionType.Full);
            return unmergedChanges
                .Where(x => x.Changeset.Owner.ToLower().Contains(username) && !ignoredChangesets.Contains(x.Changeset.ChangesetId))
                .Select(x => new UnmergedChange
                {
                    ChangesetId = x.Changeset.ChangesetId,
                    Comment = x.Changeset.Comment,
                    WorkItemTitle = WorkItemTitle(x),
                    CreationDate = x.Changeset.CreationDate
                });
        }

        private string WorkItemTitle(MergeCandidate mc)
        {
            var workItem = mc.Changeset.AssociatedWorkItems.FirstOrDefault();
            return workItem == null ? string.Empty : $"{workItem.Id} - {workItem.Title}";
        }
    }
}
