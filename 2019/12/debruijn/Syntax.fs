module Lambda
open System

(*

# Fun with Languages and Pattern Matching!

One of my favorite features of F# is pattern matching.  It really is an amazing feature
because it's a Swiss Army knife of manipulating data, especially if that data is encoded 
with a tree-like data structure.  This means, it's great for manipulating Abstract Syntax 
Trees, XML and JSON like data-structures and anything that resembles a structured 
documents.

Because it's the holidays, I'm going to show you how I like to play with one of my favorite
tree-like data structures.  We're going to define a very simple lambda calculus AST and
use the amazing F# tools to slice, dice and manipulate structured data.

So, let's get started!

Here's the tree-like data type I'm going to use to encode my lambda calculus programs:

*)

type Term = 
    | Lambda of string * Term
    | App of Term * Term
    | Var of string

// Here are some examples I plucked from the Wikipedia:

// 0 := λf.λx.x
let ZERO = Lambda("f", Lambda("x", Var("x")))

// 1 := λf.λx.f x
let ONE = Lambda("f", Lambda("x", App(Var("f"), Var("x"))))

// 2 := λf.λx.f (f x)
let TWO = Lambda("f", Lambda("x", App(Var("f"), App(Var("f"), Var("x")))))


(*

As you can see, there's not a lot there.  Unlike the F# expression type 
(https://msdn.microsoft.com/visualfsharpdocs/conceptual/quotations.expr-class-%5bfsharp%5d), 
we our AST only lets us do three things:

1. You can define functions.  (Lambda)
2. You can your functions.  (App)
3. You can refer to variables. (Var)

Now even though this language doesn't have built-in numeric types, if statements, for-loops, 
tuples or even boolean values built in to it, I can encode those things into lambda calculus itself.

Before we get into that, I want to show you how you can use F# to turn pure lambda calculus 
into F# and how we can turn F# into pure lambda calculus.  

Let's start with a pattern matching function that turns my AST into F# code:

*)

let rec toFSharp = function
    | Var(name)               -> name
    | Lambda(paramName, body) -> sprintf "(fun %s -> %s)" paramName (toFSharp body)
    | App(func, arg)          -> sprintf "(%s %s)" (toFSharp func) (toFSharp arg)

(*

Let's break this down.  Here I'm defining a *recursive* pattern matching function 
(note the rec keyword) that will print our lambda functions as F# code.  What I like about
pattern matching is it lets me put examples on the right side and transformations on the 
left side.  It effectively lets me say:

* When I see a Var with a name inside, I'm just going to return the name.
* When I see a Lambda, give me the parameter name in the body:
    * I'll format the body and print it in this template to make it look like F#.
* When I see a function call (App), give me the function and it's argument:
    * I'll format both and put them in parenthesis.

Let's try it out:

*)

ONE |> toFSharp |> printfn "%s"  
//  Prints:  (fun f -> (fun x -> (f x)))

(*

That's not bad, but there are a lot of parenthesis and if I was writing this in F#,
I'd write it a little cleaner like so:

    fun f x -> f (f x)

Let's see if pattern matching can help us here.  I'm going to create a rule that will 
match this exact scenario and format it the way I want, then we'll go from there.

*)

let rec toFSharp = function
    //  When we see this pattern
    | Lambda(p1, Lambda(p2, App(Var(v1), Var(v2)))) 
                              // Return this
                              -> sprintf "fun %s %s -> %s %s" p1 p2 v1 v2
    | Var(name)               -> name
    | Lambda(paramName, body) -> sprintf "(fun %s -> %s)" paramName (toFSharp body)
    | App(func, arg)          -> sprintf "(%s %s)" (toFSharp func) (toFSharp arg)


//  Let's try it!
ONE |> toFSharp |> printfn "%s"   //  It now prints "fun f x -> f x"

(*

What makes pattern matching powerful is it allows us decode complicated instances by 
deconstructing complicated instances into its simpler parts.  Here we're testing to see if
the instance matches a relatively complicated shape.  When it matches, the properties are
automated extracted and bound to variables that we can use on the right side of the -> arrow.
In this case, that means we're getting the names of the parameters and the names of the variables.

As neat as this is, this pattern is too specific so we should generalize it a bit.  
After all, what happens when we have three lambdas or 4 lambdas?  Also, maybe we want 
to separate the logic that groups lambda parameters from the logic that determines
whether we need parenthesis.

Let's break out the application rule first and take a first stab at dealing with the
parenthesis.

*)

let rec toFSharp = function
    | Var(name)                    -> name
    //  When we see two lambdas, let's group them...  (We'll fix this in a bit)
    | Lambda(p1, Lambda(p2, body)) -> sprintf "fun %s %s -> %s" p1 p2 (toFSharp body)
    | Lambda(paramName, body)      -> sprintf "fun %s -> %s" paramName (toFSharp body)
    | App(func, arg)               -> sprintf "%s %s" (addParens true func)  (addParens false arg)


and addParens isLeft = function
    | Var(v1)                 -> v1
    | Lambda(_) as l          -> sprintf "(%s)" (toFSharp l)
    | App(_) as a when isLeft -> sprintf "%s" (toFSharp a)
    | App(_) as a             -> sprintf "(%s)" (toFSharp a)


//  Ok, let's try it out

ONE |> toFSharp |> printfn "%s"  
//  Perfect, it still prints "(fun f x -> f x)"

//  Lambda calculus is left-associative, so we shouldn't have parenthesis
App(App(Var("a"), Var("b")), Var("c")) |> toFSharp |> printfn "%s"
//  Prints "a b c"

//  Lambda calculus is left-associative, so we shouldn't have parenthesis
App(Var("a"), App(Var("b"), Var("c"))) |> toFSharp |> printfn "%s"
//  Prints "a (b c)"

App(App(ZERO, Var("b")), Var("c")) |> toFSharp |> printfn "%s"
//  Prints "(fun f x -> x) b c"

App(App(ZERO, Var("b")), ZERO) |> toFSharp |> printfn "%s"

(*

Before we deal with the lambda scenario, I want to show you an amazing F# feature called 
Active Patterns.  Just to clear things up:  Active Patterns are just functions that are 
called differently.  Let me show you with a simple example where I create two functions 
that add two numbers.  The first will be a regular function, the second will be an active 
pattern.

*)

//  Here's a basic function that accepts a tuple of integers
let add(x, y) = x + y

//  We call it like a regular function
let z = add(1, 2)

//  Here we define an active pattern that does the same thing.  
//  Notice how the name is surrounded by what we call banana clips (| |)
let (|Sum|) (x, y) = x + y

//  Active pattern functions are invoked on the left side of the equals sign.
let (Sum(z)) = (1, 2)

//  Oh yeah, if you have a function and want to turn it into an active pattern,
//  you can simple create an alias like so:

let (|Sum|) = add

(*

Going back to our printer, let's take our function printer function and turn it into an 
active pattern. 

*)

// Let's define the pattern FSharp.  Like toFSharp, it simply turns Terms into strings.
// Notice how we invoke the FSharp pattern on the left side of the ->
let rec (|FSharp|) = function
    | Var(name)                            -> name
    | Lambda(paramName, FSharp(body))      -> sprintf "(fun %s -> %s)" paramName body
    | App(FSharp(func), FSharp(arg))       -> sprintf "(%s %s)" func arg


(*

Active Patterns do more than allow us to format code differently, they can let us create more
complicated patterns that are based on more complicated tests.

Here we're going to create something called a partial active pattern.  It's called partial 
because you create partial active patterns with partial functions.  When we want to create a
partial function in F#, we always use the option type.

Here's a simpler partial active pattern that parses a string into an integer.  

*)

let tryToParseInt (input:string) = 
    let (ok, value) = Int32.TryParse(input)
    if ok then Some(value) else None

//  We can turn it into an partial active pattern like we did before with our banana clips.
//  BUT!  But active patterns require us to add some additional characters in the name to
//  indicate this is an partial functions.  NOTE the "|_|" in the name.

//  This is *NOT* how to define a partial active pattern, we're missing |_| in the name.
let (|Int|) = tryToParseInt

//  This is how to define a partial active pattern.
//  Also, all partial active patterns require your function returns an option type.
let (|Int|_|) = tryToParseInt

(*

Back to our printer.  I want to detect nested lambda functions.  Here's an example:

    S = λx.λy.λz.((xz)(yz))

When I come across nested lambdas like this, I want to extract:

    1. The list of all the parameters:    ["x"; "y"; "z"]
    2. The body of the inner most lambda: (x y)(y z) 

Here's how we do it with a recursive partial active pattern:

*)

let rec (|Lambdas|_|) = function
    //  Is it a sequences of lambdas?
    | Lambda(p1, Lambdas(parameterNames, body)) -> Some(p1::parameterNames, body)
    //  Is this the inner-most lambda?
    | Lambda(p1, body)                          -> Some([p1], body)
    //  If the term wasn't even a lambda, let's not match.
    | term                                      -> None


(*

Let's break this function down:

    1.  The first test checks to see if our term is a lambda.  Not only that,
        it also checks if the body is made up of a sequences of lambdas (One or more)
        When it does, we're going to return a list of strings for all of the parameter
        names and the body of the inner most lambda.
    2.  The first test only matches when there are two or more lambdas.  We need this 
        test to match our base case (the inner most lambda).

*)

//  Let's try it out:
let rec (|FSharp|) = function
    //  Here we use our new pattern
    | Lambdas(parameters, FSharp(body)) -> sprintf "(fun %s -> %s)" (String.concat " " parameters) body
    | App(FSharp(func), FSharp(arg))    -> sprintf "(%s %s)" func arg
    | Var(name)                         -> name



let toFSharp = (|FSharp|)

Lambda("a", Lambda("b", Lambda("c", Var("c")))) |> toFSharp |> printfn "%s"


(*

Let's switch gears a little bit, because I want to show you how we can use quotations to 
turn F# code into pure lambda calculus.  Quotations allow us to access the AST of F# code.

Let me show you:

*)

//  Here I'm wrapping a lambda function in a quotation block: <@  @>
let add = <@ fun x y -> x + y @>

//  Let's print it:
printfn "add: %A" add
//  PRINTS:  add: Lambda (x, Lambda (y, Call (None, op_Addition, [x, y])))

(*

The AST that's printed above, more or less, shows us how F# encodes a function 
before the compiler turns it into IL code.  In other words, we can see what F#'s 
parser sees and transform it into what we want.  The Fable compiler uses a form of
these expressions to turn F# into JavaScript.  

Let's try turning F# into lambda calculus!

*)

//  First, let's create a shortcut to the module that has all the active patterns 
//  we're going to use to transform our quotations.

module P = Microsoft.FSharp.Quotations.Patterns

//  Next, let's write a small function to get us started by mapping the basic elements:
let rec fromExpr = function
    | P.Var(v)                              -> Term.Var(v.Name)
    | P.Lambda(var, LTerm(expr))            -> Term.Lambda(var.Name, expr)
    | P.Application(LTerm(ex1), LTerm(ex2)) -> Term.App(ex1, ex2)
    | x -> failwithf "Don't know how to deal with %A" x
and (|LTerm|) = fromExpr

(*

Here the mapping was pretty trivial.  We mapped F#'s variables, lambdas and function application
to our simple lambda calculus.  For anything else, we throw an error.

*)

//  Let's try it out:
let ID = fromExpr <@ fun x f -> f x @>

printfn "%A" ID

(*

Let's make it a little more interesting.

From the Wikipedia page on Lambda Calculus, they show us how we can encode other things into
lambda calculus.  For example, here's how we can encode numbers:

0 := λf.λx.x
1 := λf.λx.f x
2 := λf.λx.f (f x)
3 := λf.λx.f (f (f x))

The pattern seems clear enough.  If we want to encode a positive integer, we'll wrap our body
of our lambda function with another call to f.  Let's write a function that does this for us:

*)

let rec numToLambda n = Lambda("f", Lambda("x", (numBodyToLamda n))) 
and numBodyToLamda = function
    | 0 -> Var("x")
    | n -> App(Var("f"), (numBodyToLamda (n - 1)))

numToLambda 4 |> toFSharp |> printfn "%s"
// PRINTS: (fun f x -> (f (f (f (f x)))))



type Term with
    member this.Call(t) = App(this, t)
    static member Fn([<ReflectedDefinition>] f:Expr<'a>) = fromExpr f

open Term

let ID    = Term.Fn(fun a -> a)
let TRUE  = Term.Fn(fun x y -> x)
let FALSE = Term.Fn(fun x y -> y)

// let AND = fromExpr <@ fun p q -> p q p @>
// let OR = fromExpr <@ fun p q -> p p q @>