# Fun with Languages and Pattern Matching!


I'm really excited to be taking part of the [F# Advent Calendar](https://sergeytihon.com/2019/11/05/f-advent-calendar-in-english-2019/) this year and I want to thank Sergey Tihon for organizing it and everything else he does to cultivate the community.  Thank you Sergey!

One more thing before I start:  This year the Washington DC F# group is organizing the [**Capitol F#**](https://www.capitolfsharp.org/) conference this year and I want to put a reminder out that we are still accepting a [call for speakers](https://sessionize.com/capitol-fsharp) up to December 31st.

## I Just Love to Pattern Match - It's my Favorite

Pattern matching is one of my favorite F# features hands down.  I always knew I liked them, but after seeing [Paul Blasucci's talk](https://github.com/pblasucci/DeepDive_ActivePatterns), I fell in love with them

It really is an amazing feature.  It's a Swiss Army knife for manipulating data, especially if that data is encoded 
with a tree-like data structure.  It slices and dices ASTs, XML, JSON and other tree-like structures like not other.

Because it's the holidays and I like to play with toys, I'm going to show you how we can use pattern matching to play with a toy version of lambda calculus.  We're going to define a very simple lambda calculus AST and use this amazing F# tool to slice, dice and manipulate tree structured data types.

So, let's get started!

Here's what we're going to use to encode all of our lambda calculus programs thoughout this post:

```fsharp
type Term = 
    | Lambda of string * Term
    | App of Term * Term
    | Var of string
```

As you can see, there's not a lot there.  Unlike the [F# expression type](https://msdn.microsoft.com/visualfsharpdocs/conceptual/quotations.expr-class-%5bfsharp%5d), 
we our AST only lets us do three things:

1. You can define functions.  (Lambda)
2. You can your functions.  (App)
3. You can refer to variables. (Var)

Now even though this language doesn't have built-in numeric types, if statements, for-loops, 
tuples or even boolean values built in to it, I can encode those things into lambda calculus itself.

Here's how we might encode numbers in lambda calculus using [Church encoding](https://en.wikipedia.org/wiki/Church_encoding).

```fsharp
// 0 := λf.λx.x
let ZERO = Lambda("f", Lambda("x", Var("x")))

// 1 := λf.λx.f x
let ONE = Lambda("f", Lambda("x", App(Var("f"), Var("x"))))

// 2 := λf.λx.f (f x)
let TWO = Lambda("f", Lambda("x", App(Var("f"), App(Var("f"), Var("x")))))
```

By the time we get to the end of this post, I want to show you how we can use pattern matching to transform pure lambda calculus into F# and F# back into lambda calculus.

Let's not waste time.  Let's start by making a simple printer that will give use our F# code.

[Next Up - Turning Lambda Calculus into F# >>>](02-basic-formatter.md)
