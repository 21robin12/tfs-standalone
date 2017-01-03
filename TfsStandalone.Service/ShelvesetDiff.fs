module TfsStandalone.Service.ShelvesetDiff

open System
open System.IO
open Microsoft.TeamFoundation.VersionControl.Client

let private GetPendingChanges(shelvesetName, projectCollectionUrl, username, altUsername, altPassword) =
    let vcs = VersionControl.GetVersionControlServer (projectCollectionUrl, altUsername, altPassword)
    let shelveset = vcs.QueryShelvesets(shelvesetName, username).[0]
    let change = vcs.QueryShelvedChanges(shelveset).[0]
    let pending = change.PendingChanges;
    pending
        |> List.ofArray

let private HasChanged (shelvesetFile: PendingChange, projectCollectionUrl, username, altUsername, altPassword) =
    let stream = shelvesetFile.DownloadShelvedFile()
    use reader = new StreamReader(stream)
    let shelvesetText = reader.ReadToEnd()

    let machineName = Environment.MachineName
    let vcs = VersionControl.GetVersionControlServer (projectCollectionUrl, altUsername, altPassword) // TODO remove this & elsewhere
    let workspace = vcs.GetWorkspace(machineName, username);
    let localFile = workspace.GetLocalItemForServerItem(shelvesetFile.ServerItem)
    let localText = File.ReadAllText(localFile)
    shelvesetText <> localText

let GetShelvesetFilenames(projectCollectionUrl, username, altUsername, altPassword, shelvesetName) =
    let pending = GetPendingChanges(shelvesetName, projectCollectionUrl, username, altUsername, altPassword)
    pending
        |> List.filter (fun x -> HasChanged(x, projectCollectionUrl, username, altUsername, altPassword)) // TODO check this doesn't take ages!
        |> List.map (fun x -> x.ServerItem)    

let GetShelvesets (projectCollectionUrl, username, altUsername, altPassword) =
    let vcs = VersionControl.GetVersionControlServer (projectCollectionUrl, altUsername, altPassword)
    let shelvesets = vcs.QueryShelvesets(null, username)
    shelvesets
        |> List.ofSeq
        |> List.sortByDescending (fun x -> x.CreationDate)
        |> List.map (fun x -> x.Name)
        |> Seq.ofList

let Diff(projectCollectionUrl, username, altUsername, altPassword, shelvesetName, serverItem) =
    let pending = GetPendingChanges(shelvesetName, projectCollectionUrl, username, altUsername, altPassword)
    let shelvesetFile = List.find (fun (item:PendingChange) -> item.ServerItem = serverItem) pending
    let stream = shelvesetFile.DownloadShelvedFile()
    use reader = new StreamReader(stream)
    let shelvesetText = reader.ReadToEnd()

    let machineName = Environment.MachineName
    let vcs = VersionControl.GetVersionControlServer (projectCollectionUrl, altUsername, altPassword) // TODO remove this & elsewhere
    let workspace = vcs.GetWorkspace(machineName, username);
    let localFile = workspace.GetLocalItemForServerItem(serverItem)
    let localText = File.ReadAllText(localFile)

    let diff = DiffEngine.Diff shelvesetText localText
    diff
        |> Seq.ofList