# Adding Library References!

Now that we have a project, let's add some library references to our project.  I'm going to cd 
into the F# project folder and use dotnet to add a few Nuget packages to our project.

Specifically, I'm going to add the Fable.Core package and the Fable.Node package to get the basic node
bindings.  

```bash
cd src/MyTest/
dotnet add package Fable.Core
dotnet add package Fable.Node
```

Now that we have these bindings, we can do some stuff with node.  Let's cd back to the project
root and get our test script going:

```bash
cd ../../
npm run test
```

# Hello World ala JavaScript Console

We're going to start with something simple.  Instead of using the F# libraries to print something 
out, let's use the built-in JavaScript console object instead.

The Fable.Core package includes basic bindings to core JavaScript objects and a few other things.
We can use it in our program by adding ```open Fable.Core.JS``` to the top to tell F# we're using
that module.

We'll then call the console object, much like we would if we were writing JavaScript.


```fsharp
module MyTest

open Fable.Core.JS

[<EntryPoint>]
let main argv =
    
    console.clear()
    console.log("Hello world")

    0 // return an integer exit code

```

You should see this in your console:

```console
Hello World
```

