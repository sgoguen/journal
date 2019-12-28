# Printing Our AST

Let's start with a pattern matching function that turns my AST into F# code: 


```fsharp
let rec toFSharp = function
    | Var(name)               -> name
    | Lambda(paramName, body) -> sprintf "(fun %s -> %s)" paramName (toFSharp body)
    | App(func, arg)          -> sprintf "(%s %s)" (toFSharp func) (toFSharp arg)
```

Let's break this down.  Here I'm defining a *recursive* pattern matching function 
(note the rec keyword) that will print our lambda functions as F# code.  What I like about
pattern matching is it lets me put examples on the right side and transformations on the 
left side.  It effectively lets me say:

* When I see a Var with a name inside, I'm just going to return the name.
* When I see a Lambda, give me the parameter name in the body:
    * I'll format the body and print it in this template to make it look like F#.
* When I see a function call (App), give me the function and it's argument:
    * I'll format both and put them in parenthesis.

Let's try it out:

```fsharp
ONE |> toFSharp |> printfn "%s"  
//  Prints:  (fun f -> (fun x -> (f x)))
```

[Next - Pattern Matching >>>](03-pattern-matching.md)
