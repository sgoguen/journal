# Encoding Arithematic

Let's update our fromExpr function so we can encode integers and addition as pure lambda functions.

What I want to do is to define a couple of variables, do some basic arthematic and encode it in pure
lambda calculus.

Here's my test below:


```fsharp
[<Fact>]
let ``Lambda Formatting``() =
    //  Let's convert everything inside our quotation  ( <@ @> )
    let lambdaCode = fsharpToLambda <@  let var1 = 1
                                        let var2 = 2
                                        let sum = var1 + var2
                                        sum
                                     @>
    //  This is what it should turn it.
    let expectedOutput = "((λ.var1 -> ((λ.var2 -> ((λ.sum -> sum) (((λ.m n f x -> ((m f) ((n f) x))) var1) var2))) (λ.f x -> (f (f x))))) (λ.f x -> (f x)))"
    Assert.StrictEqual(expectedOutput, lambdaCode)
```

Ok.  First thing's first, we need to update our function that converts F# into lambda calculus.

```fsharp
let rec (|LTerm|) = function
    //  We'll carry these rules over from our previous module
    | P.Var(v)                              -> Var(v.Name)
    | P.Lambda(var, LTerm(expr))            -> Lambda(var.Name, expr)
    | P.Application(LTerm(ex1), LTerm(ex2)) -> App(ex1, ex2)
    //  Next, when we come across an integer, let's use our Church encoder to turn it into a lambda function.
    | QInt(n)                               -> numToLambda n
    //  When we come across an addition operation, let's use a lambda definition of addition (PLUS)
    | QAdd(LTerm(left), LTerm(right))       -> App(App(PLUS, left), right)
    //  For let statements, 
    | P.Let(var, LTerm(expr), LTerm(body))  -> App(Lambda(var.Name, body), expr)
    | x -> failwithf "Don't know how to deal with %A" x

// In order to make the above function work, we need a few helpers:

let fromExpr = (|LTerm|)

let fsharpToLambda x = x |> fromExpr |> toLambda

//  We're going to need to extract integer literals.
let (|QInt|_|) = function
    | P.Value(v, t) when t = typeof<int> -> Some(v :?> int)
    | _ -> None

let (|QAdd|_|) = function
    | P.Call(None, f, [x; y]) when f.Name = "op_Addition" -> Some(x, y)
    | _ -> None

//  λm.λn.λf.λx.m f (n f x)
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



let rec (|FLambda|) = function
    | Lambdas(parameters, FLambda(body)) -> sprintf "(λ.%s -> %s)" (String.concat " " parameters) body
    | App(FLambda(func), FLambda(arg))    -> sprintf "(%s %s)" func arg
    | Var(name)                         -> name
and toLambda = (|FLambda|)
```
