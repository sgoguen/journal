//  My favorite total function is actually a pair of total
//  functions.  The first function takes any two integers
//  and maps it to a single and *unique* integer for any
//  given integer.

//  The second function takes any integer, and returns a unique
//  pair of integers.

//  These two function complement each other.

//  But they can also be used as building blocks
//  to create functions that can map integers
//  to complicated data types.

let sqrt = float >> System.Math.Sqrt >> int

//  This function will turn any pair of integers into a single
//  integer.
let pair(k1:int, k2:int) = 
    ((k1 + k2) * (k1 + k2 + 1) / 2) + k2

//  This function will turn any integer into a pair of integers.
let unpair(z:int) = 
    let w = ((sqrt(8 * z + 1) - 1) / 2)
    let t = (w * w + w) / 2
    let y = z - t
    let x = w - y
    (x, y)

//  Let's test our functions with the first 100 integers
for i in 0..100 do
    //  We'll first turn an integer into a pair
    let (x,y) = unpair i
    //  Then feed it to the Cantor pairing function
    let i2 = pair(x, y)
    //  Because these functions create a bijection, we'll
    //  get the original integer we started with
    printfn "%A --> %A --> %A" i (x, y) i2









