type Answer = 
    | Answer of int
    | Undefined

//  While this function is now total
let divide x y = 
    if y <> 0 then Answer(x / y) else Undefined

//  Our call to the function is a mess.  We're
//  back to throwing runtime exceptions... :(
let z = match (divide 123 4) with
        | Answer(a) -> Answer(a * 22)
        | Undefined -> Undefined

//  One option is we can return an Answer again.
//  Another option is to use a built-in type...

let z = match (divide 123 4) with
        | Answer(a) -> Some(a * 22)
        | Undefined -> None

//  Or another built-in type...

let z = match (divide 123 4) with
        | Answer(a) -> Ok(a * 22)
        | Undefined -> Error("This is undefined")

