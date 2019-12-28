# Conditional Formatting

```fsharp
let rec toFSharp = function
    | Var(name)                    -> name
    //  When we see two lambdas, let's group them...  (We'll fix this in a bit)
    | Lambda(p1, Lambda(p2, body)) -> sprintf "fun %s %s -> %s" p1 p2 (toFSharp body)
    | Lambda(paramName, body)      -> sprintf "fun %s -> %s" paramName (toFSharp body)
    | App(func, arg)               -> sprintf "%s %s" (addParens true func)  (addParens false arg)


and addParens isLeft = function
    | Var(v1)                 -> v1
    | Lambda(_) as l          -> sprintf "(%s)" (toFSharp l)
    | App(_) as a when isLeft -> sprintf "%s" (toFSharp a)
    | App(_) as a             -> sprintf "(%s)" (toFSharp a)


//  Ok, let's try it out

ONE |> toFSharp |> printfn "%s"  
//  Perfect, it still prints "(fun f x -> f x)"

//  Lambda calculus is left-associative, so we shouldn't have parenthesis
App(App(Var("a"), Var("b")), Var("c")) |> toFSharp |> printfn "%s"
//  Prints "a b c"

//  Lambda calculus is left-associative, so we shouldn't have parenthesis
App(Var("a"), App(Var("b"), Var("c"))) |> toFSharp |> printfn "%s"
//  Prints "a (b c)"

App(App(ZERO, Var("b")), Var("c")) |> toFSharp |> printfn "%s"
//  Prints "(fun f x -> x) b c"

App(App(ZERO, Var("b")), ZERO) |> toFSharp |> printfn "%s"
```

[Next - Active Patterns >>>](05-active-patterns.md)
