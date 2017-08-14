using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace TfsStandalone.Services
{
    public class AuthenticationService
    {
        public void EnsureAuthenticated(string projectCollectionUrl)
        {
            // needs to make some arbitrary call to TFS - this brings up the login window if not authenticated
            var vcs = VCS.Get(projectCollectionUrl);
            vcs.QueryShelvesets("", "");
        }

        public void ClearCachedTfsCredentials(string projectCollectionUrl)
        {
            try
            {
                var clientCredentails = new VssClientCredentialStorage();
                var tfsUri = new Uri(projectCollectionUrl);
                var federatedCredentials = clientCredentails.RetrieveToken(tfsUri, VssCredentialsType.Federated);
                clientCredentails.RemoveToken(tfsUri, federatedCredentials);
            }
            catch (Exception)
            {
                // can't always clear cached creds - seems to work only for VSO
            }
        }
    }
}
