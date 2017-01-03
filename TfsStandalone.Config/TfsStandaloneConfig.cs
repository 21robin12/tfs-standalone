namespace TfsStandalone.Config
{
    using System.Collections.Generic;

    public class TfsStandaloneConfig
    {
        public IEnumerable<TfsProjectCollection> ProjectCollections { get; set; }
    }
}
