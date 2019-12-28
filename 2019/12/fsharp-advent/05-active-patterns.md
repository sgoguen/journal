
# Active Patterns

Before we deal with the lambda scenario, I want to show you an amazing F# feature called 
Active Patterns.  So what are Active Patterns?  They're just functions, but you call them differently than regular functions.  Let me show you with a simple example where I create two functions that add two numbers.  The first will be a regular function, the second will be an active  pattern.


Here's a basic function that accepts a tuple of integers
```fsharp
let add(x, y) = x + y
```

We call it like a regular function
```fsharp
let z = add(1, 2)
```

Now, let's define an active pattern for the same function.  We're going to tell F# this is an active pattern by surrounding it with banana clips (| |)
```fsharp
let (|Sum|) (x, y) = x + y
```

Let's invoke it!  
```fsharp
let (Sum(z2)) = (1, 2)
```

Ok, the big thing that stands out here is name of our function shifts to the left side of the equals side and the actual sum is nestled inside name inside the parenthesis.

The reason for doing this is to match patterns.  With our new active pattern, we can write pattern matching functions that do this:

```fsharp
let howDoTheseAddUp = function // int * int -> string
  | Sum(0) -> "Your numbers don't add up to nuttin'"
  | Sum(1) -> "You only have that one thing?"
  | Sum(n) -> sprintf "Your sum is: %i" n
```

They have a different syntax because they're used in very different circumstance.

One more thing, if you ever have a function you want to turn into an active pattern, you can simply assign it like so:

```fsharp
let (|Sum|) = add
```

Likewise, if you had an active pattern you wanted to call like a regular function, you can do the opposite:

```fsharp
let add = (|Sum|)
```

Going back to our printer, let's take our function printer function and turn it into an 
active pattern. 

## Active Patterns Matching Themselves 

My last example of an active pattern is a recursive one.  I'm going to rewrite out `toFSharp` function so it only uses active patterns.

Here it is in all it's glory!

```fsharp
let rec (|FSharp|) = function // Term -> string
    | Var(name)                            -> name
    | Lambda(paramName, FSharp(body))      -> sprintf "(fun %s -> %s)" paramName body
    | App(FSharp(func), FSharp(arg))       -> sprintf "(%s %s)" func arg
```

Take a good look at it and compare it to our original function.  Here's the original:

```fsharp
let rec toFSharp = function
    | Var(name)               -> name
    | Lambda(paramName, body) -> sprintf "(fun %s -> %s)" paramName (toFSharp body)
    | App(func, arg)          -> sprintf "(%s %s)" (toFSharp func) (toFSharp arg)
```

Do you notice how we invoke the active patterns on the left side of the arrow?  The big idea here is we when we start doing stuff on the right side of the arrow, we'll have exactly the information we need.  In this case, our nested are already formatted into `FSharp` expressions and are ready to be put into templates! 

That's pretty powerful stuff, but I have one more type of active pattern I want to show you that will really make manipulating languages a lot easier.

[Up Next - Partial Active Patterns!!! >>>](06-partial-active-patterns.md)
