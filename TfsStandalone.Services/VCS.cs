using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsStandalone.Services
{
    internal static class VCS
    {
        internal static VersionControlServer Get(string projectCollectionUrl)
        {
            var teamProjectCollection = new TfsTeamProjectCollection(new Uri(projectCollectionUrl));
            return teamProjectCollection.GetService<VersionControlServer>();
        }
    }
}
