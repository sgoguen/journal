module Advent.S08

open Advent.AST
open Advent.S04


(*

Let's make it a little more interesting.

From the Wikipedia page on Lambda Calculus, they show us how we can encode other things into
lambda calculus.  For example, here's how we can encode numbers:

0 := λf.λx.x
1 := λf.λx.f x
2 := λf.λx.f (f x)
3 := λf.λx.f (f (f x))

The pattern seems clear enough.  If we want to encode a positive integer, we'll wrap our body
of our lambda function with another call to f.  Let's write a function that does this for us:

*)

let rec numToLambda n = Lambda("f", Lambda("x", (numBodyToLamda n))) 
and numBodyToLamda = function
    | 0 -> Var("x")
    | n -> App(Var("f"), (numBodyToLamda (n - 1)))

numToLambda 4 |> toFSharp |> printfn "%s"
// PRINTS: (fun f x -> (f (f (f (f x)))))