module MyTest
// Add this line above

open System

[<EntryPoint>]
let main argv =
    for i in 1..10 do
        printfn "Hello World %i from F#!!" i
    0 // return an integer exit code
