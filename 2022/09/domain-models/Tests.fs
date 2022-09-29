module Tests

open System
open Xunit
open Crypto

type Person(firstName, lastName) =
    let (privateKey, publicKey) = generatePublicAndPrivateKeys ()
    member this.FirstName = firstName
    member this.LastName = lastName
    member this.PublicKey = publicKey
    member this.CreateReceipt(message: string) = 
        Receipt(this, message)

and Receipt(person, message) =
    member this.Person = person
    member this.Message = message



[<Fact>]
let ``A person can create a simple text receipt`` () =
    let person = new Person("John", "Doe")
    let receipt = person.CreateReceipt("I owe Bob $10")

    Assert.Equal("John", receipt.Person.FirstName)
    Assert.Equal("Doe", receipt.Person.LastName)
    Assert.Equal("I owe Bob $10", receipt.Message)
