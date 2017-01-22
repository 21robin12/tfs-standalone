module TfsStandalone.Service.UnmergedChanges

open System
open System.IO
open System.Web
open System.Net
open Microsoft.TeamFoundation.VersionControl.Client
open System.Collections.Generic

type UnmergedChange = { ChangesetId: int; Comment: string; WorkItemTitle: string; CreationDate: DateTime }

let Get (projectCollectionUrl, username, fromBranch, toBranch, ignoredChangesets: IEnumerable<int>): IEnumerable<UnmergedChange> =
    let mapUnmergedChangeset (mc: MergeCandidate): UnmergedChange = 
        let workItemTitle (x: AssociatedWorkItemInfo) = sprintf "%i - %s" x.Id x.Title
        let title = match mc.Changeset.AssociatedWorkItems |> Array.toList with
            | [x] -> workItemTitle x
            | x::xs -> workItemTitle x
            | [] -> ""
        { ChangesetId = mc.Changeset.ChangesetId; Comment = mc.Changeset.Comment; WorkItemTitle = title; CreationDate = mc.Changeset.CreationDate }
    
    let vcs = VersionControl.GetVersionControlServer (projectCollectionUrl)
    let ignoredChangesetsList = List.ofSeq ignoredChangesets
    let unmergedChanges = vcs.GetMergeCandidates(fromBranch, toBranch, RecursionType.Full)
    unmergedChanges
        |> List.ofArray
        |> List.filter (fun x -> x.Changeset.Owner.ToLower().Contains username)
        |> List.filter (fun x -> not (List.exists (fun y -> y = x.Changeset.ChangesetId) ignoredChangesetsList))
        |> List.map mapUnmergedChangeset
        |> Seq.ofList