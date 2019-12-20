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

let letter = pred Char.IsLetter
let var = letter |> map Var

let applyAll = List.fold (fun a b -> App(a, b))

let exprSetter, expr = slot()
let brack = 
    char '(' <*>> anySpace <*>> expr <<*> anySpace <<*> char ')'
let term = 
    var <|> brack
let apply = 
    term <<*> anySpace <*> separated anySpace term |> map (fun (first, rest) -> applyAll first rest) 
    |> map id 
    |> map id |> map id |> map id |> map id |> map id |> map id |> map id
let lamb = char '\\' <*>> letter <<*> char '.' <*> term |> map Lambda
let exprAux = apply <|> term <|> lamb

exprSetter.Set exprAux

module Tests =
    let t1 = test exprAux "\\x.(x (y z))"