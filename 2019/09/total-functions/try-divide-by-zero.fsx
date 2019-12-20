type Answer = 
    | Answer of int
    | Undefined

let divide x y = 
    if y <> 0 then Answer(x / y) else Undefined

let z = match (divide 123 4) with
        | Answer(a) -> a * 22
        | Undefined -> raise(exn("Oh noes!"))

//  Tuomas pointed out an issue that I glossed over.  I have
//  a tendency to do that.  These videos are short.

//  In this example (it's not a best practice), I'm showing you
//  that you can circumvent returning a value for both match cases
//  by throwing an exception.  What was the point of creating a total
//  function, if we're just going to call it with code that throws
//  runtime exceptions.  We'll miss out on an important stack trace 
//  too!

//  So, let's modify this a bit and try a few alternatives:

//  First - Let's use our Answer type:

let z1 = match (divide 123 4) with
         | Answer(a) -> Answer(a * 22)
         | Undefined -> Undefined

//  Another option...  We can use the built-in Option type

let z2 = match (divide 123 4) with
         | Answer(a) -> Some(a * 22)
         | Undefined -> None

//  Or we can use the built-in Result type

let z3 = match (divide 123 4) with
         | Answer(a) -> Ok(a * 22)
         | Undefined -> Error("This is undefined")

//  But aren't we just pushing the problem futher up?
//  Where does it stop???