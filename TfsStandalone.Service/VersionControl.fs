module TfsStandalone.Service.VersionControl

open System
open System.Net
open Microsoft.TeamFoundation.VersionControl.Client
open System.Collections.Generic

let GetVersionControlServer (tfsUrl): VersionControlServer =
    let teamProjectCollection = new Microsoft.TeamFoundation.Client.TfsTeamProjectCollection(Uri(tfsUrl))
    teamProjectCollection.GetService<VersionControlServer>()