
///////////////////////////////////////////////////////////////////////////

//  Total functions are an important part of functional programming.
//  You can't *always* make everything a total function, but F# really
//  tries to give you a lot of tools and good defaults.

//  First off.  F# doesn't have if statements (This is partially true), it has 
//  conditional **expressions**.  Expressions always return values, where 
//  statements do not.  

let canDrinkInTheUS age = 
    let message = if age >= 21 then
                    "Yes!"  //  Here we 
                  else
                    "Go away"
    message




let boolAnd x y =  
    match x, y with
    | true, true -> true
    | _, false -> false
    | false, true -> false

//  Here we're defining the boolean and operation by example using the
//  a match **expression** (not statement).  I say expression, because expressions
//  are requires to return a value for each condition.  Not only if this true for the
//  match expression, but for if expressions (They're not if statements)
//
//  Right off the bat, the F# compiler is telling me what scenarios I'm
//  not matching against.  Specifically, it's saying (_, false) isn't handled.
//  This means, I have no solution when y is false.  The _ means we don't care
//  about x.
//  When I fix that scenario, it tells me (false, true) isn't handled.
//  After fixing that, the F# compiler tells me I've covered all my scenarios.

///////////////////////////////////////////////////////////////////////////

//  A partial function is a function that does not map every possible input to 
//  an output.  Here's a subtle example of a partial function.

let divide x y = x / y  
//  This is a partial function because we know we can't divide x by 0, because it's
//  undefined and passing 0 to the y parameter will throw a runtime exception


let tryDivide x y = if y = 0 then None else Some(x / y)
//  But we can turn this partial function into a total function
//  by wrapping it in a Option type.  Ok...Ok.. We're not returning a number,
//  but we are returning a *value* (None in this case).  More importantly,
//  the return type of this function provides an important clue to the 
//  programmer using this function, that it might not return an integer value.

/////////////////////////////////////////////////////////////////////////////



let z = tryDivide 2 0
//  Even if you were to ignore the clue, F# won't let you ignore it.
// let a = z * 10
//  I can't just use the value like a C# nullable.  It won't automatically
//  coerce itself to an integer.

let a = z.Value * 10
//  Now I could cheat and write this, but if z is None, it will throw an exception
//  and that's not what we want.
// let a = z |> function Some(n) -> n | None -> 0