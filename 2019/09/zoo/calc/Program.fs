// module MainProg

// Learn more about F# at http://fsharp.org

open System
open Calc.Lexer

[<EntryPoint>]
let main argv =
    Console.Clear()
    // let input = "23 + 7 -8 * (4 + 8)"
    let input = "23 + 4 * 4 + 3"
    printfn "Input: %s" input

    let tokens = tokenize <| Seq.toList input
    printfn "Tokens: %A" tokens

    match tokens with
    | Expr(e) -> printfn "Good: %A" (e)
    | ExprErr(err)  -> printfn "Error: %A" err

    // Calc.Lexer.tokenize input |> printfn "%A"
    
    // printfn "Hello Calc!"
    0 // return an integer exit code
