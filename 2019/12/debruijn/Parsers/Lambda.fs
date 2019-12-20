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
    term <<*> anySpace <*> separated anySpace term 
    |> map (fun (first, rest) -> applyAll first rest) 
let lamb = char '\\' <*>> letter <<*> char '.' <*> expr |> map Lambda
let exprAux = lamb <|> apply <|> term

exprSetter.Set exprAux

module Tests =
    let test = test expr
    test "x"
    test "x y"
    test "a b c d e"
    test "a b (c d) e"
    test "\\x.x"
    test "\\x.x y"
    test "\\x.a b c d e"
    test "\\x.a b (c d) e"
    test "\\x.a b (\\c.c d) e"
    test "\\x.(x (y z))"