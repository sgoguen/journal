module Program
// Learn more about F# at http://fsharp.org

open System

open Fable.Mocha
open Gilli

module TestFunctions = 
    let tokenize = stringToList >> tokenize
    let format = tokenize >> parse >> pretty >> listToString
    let parse = tokenize >> parse
    let toStr x = sprintf "%A" x

open TestFunctions

let arithmeticTests = testList "Parsing Tests" [

    let (==>) x y = Expect.equal (toStr x) (toStr y) "should equals"


    testCase "Tokenize" <| fun () ->
        tokenize "λx.x" ==> [Lambda; Variable('x'); Dot; Variable('x')]
        tokenize "λxy.x" ==> [Lambda; Variable('x'); Variable('y'); Dot; Variable('x')]
        tokenize "λxy.x(y)" ==> [Lambda; Variable('x'); Variable('y'); Dot; Variable('x'); LParen; Variable('y'); RParen]
        tokenize "λxy.x  y" ==> [Lambda; Variable('x'); Variable('y'); Dot; Variable('x'); Variable('y')]
        tokenize "λf.λx.f (f x)" ==> [Lambda; Variable('f'); Dot; Lambda; Variable('x'); Dot; Variable('f'); LParen; 
                                      Variable('f'); Variable('x'); RParen]

    testCase "Parsing" <| fun () ->
        parse "λx.x" ==> LambdaT('x', VariableT('x'))
        parse "λxy.xy" ==> LambdaT('x', LambdaT('y', ApplicationT(VariableT('x'), VariableT('y'))))
        // parse "λf.λx.f (f x)" ==> LambdaT('f', LambdaT('x', ApplicationT(VariableT('f'), ApplicationT(VariableT('f'), VariableT('x')))))

    testCase "Format" <| fun () ->
        format "λx.x" ==> "λx.x"
        format "λxy.(x)y" ==> "λx.λy.(x y)"
        format "λxy.xy" ==> "λx.λy.(x y)"
        format "λabc.abc" ==> "λa.λb.λc.((a b) c)"
        format "λf.λx.x" ==> "λf.λx.x"
        // format "λf.λx.f (f x)" ==> "λf.λx.f (f x)"
]



[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    Mocha.runTests arithmeticTests
    0 // return an integer exit code
