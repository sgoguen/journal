# Total & Partial Functions

You could divide functions into two categories:

*total functions* - These functions return a value given and input (assuming the input is in the domain of the function)
*partial functions* - These don't.

```javascript
//  Here's an example of a partial function

function returnPositive(x) {
    if(x >= 0) {
        return x;
    }
    //  What happens when x < 0?
}
```

So F# makes it a little more difficult to  create partial functions:

```fsharp

let returnPositive x = 
    if x >= then x
    
```

Here's the problem with the above function in F#:

It doesn't compile.

In F#, we don't actually have if *statements*.  We have **conditional expressions**.

So...  What's the difference?

```javascript
//  Let's look at a JavaScript example

var x = 5;
var y = (x > 0) ? (x * x) : (x + x);

```

The above assignment in JavaScript uses the ternary operator to test if `x > 0` and then conditionally assigns y to either `x * x` or `x + x` depending on the result.

In F#, it would look like this:

```fsharp
let x = 5
let y = if x > 0 then x * x else x + x
```

So, in F#, the if keyword is really the ternary operator.

This means you *almost always* must handle the else case.

Almost...

## Total Functions

So what's an example of a total function?  

Well... we could use the above code:

```fsharp
let calc(x) = 
    if x > 0 then 
        x * x 
    else 
        x + x
```

This is a total function.

## Partial Functions

Question:  Is this function a partial function?

```fsharp
let divideInts x y = x / y
```

We would consider this to be a partial function because dividing by zero is undefined.  Practically speaking, it throws a runtime exception.