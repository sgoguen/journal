module Lambda
open System

(*

# Fun with Languages and Pattern Matching!

One of my favorite features of F# is pattern matching.  It really is an amazing feature
Because it's a Swiss Army knife of manipulating data, especially if that data is encoded 
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
let TWO = Lambda("f", Lambda("x", App(Var("f"), Var("x"))))


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

TWO |> toFSharp |> printfn "%s"  //  Prints:  (fun f -> (fun x -> (f x)))

(*

That's not bad, but there are a lot of parenthesis and if I was writing this in F#,
I'd write it a little cleaner like so:

    fun f x -> f x

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
TWO |> toFSharp |> printfn "%s"   //  It now prints "fun f x -> f x"

(*

What makes pattern matching powerful is it allows us decode complicated instances  
deconstructing complicated instances into simpler parts.  Here we're testing to see if
the instance matches a relatively complicated shape, then we extract all of the relevent
information from it.  In this case, that means we're getting the names of the parameters
and the names of the variables.

Unfortunately, this pattern is too specific and we need to generalize it.  What happens 
when we have three lambdas or 4 lambdas?  Also, maybe we want to separate how we format 
lambda parameters from how we format function application.

Let's break out the application rule first.

*)

let rec toFSharp = function
    | Var(name)                    -> name
    //  When we see two lambdas, let's group them...  (We'll fix this in a bit)
    | Lambda(p1, Lambda(p2, body)) -> sprintf "(fun %s %s -> %s)" p1 p2 (toFSharp body)
    | Lambda(paramName, body)      -> sprintf "(fun %s -> %s)" paramName (toFSharp body)
    //  If we call a function with a variable as the argument, we don't need parenthesis
    | App(func, Var(v1))           -> sprintf "%s %s" (toFSharp func) v1
    | App(func, arg)               -> sprintf "(%s %s)" (toFSharp func) (toFSharp arg)

TWO |> toFSharp |> printfn "%s"  
//  Prints "(fun f x -> f x)"

App(App(Var("a"), Var("b")), Var("c")) |> toFSharp |> printfn "%s"
//  Prints "a b c"

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
    | Lambda(p1, Lambda(p2, FSharp(body))) -> sprintf "(fun %s %s -> %s)" p1 p2 body
    | Lambda(paramName, FSharp(body))      -> sprintf "(fun %s -> %s)" paramName body
    | App(FSharp(func), Var(v1))           -> sprintf "%s %s" func v1
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

//  This is NOT how to define a partial active pattern
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
    //  This will never get caught.  Technically we can comment it out, but the F# compiler
    //  will generate a warning because it can't be sure we're testing for lambdas.
    | Lambda(p1, FSharp(body))          -> sprintf "(fun %s -> %s)" p1 body
    | App(FSharp(func), Var(v1))        -> sprintf "%s %s" func v1
    | App(FSharp(func), FSharp(arg))    -> sprintf "(%s %s)" func arg
    | Var(name)                         -> name


let toFSharp = (|FSharp|)

Lambda("a", Lambda("b", Lambda("c", Var("c")))) |> toFSharp |> printfn "%s"


(*

Let's switch gears a little bit, because I want to show you how we can use quotations to 
turn F# code into pure lambda calculus.

*)



(*



Ok.  We took care of the application rule, but we still need to deal with the lambda scenario. 

Now, let's say we want to convert some F# code into our lambda calculus AST.

* Whenever you have some data bundled together in tuples or discriminated unions

I first learned about Lambda calculus a 15 years and I'm still amazed by it for so many
reasons.

First, Lambda calculus was introduced in the 1930's by Alonzo Church back when
mathematicians were looking for an axiomatic foundation and algorithm where they
could feed in an arbitrary statement written in some type of formal language to see
if the statement was true or false.  While mathematicians were able to construct formal
languages to encode these questions, they learned there are simply some questions that
simply won't ever return a simple true or false.

Nevertheless, I'm still absolutely enamoured by Church's lambda calculus because he was 
able to create an incredibily powerful minimalist language that makes almost every modern
programming language look incredibly complicated by comparison.  Why?  Because lamba
calculus really only lets you do three things:



That's it!  Really!

Check this out:

*)



(*
Here I'm defining the data types for our lambda calculus AST.  As you can see it's very
minimalistic, but it's 

If I wanted to define the ID function, I could encode it by defining a very simple lambda 
function that accepts a single argument and returns it.  In F#, we'd write it like this:
*)

let id = fun x -> x

(*
With our little lambda calculus 
*)

let ID = Lambda("x", Var("x"))

(*

I don't know abou

*)

let rec termToFSharp = function
    | Var(var)          -> sprintf "%s" var
    | Lambda(var, body) -> sprintf "fun %s -> %s" var (termToFSharp body)
    | App(func, arg)    -> sprintf "(%s %s)" (termToFSharp func) (termToFSharp arg)

let printTerm = termToFSharp >> printfn "%s"

printTerm ID
printTerm (Lambda("x", Lambda("y", App(Var("x"), App(Var("x"), Var("y"))))))


(*

This is all we need to encode a lambda calculus function.  We have no notion of if 
statements, boolean values, for loops, data types.

However...  We can encode those things into this language here.

*)

open Microsoft.FSharp.Quotations


    open Microsoft.FSharp.Quotations.Patterns

    let rec fromExpr = function
        | Var(v)                              -> Term.Var(v.Name)
        | Application(LTerm(ex1), LTerm(ex2)) -> Term.App(ex1, ex2)
        | Lambda(var, LTerm(expr))            -> Term.Lambda(var.Name, expr)
        | x -> failwithf "Don't know how to deal with %A" x
    and (|LTerm|) = fromExpr

type Term with
    member this.Call(t) = App(this, t)
    static member Fn([<ReflectedDefinition>] f:Expr<'a>) = Term.fromExpr f

open Term

let ID    = Term.Fn(fun a -> a)
let TRUE  = Term.Fn(fun x y -> x)
let FALSE = Term.Fn(fun x y -> y)

// let AND = fromExpr <@ fun p q -> p q p @>
// let OR = fromExpr <@ fun p q -> p p q @>