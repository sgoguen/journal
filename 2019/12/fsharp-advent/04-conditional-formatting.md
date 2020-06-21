# Conditional Formatting

Ok.  Let's tackle this issue of too many parenthesis.  There are a few things contributing to it:

1. Lambda functions don't need parenthesis if they're the outermost thing, but they always need them if they're the function being called or being passed to another function.
2. Both F# and lambda calculus are right associative.  

In other words if I see:
    
    (a b) c

I can rewrite it as:

    a b c

For this, I'm going to add a helper function that will conditionally add parenthesis when I'm applying a function.  You see here that ```toFSharp``` and ```addParens``` mutually recursive functions.  To do this, I use the ```and``` keyword to tell F# that ```addParens``` is part of ```toFSharp```'s definition.

```fsharp
let rec toFSharp = function
    | Var(name)               -> name
    | Lambda(paramName, body) -> sprintf "fun %s -> %s" paramName (toFSharp body)
    | App(func, arg)          -> sprintf "%s %s" (addParens true func) (addParens false arg)


and addParens isLeft = function
    | Var(v1)                 -> v1
    | Lambda(_) as l          -> sprintf "(%s)" (toFSharp l)
    | App(_) as a when isLeft -> sprintf "%s" (toFSharp a)
    | App(_) as a             -> sprintf "(%s)" (toFSharp a)
```

Ok, let's try it out

```fsharp
ONE |> toFSharp |> printfn "%s"  
// Prints: fun f -> fun x -> f x
```

It prints: ```fun f -> fun x -> f x```

Lambda calculus is left-associative, so we shouldn't have parenthesis

```fsharp
App(App(Var("a"), Var("b")), Var("c")) |> toFSharp |> printfn "%s"
//  Prints "a b c"
```

When we group on the right, we should see parenthesis.

```fsharp
App(Var("a"), App(Var("b"), Var("c"))) |> toFSharp |> printfn "%s"
//  Prints "a (b c)"
```

Lambdas are always put in parens when called.

```fsharp
App(App(ZERO, Var("b")), Var("c")) |> toFSharp |> printfn "%s"
//  Prints "(fun f x -> x) b c"
```

Next up, we're going to learn about one of my favorite pattern matching features called **Active Patterns** and we're going to use them to solve how we group our variables.

[Next up - Active Patterns! >>>](05-active-patterns.md)
