# Getting Started with Fable Splitter

Fable-splitter is a fun way to try our F# if you want to target JavaScript and the node runtime.  What makes it particularly great is it allows you to see how your F# code gets compiled into JavaScript while you're developing.

Before you begin, you should visit the Fable website at [fable.io](https://fable.io) and click on the [Get Started](https://fable.io/docs/2-steps/setup.html) link to make sure you have everything you'll need.  For this, you'll need Node, NPM, .NET Core SDK.  For an editor, I would recommend VS Code and the *most awesome* [Ionide plug-in for VS Code](http://ionide.io/).

Ok, let me show you how to get started:

##  Step 1 - Create an NPM Project

Let's create a new folder:

```bash
mkcdir ~/journal/2019/08/splitter-demo
```

( Note: [mkcdir](https://unix.stackexchange.com/questions/125385/combined-mkdir-and-cd) to creates and takes you to the directory.)

We'll then create our NPM project:

```bash
npm init
```

##  Installing Fable-Splitter and the Fable Compiler

Both the Fable compiler and Fable Splitter are available as NPM packages, so let's install them:

```bash
npm install fable-compiler fable-splitter
```

##  Creating an F# Project

Finally, we'll use the dotnet CLI to create a simple console project:

```bash
mkcdir src/MyTest
dotnet new console --language F#
```

##  Configuring NPM to compile and execute out F# Project

At this point, I'm really interested in setting up a quick test-feedback loop, so let's modify the test script to watch our F# project and execute it any time we change the file.  I also want it to write the files in a folder called dist.

I'll do that by calling fable-splitter like so:

```bash
fable-splitter src/MyTest/MyTest.fsproj -w --run -o dist/
```

The -w flag watches for changes and the --run flag executes the project.

Our package.json should look something like this:

```json
{
    "name": "splitter-demo",
    "version": "1.0.0",
    "description": "",
    "main": "index.js",
    "scripts": {
        "test": "fable-splitter src/MyTest/MyTest.fsproj -w --run -o dist/"
    },
    "author": "",
    "license": "ISC",
    "dependencies": {
        "fable-compiler": "^2.3.20",
        "fable-splitter": "^2.1.10"
    }
}
```

# Let's test it!

We'll go back to the root of our project and test it out:

```bash
cd ../..
npm run test
```

# Oh no!  An error...

Unfortunately, when I run it, I get this error:

```bash
fable: Compiled src/MyTest/Program.fs
/Users/sgoguen/journal/2019/08/splitter-demo/src/MyTest/Program.fs(3,1): (4,1) error FSHARP: Files in libraries or multiple-file applications must begin with a namespace or module declaration, e.g. 'namespace SomeNamespace.SubNamespace' or 'module SomeNamespace.SomeModule'. Only the last source file of an application may omit such a declaration. (code 222)
fable: Compilation failed at 3:42:01 PM (0.032 s)
```

This is complaining that I don't have a module or namespace in my main file, Program.fs.  I can fix it by adding a module to the top of the file like so:

```fsharp
module MyTest
// Add this line above

open System

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    0 // return an integer exit code
```

# Let's have some fun!

First, try making some small changes to the script and save the file.  Notice how fast it run?

Also, if you look in the ```dist/``` folder, you'll notice a file called Program.js.  Often, I like to open that file up, split my screen and look at the JavaScript output while I write F#.

If you're a JavaScript developer, this is a great way to get started and get familiar with F#.  While many of the features correspond to JavaScript in a one-to-one way, sometimes you'll see it expands to a lot more.