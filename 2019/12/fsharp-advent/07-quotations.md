# F# -> Lambas using F# Quotations

Like I said, we're going to use quotations to 
turn F# code into pure lambda calculus.  So what are quotations?

Quotations allow us to 'quote' your code so you can turn code into data.  You're basically telling the F# compiler that you want it to parse the code, but you don't want it to compile it.  You just want the data describing the code so you can play with it. 

Let me show you.  Here I'm wrapping a lambda function in a quotation block: <@  @>



```fsharp
let add = <@ fun x y -> x + y @>
```

Let's print it!

```fsharp
printfn "add: %A" add
//  PRINTS:  add: Lambda (x, Lambda (y, Call (None, op_Addition, [x, y])))
```

The AST that's printed above, more or less, shows us how F# encodes a function before the compiler turns it into IL code.  In other words, we can see what F#'s parser sees and transform it into what we want.  FYI, the Fable compiler uses a form of these expressions to turn F# into JavaScript.  

Let's try turning F# into lambda calculus!

First, let's create a shortcut to the module that has all the active patterns 
we're going to use to transform our quotations.

```fsharp

module P = Microsoft.FSharp.Quotations.Patterns
```

Next, let's write a small function to get us started by mapping the basic elements:

```fsharp
let rec fromExpr = function
    | P.Var(v)                              -> Term.Var(v.Name)
    | P.Lambda(var, LTerm(expr))            -> Term.Lambda(var.Name, expr)
    | P.Application(LTerm(ex1), LTerm(ex2)) -> Term.App(ex1, ex2)
    | x -> failwithf "Don't know how to deal with %A" x
and (|LTerm|) = fromExpr
```

Here the mapping was pretty trivial.  We mapped F#'s variables, lambdas and function application
to our simple lambda calculus.  For anything else, we throw an error.

```fsharp
//  Let's try it out:
let ID = fromExpr <@ fun x f -> f x @>

printfn "%A" ID
//  Lambda("x", Lambda("f", App(Var("f"), Var("x"))))
```

That's pretty slick.

Up next!  We're going to start translating simple F# arithemetic into pure lambda calculus using Church Encoding.

[Up Next - Church Encoding!!!  >>>](08-church-encoding-integers.md)
