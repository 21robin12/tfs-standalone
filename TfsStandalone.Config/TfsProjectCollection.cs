namespace TfsStandalone.Config
{
    using System.Collections.Generic;

    public class TfsProjectCollection
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public IEnumerable<TfsProject> Projects { get; set; }
    }
}
