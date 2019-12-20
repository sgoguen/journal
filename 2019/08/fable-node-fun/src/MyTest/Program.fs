module MyTest

open Fable.Core.JS
open MyBindings

[<EntryPoint>]
let main argv =
    
    console.clear()
    console.log("Args", argv)
    // console.log("Hello world", readDir("."))
    // console.log("My processes", getProcesses())

    0 // return an integer exit code
