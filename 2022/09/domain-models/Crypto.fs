module Crypto

open System
open System.Security.Cryptography
open System.Text

let generatePublicAndPrivateKeys () =
    let rsa = new RSACryptoServiceProvider(2048)
    let publicKey = rsa.ToXmlString(false)
    let privateKey = rsa.ToXmlString(true)
    (publicKey, privateKey)

type SignedMessage =
    { Message: string
      Signature: string
      PublicKey: string }

let inline SignedMessage (message, signature, publicKey) =
    { Message = message
      Signature = signature
      PublicKey = publicKey }

let inline (|SignedMessage|) (signedMessage: SignedMessage) =
    signedMessage.Message, signedMessage.Signature, signedMessage.PublicKey

let signMessage (message: string, privateKey: string, publicKey: string) =
    let rsa = new RSACryptoServiceProvider(2048)
    rsa.FromXmlString(privateKey)
    let bytes = Encoding.UTF8.GetBytes(message)
    let signature = rsa.SignData(bytes, new SHA1CryptoServiceProvider())
    let base64Signature = Convert.ToBase64String(signature)
    SignedMessage(message, base64Signature, publicKey)

let verifyMessage ((SignedMessage (message, signature, publicKey))) =
    try
        let rsa = new RSACryptoServiceProvider(2048)
        rsa.FromXmlString(publicKey)
        let bytes = Encoding.UTF8.GetBytes(message)
        let base64Signature = Convert.FromBase64String(signature)
        rsa.VerifyData(bytes, new SHA1CryptoServiceProvider(), base64Signature)
    with ex ->
        false

module Tests =

    open Xunit

    [<Fact>]
    let ``We can generate public and private keys`` () =
        let (publicKey, privateKey) = generatePublicAndPrivateKeys ()
        Assert.True(publicKey.Length > 0)
        Assert.True(privateKey.Length > 0)

    [<Fact>]
    let ``We can use the private key to sign a message and the public key to verify the signature`` () =
        let (publicKey, privateKey) = generatePublicAndPrivateKeys ()
        let signedMessage = signMessage ("Hello World", privateKey, publicKey)
        Assert.True(verifyMessage (signedMessage))

    let (publicKey, privateKey) = generatePublicAndPrivateKeys ()
    let signedMessage = signMessage ("Hello World", privateKey, publicKey)

    [<Fact>]
    let ``An unmodified signed message should verify`` () =
        // This is a message signed with a good key - it should verify
        Assert.True(verifyMessage (signedMessage))

    //  Let's remove the last character from the public key and try to verify the message again
    [<Fact>]
    let ``A bad key should not verify a message`` () =
        let messageWithBadKey =
            SignedMessage(signedMessage.Message, signedMessage.Signature, publicKey.Substring(0, publicKey.Length - 1))

        Assert.False(verifyMessage (messageWithBadKey))

    //  Changing the message should also fail verification
    [<Fact>]
    let ``A modified message should not verify`` () =
        let messageWithBadMessage =
            SignedMessage("Hello World!", signedMessage.Signature, publicKey)

        Assert.False(verifyMessage (messageWithBadMessage))

    //  Changing the signature should also fail verification
    [<Fact>]
    let ``A modified signature should not verify`` () =
        let messageWithBadSignature =
            SignedMessage(signedMessage.Message, "bad signature", publicKey)

        Assert.False(verifyMessage (messageWithBadSignature))
