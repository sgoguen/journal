# Partial Active Patterns

Partial active patterns are active patterns created with partial functions.  In other words, these are functions that are underdefine for some input.  Let me give you can example.

Here's a function that tries to parse an integer:

```fsharp
let tryToParseInt (input:string) = 
    let (ok, value) = Int32.TryParse(input)
    if ok then Some(value) else None
```

I say tries because sometimes you can't.  If I feed this function 'aabbcc', it will return `None`.  Likewise, when I feed it "123", `tryToParseInt` will return `Some(123)`

These types of functions are the building blocks for partial active patterns.

Like before, we can turn it into an partial active pattern like we did before with our banana clips.

**HOWEVER**!  To make it a partial active patterns, we have to add some additional characters to the end of the name to tell the F# compile it's partial.

```fsharp
let (|Int|_|) = tryToParseInt
```

Notice the |_|.  This is important.

You don't want to forget it an accidentally write it like so:
```fsharp
let (|Int|) = tryToParseInt
```

## Detecting Nested Lambdas

Back to our printer.  I want to detect nested lambda functions.  Here's an example:

    S = λx.λy.λz.((xz)(yz))

When I come across nested lambdas like this, I want to extract the following information:

1. The list of all the parameters:    ["x"; "y"; "z"]
    * Because all we're going to do is combine it into one string.
2. The body of the inner most lambda: (x y)(y z) 

Here's how we do it with a recursive partial active pattern:


```fsharp
let rec (|Lambdas|_|) = function
    //  Is it a sequences of lambdas?
    | Lambda(p1, Lambdas(parameterNames, body)) -> Some(p1::parameterNames, body)
    //  Is this the inner-most lambda?
    | Lambda(p1, body)                          -> Some([p1], body)
    //  If the term wasn't even a lambda, let's not match.
    | term                                      -> None
```

Let's break this function down.  We do three checks:

1.  Right off the bat, we try to see if we have a lambda with nested lambas.
    * This means we're recursing right away.
    * If the body is one or more lambdas, it gives us the list of parameters and the inner-most lambda body that is not a lambda function.
    * On the right side of the arrow, we return a new list of parameters with the outermost parameter `p1` at the head of the list.  We also return the inner most body.
2.  We need the second check to catch our first lambas.
    * If we've gotten here, it's because its body is not a lambda function.
    * Knowing that, we simply return a list with our single parameter name and body.
3.  Finally, we need to know what to do if the term isn't even a lambda.
    * If we're looking at an `App` or `Var` we need to bail.
    * We indicate we didn't match by returning `None`.


## Trying it out!

Let's take our fancy new active pattern for a spin with our formatter.

```fsharp
let rec (|FSharp|) = function
    //  Here we use our new pattern
    | Lambdas(parameters, FSharp(body)) -> sprintf "fun %s -> %s" (String.concat " " parameters) body
    | App(FSharp(func), FSharp(arg))    -> sprintf "(%s %s)" func arg
    | Var(name)                         -> name
```

Let's add this, so we can call it like a regular function.
```fsharp
let toFSharp = (|FSharp|)
```

And give it a test

```fsharp
let ex1 = Lambda("a", Lambda("b", Lambda("c", Var("c"))))

ex |> toFSharp |> printfn "%s"
```

Our new formatted should now print.

```fsharp
fun a b c -> c
```

As slick as that is, we're going to embark on something even slicker.  We're going to use F# and active patterns to turn F# code into lambda calculus.

You're not going to want to miss this holiday magic.

[Up Next - Quotations!!! >>>](07-quotations.md)
