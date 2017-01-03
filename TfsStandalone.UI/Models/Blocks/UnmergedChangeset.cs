using System;

namespace TfsStandalone.UI.Models.Blocks
{
    class UnmergedChangeset
    {
        public int ChangesetId { get; set; }
        public string Comment { get; set; }
        public string WorkItemTitle { get; set; }
        public string CreationDate { get; set; }
    }
}
