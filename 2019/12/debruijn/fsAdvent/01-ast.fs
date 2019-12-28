module Advent.AST
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

*)