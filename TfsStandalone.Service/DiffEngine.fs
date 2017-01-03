module TfsStandalone.Service.DiffEngine

// from http://devdirective.com/post/91/creating-a-reusable-though-simple-diff-implementation-in-csharp-part-1
// based on https://en.wikipedia.org/wiki/Longest_common_substring_problem
// much easier, but not as good as DiffMatchPatch (Myers) solution

open System
open System.Text

type DiffSectionType = Copy = 0 | Insert = 1 | Delete = 2
type DiffSection<'a> = { diffType: DiffSectionType; items: 'a list }
type DiffLine = { diffType: DiffSectionType; text: string  }

type private LongestCommonSubarrayResult<'a> = { success: bool; index1: int; index2: int; items: 'a list }

let private getCommonListFromStart (array1: 'a array) (array2: 'a array): 'a list =
    List.foldTogetherContinuation (fun acc x y k -> if x = y then k (x::acc) else acc) [] (List.ofArray array1) (List.ofArray array2) |> List.rev // iterate backwards in foldBack style to avoid this?

let private getLongestCommonSubarray (array1: 'a array) firstStart firstEnd (array2: 'a array) secondStart secondEnd: LongestCommonSubarrayResult<'a> =
    // TODO rewrite in functional style
    let mutable result = { success = false; index1 = 0; index2 = 0; items = [] }
    for index1 in [firstStart..(firstEnd - 1)] do
        for index2 in [secondStart..(secondEnd - 1)] do
            if array1.[index1] = array2.[index2] then
                let sub1 = Array.sub array1 index1 (firstEnd - index1)
                let sub2 = Array.sub array2 index2 (secondEnd - index2)

                let items = getCommonListFromStart sub1 sub2
                if items.Length > result.items.Length then
                    result <- { success = true; index1 = index1; index2 = index2; items = items }
    result

let rec private diff (array1: 'a array) firstStart firstEnd (array2: 'a array) secondStart secondEnd: DiffSection<'a> list =
    // TODO rewrite in functional style
    [
        let lcs = getLongestCommonSubarray array1 firstStart firstEnd array2 secondStart secondEnd
        match lcs.success with
            | true ->
                let sectionsBefore = diff array1 firstStart lcs.index1 array2 secondStart lcs.index2
                for section in sectionsBefore do
                    yield section
                yield { diffType = DiffSectionType.Copy; items = lcs.items }
                let sectionsAfter = diff array1 (lcs.index1 + lcs.items.Length) firstEnd array2 (lcs.index2 + lcs.items.Length) secondEnd
                for section in sectionsAfter do 
                    yield section
            | false -> 
                if firstStart < firstEnd then
                    yield { diffType = DiffSectionType.Delete; items = Array.sub array1 firstStart (firstEnd - firstStart) |> List.ofArray }
                if secondStart < secondEnd then
                    yield { diffType = DiffSectionType.Insert; items = Array.sub array2 secondStart (secondEnd - secondStart) |> List.ofArray }
    ]           

let Diff text1 text2 =
    let sectionToLines (diff: DiffSection<'a>) = 
        let lines = List.fold (fun acc (item:string) -> ({ diffType = diff.diffType; text = item.Replace("\t", "    ") })::acc) [] diff.items // TODO restrict to string elsewhere?
        lines |> List.rev // TODO foldback to avoid this?

    let getLines (t:string) = t.Split([|"\r\n"|], StringSplitOptions.None)
    let lines1 = getLines text1
    let lines2 = getLines text2
    let diffs = diff lines1 0 lines1.Length lines2 0 lines2.Length
    
    let result = List.fold (fun (acc: DiffLine list) item -> List.concat(Seq.ofList([acc; sectionToLines(item)]))) [] diffs
    result