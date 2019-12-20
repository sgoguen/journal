module Parsers.Lambda

#if INTERACTIVE
#load "Parsec.fs"
#endif

open System
open Parsec.V1

type Term =
    | Var of char
    | App of Term * Term
    | Lambda of char * Term

let space = (char ' ') |> ignore
let openP = char '(' |> ignore
let var = pred (fun c -> Char.IsLetter(c))
let rec word = oneOrMore letter |> map listToString
and apply = separated space var 
    // <*> space <*> var // |> map (fun ((x, _), y) -> (x, y))

module Tests =
    let t1 = test apply "x y z"