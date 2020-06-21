module Tests

open System
open Xunit
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Cantor
open Debruijn

module Basics = 
    let PAIR = fun a b -> fun p -> p a b ;
    let FIRST = fun p -> p (fun x y -> x)
    let SECOND = fun p -> p (fun x y -> y)

    // -- The constant function

    let K = fun x y -> x ;

    let TRUE = fun x y -> x
    let FALSE = fun x y -> y
    
    let IF = fun x -> x
    let AND = fun x y -> IF x y FALSE
    let OR = fun x y -> IF x TRUE y
    let NOT = fun x -> IF x FALSE TRUE

    // let FIX = fun f -> (fun x -> f (x x)) (fun x -> f (x x))
    let NIL = fun x f -> x
    let CONS = fun g r -> fun x f -> f g r

    // let HEAD = fun l -> l error (fun a b -> a)
    // let TAIL = fun l -> l error (fun a b -> b)

    let MATCH = fun l x f -> l x f

// let MAP = fix (^map f l . match l nil (^x xs. cons (f x) (map f xs))) ;

// let FOLD = fix (^fold x f l. match l x (^y ys . f y (fold x f ys))) ;    




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
    let (==>) x y = Assert.StrictEqual(x, y)
    let toStr = Term.toString 'a'

    // check (Term.toString 'a') [
    toStr Examples.id ==> "λa. a"
    toStr <| Examples.k ==> "λc. λb. λa. c"
    toStr <| Examples.s ==> "λc. λb. λa. c a (b a)"
    toStr <| Lam(Lam(Var(0))) ==> "λb. λa. a"
    toStr <| Lam(Lam(Var(1))) ==> "λb. λa. b"
    toStr <| Lam(App(Var(0), Lam(App(Var(0),Var(1))))) ==> "λb. b (λa. a b)"
    toStr <| App(Var(0), Var(1)) ==> "λb. λa. a b"
    toStr <| App(Var(2), App(Var(0), Var(1))) ==> "λc. λb. λa. c (a b)"
    toStr <| App(Var(2), App(Var(0), Lam(Var(1)))) ==> "λd. λc. λb. d (b (λa. b))"
    toStr <| Lam(App(Var(0), Lam(Var(0)))) ==> "λb. b (λa. a)"

    // toStr <| Lam(App(Var(2), Var(0))) ==> "λc. λb. (λa. c a)"
    // toStr <| App(Lam(App(Var(0), Lam(Var(0)))), Lam(App(Var(2), Var(0)))) ==> "λc. (λb. b (λa. a)) (λa. c a)"
    // toStr <| Lam(App(Examples.s, Examples.k)) ==> "λd. λc. λb. λa. c a (b a) (λb. λa. c)"
        // ]

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