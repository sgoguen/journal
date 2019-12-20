module Debruijn

type Term =
    | Var of int
    | Lam of Term
    | App of Term * Term

module Term =

    let rec fromInt n = 
        let r = n % 3  
        let n = n / 3
        let (x, y) = Cantor.unpair(n)
        match r with
        | 0 -> Var(n)
        | 1 -> Lam((fromInt n))
        | 2 -> App((fromInt x), (fromInt y))
        | _ -> failwith "This shouldn't happen"


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

    let rec (|Count|) = function
        | Var(n) -> 1
        | Lam(Count(b)) -> b 
        | App(Count(x), Count(y)) -> x + y
    and count (Count(c)) = c

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