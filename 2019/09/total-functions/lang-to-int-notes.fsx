module Cantor =

    let sqrt = float >> System.Math.Sqrt >> int

    let pair(k1:int, k2:int) = 
        ((k1 + k2) * (k1 + k2 + 1) / 2) + k2

    let unpair(z:int) = 
        let w = ((sqrt(8 * z + 1) - 1) / 2)
        let t = (w * w + w) / 2
        let y = z - t
        let x = w - y
        (x, y)

///  A simple calculator AST
type Calc = 
    | Value of int        //  We'll have integers
    | Add of Calc * Calc  // Addition
    | Mul of Calc * Calc  // Multiplication

///  Turns a positive integer into a unique instance of our calculator AST
let rec fromInt n = 
    let r = n % 3  
    let n = n / 3
    let (x, y) = Cantor.unpair(n)
    match r with
    | 0 -> Value(n)
    | 1 -> Add((fromInt x), (fromInt y))
    | 2 -> Mul((fromInt x), (fromInt y))

///  Turns out calculator AST into a string
let rec toString = function
    | Value(n) -> string(n)
    | Add(l, r) -> sprintf "(%s + %s)" (toString l) (toString r)
    | Mul(l, r) -> sprintf "(%s * %s)" (toString l) (toString r)

///  Convert a Calc expression into a unique integer
let rec toInt = function
    | Value(n) -> n * 3
    | Add(l, r) -> let (x, y) = (toInt l), (toInt r)
                   let n = Cantor.pair(x, y)
                   (n * 3) + 1
    | Mul(l, r) -> let (x, y) = (toInt l), (toInt r)
                   let n = Cantor.pair(x, y)
                   (n * 3) + 2


//  Let's demonstrate our fromInt and toInt functions are bijective by
//  enumerating the first million expressions and converting them back 
//  to integers.
for i in 0..1000000 do
    //  Let's turn an integer into an expression
    let c = fromInt i
    //  Then let's turn that expression back into an integer
    let i2 = toInt c
    if i <> i2 then
        //  Show any expression that doesn't make the round trip
        printfn "Exception:  %i - %i - %s" i i2 (toString c)

let t = struct (1, "Hi")