module TfsStandalone.Service.VersionControl

open System
open System.Net
open Microsoft.TeamFoundation.VersionControl.Client
open System.Collections.Generic

let GetVersionControlServer (tfsUrl, altUsername, altPassword): VersionControlServer =
    let stringToOption s =
        match s with
            | null | "" -> None
            | _ -> Some s

    let username = stringToOption altUsername
    let password = stringToOption altPassword

    let getTeamProjectCollectionAltCreds () =
        let netCred = NetworkCredential(username.Value, password.Value)
        let basicCred = Microsoft.TeamFoundation.Client.BasicAuthCredential(netCred)
        let tfsCred = Microsoft.TeamFoundation.Client.TfsClientCredentials(basicCred)
        let collection = new Microsoft.TeamFoundation.Client.TfsTeamProjectCollection(Uri(tfsUrl), tfsCred)
        collection.Authenticate()
        collection.EnsureAuthenticated()
        collection

    let teamProjectCollection = match username with
        | None -> new Microsoft.TeamFoundation.Client.TfsTeamProjectCollection(Uri(tfsUrl))
        | Some u -> match password with
            | None -> failwith "altUsername provided, but no altPassword provided"
            | Some p ->  getTeamProjectCollectionAltCreds()

    teamProjectCollection.GetService<VersionControlServer>()