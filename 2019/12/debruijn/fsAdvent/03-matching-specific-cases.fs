module Advent.S03

open Advent.AST

(*

That's not bad, but there are a lot of parenthesis and if I was writing this in F#,
I'd write it a little cleaner like so:

    fun f x -> f (f x)

Let's see if pattern matching can help us here.  I'm going to create a rule that will 
match this exact scenario and format it the way I want, then we'll go from there.

*)

let rec toFSharp = function
    //  When we see this pattern
    | Lambda(p1, Lambda(p2, App(Var(v1), Var(v2)))) 
                              // Return this
                              -> sprintf "fun %s %s -> %s %s" p1 p2 v1 v2
    | Var(name)               -> name
    | Lambda(paramName, body) -> sprintf "(fun %s -> %s)" paramName (toFSharp body)
    | App(func, arg)          -> sprintf "(%s %s)" (toFSharp func) (toFSharp arg)


//  Let's try it!
ONE |> toFSharp |> printfn "%s"   //  It now prints "fun f x -> f x"

(*

What makes pattern matching powerful is it allows us decode complicated instances by 
deconstructing complicated instances into its simpler parts.  Here we're testing to see if
the instance matches a relatively complicated shape.  When it matches, the properties are
automated extracted and bound to variables that we can use on the right side of the -> arrow.
In this case, that means we're getting the names of the parameters and the names of the variables.

As neat as this is, this pattern is too specific so we should generalize it a bit.  
After all, what happens when we have three lambdas or 4 lambdas?  Also, maybe we want 
to separate the logic that groups lambda parameters from the logic that determines
whether we need parenthesis.

Let's break out the application rule first and take a first stab at dealing with the
parenthesis.

*)