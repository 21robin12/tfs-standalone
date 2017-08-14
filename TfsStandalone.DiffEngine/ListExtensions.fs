module List

let rec foldTogether f acc list1 list2 =
    match list1 with
    | []    -> acc
    | x::xs ->
        match list2 with
            | []    -> acc
            | y::ys -> foldTogether f (f acc x y) xs ys

// a "continuation fold" or CPS (Continuation-Passing Style) fold, from https://sidburn.github.io/blog/2016/05/07/cps-fold
let rec foldContinuation f acc xs =
    match xs with
    | []    -> acc
    | x::xs -> f acc x (fun lacc -> foldContinuation f lacc xs)

let rec foldTogetherContinuation f acc list1 list2 =
    match list1 with
    | []    -> acc
    | x::xs ->
        match list2 with
            | []    -> acc
            | y::ys -> f acc x y (fun lacc -> foldTogetherContinuation f lacc xs ys)