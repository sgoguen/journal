module Tests

open System
open Xunit


type Term =
    | Var of int
    | Lam of Term
    | App of Term * Term

module Term =

    //  How many lambdas do we need to add to make sure all terms are bound?
    let rec neededLambdas =
        function
        | Var(n) -> (n + 1)
        | Lam(t) -> (neededLambdas t) - 1
        | App(x, y) -> max (neededLambdas x) (neededLambdas y)

    let rec (|Depth|) =
        function
        | Var(n) -> 0
        | Lam(Depth(d)) -> d + 1
        | App(Depth(x), Depth(y)) -> max x y
    and depth (Depth(d)) = d

    //  This function turns a partial term into a lambda, making sure all the variables
    //  are bound to something
    let normalize t =
        let needed = neededLambdas t

        let rec applyRec needed t =
            if needed > 0 then applyRec (needed - 1) (Lam(t))
            else t

        applyRec needed t

    let rec beta =
        function
        | App(Lam(body), t) -> replace 0 t body
        | x -> x

    and replace index newTerm =
        function
        | Var(n) as v ->
            if n = index then newTerm
            else v
        | Lam(t) -> Lam(replace (index + 1) newTerm t)
        | App(x, y) ->
            let newX = replace index newTerm x
            let newY = replace index newTerm y
            App(newX, newY)


    let toString (startChar: char) =
        
        

        let rec render (offset:int) (b:Term) =
            let (|Char|) n = (startChar + char (n)), n
            let (|VarC|_|) = function | Var(Char(c, _)) -> Some(c) | _ -> None
            let (|Rendered|) = function
                | Lam(_) as v -> render (offset - 1) v
                | v           -> render offset v
            let (|Block|_|) = function
                | Lam(_) as v -> Some(render (offset + 1) v)
                | _ -> None            

            match b with
            | Var(Char(c, _))                       -> sprintf "%c" (c + char(offset)), 0
            | Lam(Rendered(x, Char(c, l)))          -> sprintf "λ%c. %s" c x, l + 1
            | App(Rendered(x, lx), VarC(b))         -> sprintf "%s %c" x b, lx
            | App(Block(x, lx), Rendered(y, ly)) -> (sprintf "(%s) (%s)" x y), max lx ly
            | App(Rendered(x, lx), Rendered(y, ly)) -> (sprintf "%s (%s)" x y), max lx ly

        
        //  First normalize, then render
        normalize
        >> (fun t -> let d = depth t in render (d - 1) t)
        >> fst
// let rec (|Info|) = function
//     | Var(n) as t                  -> (n, 0, t)
//     | Lam(Info(n, level, t1)) as t -> (n, level, t)
//     | App(x, y)                    -> (maxLamda t) + 1


// and (|Max|) = function
//     | Var(n) as t -> (n, t)
//     | Lam(Max(n, _)) as t -> (n, t)
//     | App(Max(x, _), Max(y, _)) as t -> ((max x y), t)
// and maxTerm (Max(n)) = n

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


//  (λ λ 4 2 (λ 1 3)) (λ 5 1)
//  (λx. λy. z x (λu. u x)) (λx. w x).

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