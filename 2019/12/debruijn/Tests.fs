module Tests

open System
open Xunit
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Cantor
open Debruijn


module Examples =
    // λx. x
    let id = Lam(Var(0))
    // λx. λy. x
    let k = Lam(Lam(Var(2)))
    // λx. λy. λz. x z (y z)
    let s = Lam(Lam(Lam(App(App(Var(2), Var(0)), App(Var(1), Var(0))))))

let check (fn: 'a -> 'b) (inputs: List<'a * 'b>) =
    for input, expected in inputs do
        let r = fn input
        Assert.StrictEqual(expected, r)

let (==>) x y = (x, y)




[<Fact>]
let ``Lambda Formatting``() =
    check (Term.toString 'a')
        [ Examples.id ==> "λa. a"
          Examples.k ==> "λc. λb. λa. c"
          Examples.s ==> "λc. λb. λa. c a (b a)"
          Lam(Lam(Var(0))) ==> "λb. λa. a"
          Lam(Lam(Var(1))) ==> "λb. λa. b"
          Lam(App(Var(0), Lam(App(Var(0),Var(1))))) ==> "λb. b (λa. a b)"
          App(Var(0), Var(1)) ==> "λb. λa. a b"
          App(Var(2), App(Var(0), Var(1))) ==> "λc. λb. λa. c (a b)"
          App(Var(2), App(Var(0), Lam(Var(1)))) ==> "λd. λc. λb. d (b (λa. b))"
          Lam(App(Var(0), Lam(Var(0)))) ==> "λb. b (λa. a)"

          Lam(App(Var(2), Var(0))) ==> "λc. λb. (λa. c a)"
          App(Lam(App(Var(0), Lam(Var(0)))), Lam(App(Var(2), Var(0)))) ==> "λc. (λb. b (λa. a)) (λa. c a)"
          Lam(App(Examples.s, Examples.k)) ==> "λd. λc. λb. λa. c a (b a) (λb. λa. c)"
        ]

[<Fact>]
let ``Test neededLambdas``() =
    check (Term.neededLambdas)
        [ Var(0) ==> 1
          Lam(Var(0)) ==> 0
          Var(1) ==> 2
          Lam(Var(1)) ==> 1
          App(Var(0), Var(0)) ==> 1
          App(Var(0), Var(1)) ==> 2
          // Lam(Lam(Var(1))) ==> Lam(Lam(Var(1)))
         ]

[<Fact>]
let ``Test normalize``() =
    check (Term.normalize)
        [ Var(0) ==> Lam(Var(0))
          Lam(Var(0)) ==> Lam(Var(0))
          Var(1) ==> Lam(Lam(Var(1)))
          Lam(Var(1)) ==> Lam(Lam(Var(1)))
          Lam(Lam(Var(1))) ==> Lam(Lam(Var(1)))
          App(Lam(Var(0)), Var(1)) ==> Lam(Lam(App(Lam(Var(0)), Var(1))))
          App(Lam(Var(1)), Var(1)) ==> Lam(Lam(App(Lam(Var(1)), Var(1))))
          App(Lam(Lam(Var(0))), Var(1)) ==> Lam(Lam(App(Lam(Lam(Var(0))), Var(1)))) ]


[<Fact>]
let ``Test beta``() =
    check (Term.beta)
        [ App(Lam(Var(0)), Lam(Lam(Var(1)))) ==> Lam(Lam(Var(1)))
          App(Lam(App(Var(0), Var(0))), Lam(Lam(Var(1)))) ==> App(Lam(Lam(Var(1))), Lam(Lam(Var(1)))) ]