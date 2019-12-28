# Encoding Arithmetic

Let's end our encoding experiment by converting some basic F# arithmetic into pure lambda calculus.  My goal is to convert this F# code:

```fsharp
let var1 = 1
let var2 = 2
let sum = var1 + var2
sum
```

Into this lambda equivalent:

```
((λ.var1 -> ((λ.var2 -> ((λ.sum -> sum) (((λ.m n f x -> ((m f) ((n f) x))) var1) var2))) (λ.f x -> (f (f x))))) (λ.f x -> (f x)))
```

Believe it or not, we have nearly everything we need.

## Lambda Addition

First, we need a pure lambda calculus definition of addition:

```fsharp
λm.λn.λf.λx.m f (n f x)
```

That would look like this in our AST: 

```fsharp
let PLUS = Lambda("m", 
             Lambda("n", 
               Lambda("f",
                 Lambda("x", 
                   App(
                     App(Var("m"), Var("f")),
                     App(App(Var("n"), Var("f")), Var("x"))
                   )
                )
              )
            )
           )
```

Next, we need to update our function that converts F# code into lambda code to include a few cases:

1. If we see an integer, convert it to a lambda using church encoding.
2. If we see an addition operation, use our lambda definition of PLUS.
3. We're going to turn all let assignment into lambdas so  when we see `let x = 5`, it will translate into the lambda equivalent of `(λx.x)(5)`

## Some Helper Patterns

Before I finish this up, I'm going to create a few helper patterns to make is easier to read the F# AST.

My first AP is something I'll call `QInt`, which is short for QuotationInt.  It looks at an F# expression and returns the integer value if it's an integer literal.

```fsharp
let (|QInt|_|) = function
  | P.Value(v, t) when t = typeof<int> -> Some(v :?> int)
  | _ -> None
```

My next AP is `QAdd`.  It helps us detect if an expression is an addition operation.  If it is, it returns the left and right expressions of that addition. 

```fsharp
let (|QAdd|_|) = function
    | P.Call(None, f, [x; y]) when f.Name = "op_Addition" -> Some(x, y)
    | _ -> None
```

## Wrapping It

Putting it all together, we're left with a 12 line function that I think does a pretty good job showing how one can begin to transpile F# code into pure lambda calculus. 

```fsharp
let rec (|LTerm|) = function
    //  Carried over
    | P.Var(v)                              -> Var(v.Name)
    | P.Lambda(var, LTerm(expr))            -> Lambda(var.Name, expr)
    | P.Application(LTerm(ex1), LTerm(ex2)) -> App(ex1, ex2)
    //  1. New AP (active pattern) to detect integers
    | QInt(n)                               -> numToLambda n
    //  2. An AP to detect addition
    | QAdd(LTerm(left), LTerm(right))       -> App(App(PLUS, left), right)
    //  3. Let assignments become lambdas
    | P.Let(var, LTerm(expr), LTerm(body))  -> App(Lambda(var.Name, body), expr)
    | x -> failwithf "Don't know how to deal with %A" x
```


##  Thank you and Happy New Year!

Again, I want to thank Sergey Tihon for organizing this and to the F# community with a special nod at the people who work tirelessly on tooling.

Hope to see you at [Capitol F#](https://www.capitolfsharp.org/)!



