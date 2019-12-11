#load "Cantor.fs"
#load "Debruijn.fs"

open Debruijn
open Debruijn.Term


let rec count(maxVar, maxApp) = 
    seq {
        for v in 0..maxVar do
            let t = Var(v)
            // if maxApp <= 0 then
            yield t
            if maxApp > 0 then
                for t2 in count(maxVar, (maxApp - 1)) do
                    yield App(t, t2)
                    yield App(t2, t)
    } |> Seq.distinct

for i in 0..4 do 
    let (x, y) = Cantor.unpair i 
    let c = count(i, i) |> Seq.length
    printfn "%i %A -- %i" i (x, y) c

count(5, 5) |> Seq.length

for t in count(1, 0) do printfn "%s" (Term.toString 'a' t)