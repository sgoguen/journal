
# Active Patterns

Before we deal with the lambda scenario, I want to show you an amazing F# feature called 
Active Patterns.  Just to clear things up:  Active Patterns are just functions that are 
called differently.  Let me show you with a simple example where I create two functions 
that add two numbers.  The first will be a regular function, the second will be an active 
pattern.


```fsharp
//  Here's a basic function that accepts a tuple of integers
let add(x, y) = x + y

//  We call it like a regular function
let z = add(1, 2)

//  Here we define an active pattern that does the same thing.  
//  Notice how the name is surrounded by what we call banana clips (| |)
let (|Sum|) (x, y) = x + y

//  Active pattern functions are invoked on the left side of the equals sign.
let (Sum(z2)) = (1, 2)

//  Oh yeah, if you have a function and want to turn it into an active pattern,
//  you can simple create an alias like so:

let (|Sum2|) = add
```

Going back to our printer, let's take our function printer function and turn it into an 
active pattern. 


```fsharp
// Let's define the pattern FSharp.  Like toFSharp, it simply turns Terms into strings.
// Notice how we invoke the FSharp pattern on the left side of the ->
let rec (|FSharp|) = function
    | Var(name)                            -> name
    | Lambda(paramName, FSharp(body))      -> sprintf "(fun %s -> %s)" paramName body
    | App(FSharp(func), FSharp(arg))       -> sprintf "(%s %s)" func arg
```
