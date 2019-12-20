#load "Cantor.fs"
#load "Debruijn.fs"

open Debruijn
open Debruijn.Term


let rec enumerate(maxVar, maxApp) = 
    seq {
        for v in 0..maxVar do
            let t = Var(v)
            if maxApp <= 0 then
                yield t
            if maxApp > 0 then
                for t2 in enumerate(maxVar, (maxApp - 1)) do
                    yield App(t, t2)
                    yield App(t2, t)
    } |> Seq.distinct

// for i in 0..4 do 
//     let (x, y) = Cantor.unpair i 
//     let c = count(i, i) |> Seq.length
//     printfn "%i %A -- %i" i (x, y) c

// count(1, 1) |> Seq.length

let info t = 
    let str = Term.toString 'a' t
    let d = (t |> Term.normalize |> Term.depth)
    let c = count t
    let p = Cantor.pair(c, d)
    (p, t)


for (i, t) in (enumerate(2, 2) |> Seq.sortBy info |> Seq.mapi (fun i a -> (i, a))) do 
    let str = Term.toString 'a' t
    printfn "%i - %s" i str