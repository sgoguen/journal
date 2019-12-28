## Pattern Matching 

To recap, our formatting function was a little heavy handed with the parenthesis:

When we asked out function to print this guy:
```fsharp
// 1 := λf.λx.f x
let ONE = Lambda("f", Lambda("x", App(Var("f"), Var("x"))))

ONE |> toFSharp |> printfn "%s"  
```

We got this:

```fsharp
(fun f -> (fun x -> (f x)))
```

Instead of this:

```fsharp
fun f x -> f x
```

If there's one thing pattern matching is good at, it's handling very specific scenarios.  I'm going to modify our ```toFSharp``` function too handle this *exact* scenario.


```fsharp
let rec toFSharp = function
    //  When we see this pattern
    | Lambda(p1, Lambda(p2, App(Var(v1), Var(v2)))) 
                              // Return this
                              -> sprintf "fun %s %s -> %s %s" p1 p2 v1 v2
    //  Do we what did before
    | Var(name)               -> name
    | Lambda(paramName, body) -> sprintf "(fun %s -> %s)" paramName (toFSharp body)
    | App(func, arg)          -> sprintf "(%s %s)" (toFSharp func) (toFSharp arg)


//  Let's try it!
ONE |> toFSharp |> printfn "%s"   
```

Needless to say, it prints exactly what we want.

```fsharp
fun f x -> f x
```

While this new rule is clearly too specific to this scenario, it highlights the power that pattern matching.  We can decode complicated instances by deconstructing it into simpler parts.  

Imagine how much code you'd have to write in another language to do this check:

* If the outer object is a lambda functions
* And it's body is a lambda function
* And that lambda function's body is a function application 
* And the left side of that call is a variable.
* And the right side of that call is a variable.

It sounds so exhausting.

With pattern matching, we write our tests using the same syntax we use to construct our instances.   When we get a match, our instance's properties are automatically extracted and bound to variables that we can use on the right side of the -> arrow.

How is that not exciting?

As neat as this is, this pattern is too specific.  So let's generalize it!  
After all, what happens when we have three lambdas or 4 lambdas?  Also, maybe we want 
to separate the logic that groups lambda parameters from the logic that determines
whether we need parenthesis.

Ok, let's handle formatting our parenthesis in the next section.

[Next - Conditional Formatting >>>](04-conditional-formatting.md)
