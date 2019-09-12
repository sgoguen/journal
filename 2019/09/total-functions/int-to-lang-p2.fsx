#load "cantor.fs"

type Calc = 
    | Value of int        //  We'll have integers
    | Add of Calc * Calc  // Addition
    | Mul of Calc * Calc  // Multiplication
//  Do you notice a pattern in my examples?
//  - Every 3 items cycle between Value, Add and Mul

//  We're going to use recursion here
let rec fromInt n= 
    let r = n % 3  //  Let's use the modulo operator to cycle
    let n = n / 3
    //  And I'm also going to use our unpairing function
    let (x, y) = Cantor.unpair(n)
    match r with
    | 0 -> Value(n)
    | 1 -> Add((fromInt x), (fromInt y))
    | 2 -> Mul((fromInt x), (fromInt y))

//  Let's write a failing test...
for i in 10000..10012 do
    printfn "%i - %A" i (fromInt i)