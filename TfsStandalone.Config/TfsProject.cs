namespace TfsStandalone.Config
{
    using System.Collections.Generic;

    public class TfsProject
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string DeveloperCmdPath { get; set; }
        public string WorkspacePath { get; set; }
        public IEnumerable<TfsBranchComparison> BranchComparisons { get; set; }
        public IEnumerable<int> IgnoredChangesets { get; set; }
    }
}
