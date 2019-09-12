type Answer = 
    | Answer of int
    | Undefined

let divide x y = 
    if y <> 0 then Answer(x / y) else Undefined

//  Using this function requires we use pattern matching
//  again.  We can't just do this:

//  Instead, we do this...
let z = match (divide 123 4) with
        | Answer(a) -> a * 22
        | Undefined -> raise(exn("Oh noes!"))
                       //  We either have to return an
                       //  integer or raise an exception