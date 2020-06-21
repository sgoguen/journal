# Converting Lambda Calculus into F#

I promised we'd transform our AST into F#.  Here's the AST as a reminder:

```fsharp
type Term = 
    | Lambda of string * Term
    | App of Term * Term
    | Var of string
```

Fortunately for us, a very crude translator function will only take us four lines of code.  It's really about the same size as the AST itself.  Right out of the gate we're going to write ourselves a recursive function that uses pattern maching.  It simply accepts a Term and returns back a string.


```fsharp
let rec toFSharp = function  // Term -> string
    | Var(name)               -> name
    | Lambda(paramName, body) -> sprintf "(fun %s -> %s)" paramName (toFSharp body)
    | App(func, arg)          -> sprintf "(%s %s)" (toFSharp func) (toFSharp arg)
```

Let's break this down.  Here I'm defining a *recursive* pattern matching function 
(note the rec keyword) that will print our lambda functions as F# code.  What I like about
pattern matching is it lets me put examples on the right side and transformations on the 
left side.  It effectively lets me say:

* When I see a Var with a name inside I'm just going to return the name.
* When I see a Lambda, give me the parameter name in the body:
    * I'll format the body and print it in this template to make it look like F#.
* When I see a function call (App), give me the function and it's argument:
    * I'll format both and put them in parenthesis.

Let's try it out our function:

```fsharp
// 1 := λf.λx.f x
let ONE = Lambda("f", Lambda("x", App(Var("f"), Var("x"))))

ONE |> toFSharp |> printfn "%s"  
//  Prints:  (fun f -> (fun x -> (f x)))
```

That's not bad for a first pass, but there are more parenthesis than I would prefer.  Also, we don't need to use the fun keyword so much because we can group the parameters like so:

```fsharp
fun f x -> f (f x)
```

Let's get a little fancier with pattern matching and make our printer pretty.

[Next - More Complicated Pattern Matching >>>](03-pattern-matching.md)
