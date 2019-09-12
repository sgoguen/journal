#load "cantor.fs"

// Imagine we're going to create a simple programming language
// or calculator.

// To test your language, wouldn't it be nice if we could 
// create a function that takes any integers and returns
// the same and unique program.  Small integers should create
// simple programs and larger integers should create big programs.

//  Let's define our a calculator language:

type Calc = 
    | Value of int
    | Add of Calc * Calc
    | Mul of Calc * Calc

//  Let's take a naive stab at mapping integers to
//  our Calc language.  Let's start by using pattern
//  matching to create examples of how we might map 
//  integers to our language.

let fromInt = function
    | 0 -> Value(0)
    | 1 -> Add(Value(0), Value(0))
    | 2 -> Mul(Value(0), Value(0))
    | 3 -> Value(1)
    | 4 -> Add(Value(1), Value(0))
    | 5 -> Mul(Value(1), Value(0))
    | 6 -> Value(2)
    | 7 -> Add(Value(0), Value(1))