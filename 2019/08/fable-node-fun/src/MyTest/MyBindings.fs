module MyBindings

open Node.Api
open Fable.Core

let readDir(path:string) = fs.readdirSync(U2.Case1(path))
let executeSync(cmd:string) = childProcess.execSync(cmd).ToString()
let getProcesses() = executeSync("ps | grep node")
