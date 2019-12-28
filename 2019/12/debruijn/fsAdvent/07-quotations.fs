module Advent.S07

open Advent.AST

(*

Let's switch gears a little bit, because I want to show you how we can use quotations to 
turn F# code into pure lambda calculus.  Quotations allow us to access the AST of F# code.

Let me show you:

*)

//  Here I'm wrapping a lambda function in a quotation block: <@  @>
let add = <@ fun x y -> x + y @>

//  Let's print it:
printfn "add: %A" add
//  PRINTS:  add: Lambda (x, Lambda (y, Call (None, op_Addition, [x, y])))

(*

The AST that's printed above, more or less, shows us how F# encodes a function 
before the compiler turns it into IL code.  In other words, we can see what F#'s 
parser sees and transform it into what we want.  The Fable compiler uses a form of
these expressions to turn F# into JavaScript.  

Let's try turning F# into lambda calculus!

*)

//  First, let's create a shortcut to the module that has all the active patterns 
//  we're going to use to transform our quotations.

module P = Microsoft.FSharp.Quotations.Patterns

//  Next, let's write a small function to get us started by mapping the basic elements:
let rec fromExpr = function
    | P.Var(v)                              -> Term.Var(v.Name)
    | P.Lambda(var, LTerm(expr))            -> Term.Lambda(var.Name, expr)
    | P.Application(LTerm(ex1), LTerm(ex2)) -> Term.App(ex1, ex2)
    | x -> failwithf "Don't know how to deal with %A" x
and (|LTerm|) = fromExpr

(*

Here the mapping was pretty trivial.  We mapped F#'s variables, lambdas and function application
to our simple lambda calculus.  For anything else, we throw an error.

*)

//  Let's try it out:
let ID = fromExpr <@ fun x f -> f x @>

printfn "%A" ID
// Lambda("x", Lambda("f", App( Var("f"), Var("x"))))