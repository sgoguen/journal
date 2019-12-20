#load "cantor.fs"

//  Imagine we want to create a simple programming language
//  or a calculator.

//  It would be nice to have a function that could generate
//  examples from integers.  But it would be nice if it 
//  wasn't random.  Small integers would generate small 
//  programs and big integers, big programs.

//  Let's define a simple calculator language:

type Calc = 
    | Value of int        //  We'll have integers
    | Add of Calc * Calc  // Addition
    | Mul of Calc * Calc  // Multiplication

//  Now let's take a stab, using pattern matching, to
//  explore what a function that creates programs from 
//  integers might look like:

let fromInt = function
    | 0 -> Value(0)
    | 1 -> Add(Value(0), Value(0))
    | 2 -> Mul(Value(0), Value(0))
    | 3 -> Value(1)
    | 4 -> Add(Value(1), Value(0))
    | 5 -> Mul(Value(1), Value(0))
    | 6 -> Value(2)
    | 7 -> Add(Value(0), Value(1))
    | 8 -> Mul(Value(0), Value(1))
    | 9 -> Value(3)
    | 10 -> Add(Value(2), Value(0))
    | 11 -> Mul(Value(2), Value(0))

//  There's more than one way to map the set of integers to our
//  language, but this way makes sense to me.  It balances all our
//  options, and it should eventually map to every possible Calc program.
//  (We're ignoring negative ints right now)