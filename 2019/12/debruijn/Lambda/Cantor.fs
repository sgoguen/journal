module Cantor

let sqrt = float >> System.Math.Sqrt >> int

let pair(k1:int, k2:int) = 
    ((k1 + k2) * (k1 + k2 + 1) / 2) + k2

let unpair(z:int) = 
    let w = ((sqrt(8 * z + 1) - 1) / 2)
    let t = (w * w + w) / 2
    let y = z - t
    let x = w - y
    (x, y)