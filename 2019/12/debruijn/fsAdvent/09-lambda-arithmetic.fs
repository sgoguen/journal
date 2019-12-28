module Advent.S09

open Advent.AST
open Advent.Formatter
open Advent.NumberEncoder

module P = Microsoft.FSharp.Quotations.Patterns

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

let rec fromExpr = function
    | P.Var(v)                              -> Term.Var(v.Name)
    | P.Lambda(var, LTerm(expr))            -> Term.Lambda(var.Name, expr)
    | P.Let(var, LTerm(expr), LTerm(body))  -> Term.App(Lambda(var.Name, expr), body)
    | P.Application(LTerm(ex1), LTerm(ex2)) -> Term.App(ex1, ex2)
    | QInt(n)                               -> numToLambda n
    | QAdd(LTerm(left), LTerm(right))       -> App(App(PLUS, left), right)
    | x -> failwithf "Don't know how to deal with %A" x

and (|LTerm|) = fromExpr


let rec (|FLambda|) = function
    //  Here we use our new pattern
    | Lambdas(parameters, FLambda(body)) -> sprintf "(λ %s -> %s)" (String.concat " " parameters) body
    | App(FLambda(func), FLambda(arg))    -> sprintf "(%s %s)" func arg
    | Var(name)                         -> name
and toLambda = (|FLambda|)

fromExpr <@  let var1 = 1
             let var2 = 2
             let sum = var1 + var2
             sum
          @> |> toLambda |> printfn "%A"