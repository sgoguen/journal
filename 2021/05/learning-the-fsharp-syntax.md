# Learning the F# Syntax First --> Then The Ideas

I think a lot of people are turned off by F# because it has an unfamiliar syntax. <show wall of code>  It seems like an unnecessary barrier. But it's not the only reason people are turned off.  
  
 Learning a functional programming language usually means learning new ideas IN ADDITION to a new syntax.

That's really hard!!!

So let's try something different:  Let's learn the syntax first and use that as a steppingstone to learning new ideas!
  
If you're a developer who knows JavaScript and is curious about functional programming, then this is for you.

## Learning The Syntax!

Throughout this endeavor, we're going to use a tool called the Fable Compiler to help us learn the F# syntax.

The Fable Compiler will help you learn F# because the Fable Compiler compiles F# into JavaScript.  It's the way most F# developers do web development.  

It's not the only way or the only F# to JavaScript compiler.  It's the most popular and it's a great teaching tool.

-----

### Let there be programs

The most important keyword in F# is ***let***.  You use it to define variables and functions.

```fsharp
let x = 5
let myName = "Steve"
let addOne x = x + 1
```

If you copy this into the Fable compiler, you get JavaScript that looks like this:

```javascript

export const x = 1;

export const myName = "Steve";

export function addOne(x_1) {
    return x_1 + 1;
}
```
  
# Notes About Examples
  
```fsharp
let addOne x = x + 1
```

* It's a simple one-line function
* It automatically infers the input and return type
* We say that functions map from one type (int) to another type (int)
* Function signatures usually tell us what functions do
* The types usually tell us how to use something
* Expressions compute values - Statements perform actions
* F# nudges you to think about your problem in terms of computing values
  
```fsharp
let addOne x = x.ToString() + "1"
``
  
* All types support the ToSting() method in F#
* This makes it hard for F# to infer a type
* So F# will automatically make this function generic for all types.
* This is called Automatic generalization

  
