module Advent.Formatter

open System
open Advent.AST

(*

Active Patterns do more than allow us to format code differently, they can let us create more
complicated patterns that are based on more complicated tests.

Here we're going to create something called a partial active pattern.  It's called partial 
because you create partial active patterns with partial functions.  When we want to create a
partial function in F#, we always use the option type.

Here's a simpler partial active pattern that parses a string into an integer.  

*)

let tryToParseInt (input:string) = 
    let (ok, value) = Int32.TryParse(input)
    if ok then Some(value) else None

//  We can turn it into an partial active pattern like we did before with our banana clips.
//  BUT!  But active patterns require us to add some additional characters in the name to
//  indicate this is an partial functions.  NOTE the "|_|" in the name.

//  This is *NOT* how to define a partial active pattern, we're missing |_| in the name.
let (|Int|) = tryToParseInt

//  This is how to define a partial active pattern.
//  Also, all partial active patterns require your function returns an option type.
let (|Int|_|) = tryToParseInt

(*

Back to our printer.  I want to detect nested lambda functions.  Here's an example:

    S = λx.λy.λz.((xz)(yz))

When I come across nested lambdas like this, I want to extract:

    1. The list of all the parameters:    ["x"; "y"; "z"]
    2. The body of the inner most lambda: (x y)(y z) 

Here's how we do it with a recursive partial active pattern:

*)

let rec (|Lambdas|_|) = function
    //  Is it a sequences of lambdas?
    | Lambda(p1, Lambdas(parameterNames, body)) -> Some(p1::parameterNames, body)
    //  Is this the inner-most lambda?
    | Lambda(p1, body)                          -> Some([p1], body)
    //  If the term wasn't even a lambda, let's not match.
    | term                                      -> None


(*

Let's break this function down:

    1.  The first test checks to see if our term is a lambda.  Not only that,
        it also checks if the body is made up of a sequences of lambdas (One or more)
        When it does, we're going to return a list of strings for all of the parameter
        names and the body of the inner most lambda.
    2.  The first test only matches when there are two or more lambdas.  We need this 
        test to match our base case (the inner most lambda).

*)

//  Let's try it out:
let rec (|FSharp|) = function
    //  Here we use our new pattern
    | Lambdas(parameters, FSharp(body)) -> sprintf "(fun %s -> %s)" (String.concat " " parameters) body
    | App(FSharp(func), FSharp(arg))    -> sprintf "(%s %s)" func arg
    | Var(name)                         -> name



let toFSharp = (|FSharp|)

Lambda("a", Lambda("b", Lambda("c", Var("c")))) |> toFSharp |> printfn "%s"