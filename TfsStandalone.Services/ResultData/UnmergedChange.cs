using System;

namespace TfsStandalone.Services.ResultData
{
    public class UnmergedChange
    {
        public int ChangesetId { get; set; }
        public string Comment { get; set; }
        public string WorkItemTitle { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
