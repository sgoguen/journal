#load "cantor.fs"

type Calc = 
    | Value of int        //  We'll have integers
    | Add of Calc * Calc  // Addition
    | Mul of Calc * Calc  // Multiplication


//  Notice any patterns here?
//   - We're cycling between our three options
//   - So let's take the modulo 3 of our input and
//     use that to determine which tag we're going to use.

let rec fromInt n = 
    let r = n % 3
    let n = n / 3
    let (x, y) = Cantor.unpair(n)
    match r with
    | 0 -> Value(n) 
    | 1 -> Add((fromInt x), (fromInt y))
    | 2 -> Mul((fromInt x), (fromInt y))
    | _ -> raise(exn("This should be impossible!"))

let rec toString = function
    | Value(n)  -> string(n)
    | Add(x, y) -> sprintf "(%s + %s)" (toString x) (toString y)
    | Mul(x, y) -> sprintf "(%s * %s)" (toString x) (toString y)

for i in 0..100 do
    let c = fromInt i |> toString
    printfn "%i - %s" i c

fromInt 4028002 |> toString


