module JWT

open System
open System.Security.Cryptography
open System.Text


open System.IdentityModel.Tokens.Jwt
open Microsoft.IdentityModel.Tokens
open System.Security.Claims
open System.Security.Cryptography.X509Certificates

let generatePublicAndPrivateKeys() = 
    let rsa = new RSACryptoServiceProvider(2048)
    let publicKey = rsa.ToXmlString(false)
    let privateKey = rsa.ToXmlString(true)
    (publicKey, privateKey)


let signTokenWithShareKey (secret: string, claims: Claim[]) =
    let tokenDescriptor = new SecurityTokenDescriptor()
    tokenDescriptor.Subject <- new ClaimsIdentity(claims)
    tokenDescriptor.Expires <- DateTime.UtcNow.AddHours(1.0)
    let key = Encoding.ASCII.GetBytes(secret)

    tokenDescriptor.SigningCredentials <-
        new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

    let tokenHandler = new JwtSecurityTokenHandler()
    let token = tokenHandler.CreateToken(tokenDescriptor)
    let signedToken = tokenHandler.WriteToken(token)
    signedToken

let signTokenWithWebKey (jKey: JsonWebKey, claims: Claim[]) =
    let tokenDescriptor = new SecurityTokenDescriptor()
    tokenDescriptor.Subject <- new ClaimsIdentity(claims)
    tokenDescriptor.Expires <- DateTime.UtcNow.AddHours(1.0)
    tokenDescriptor.SigningCredentials <- new SigningCredentials(jKey, SecurityAlgorithms.RsaSha256Signature)

    let tokenHandler = new JwtSecurityTokenHandler()
    let token = tokenHandler.CreateToken(tokenDescriptor)
    let signedToken = tokenHandler.WriteToken(token)
    signedToken

type JwtVerificationResult =
    | Valid of ClaimsPrincipal * SecurityToken
    | Invalid of string

    member this.IsVerified =
        match this with
        | Valid (c, t) -> c.Identity.IsAuthenticated && t.ValidTo > DateTime.UtcNow
        | Invalid _ -> false


let verifyTokenWithWebKey (jKey: JsonWebKey, token: string) =
    try 
        let tokenHandler = new JwtSecurityTokenHandler()
        let validationParameters = new TokenValidationParameters()
        validationParameters.ValidateIssuerSigningKey <- true
        validationParameters.IssuerSigningKey <- jKey
        validationParameters.ValidateLifetime <- true
        validationParameters.ClockSkew <- TimeSpan.Zero
        validationParameters.ValidateAudience <- false
        validationParameters.ValidateIssuer <- false
        Valid(tokenHandler.ValidateToken(token, validationParameters))
    with ex ->
        Invalid(ex.Message)


let verifyJwt (signedToken: string, secret: string) =
    try
        let tokenHandler = new JwtSecurityTokenHandler()
        let key = Encoding.ASCII.GetBytes(secret)
        let validationParameters = new TokenValidationParameters()
        validationParameters.IssuerSigningKey <- new SymmetricSecurityKey(key)
        validationParameters.ValidateIssuer <- false
        validationParameters.ValidateAudience <- false

        Valid(tokenHandler.ValidateToken(signedToken, validationParameters))
    with ex ->
        Invalid(ex.Message)




module Tests =
    open Xunit



    //  Let's create a token for our tests
    let secret = "We can create a key more than 128 bits long"
    let claims = [| new Claim(ClaimTypes.Name, "John Doe") |]
    let signedToken = signTokenWithShareKey (secret, claims)
 
    [<Fact>]
    let ``We should be able to generate a token`` () = Assert.True(signedToken.Length > 0)

    // Verify the token
    [<Fact>]
    let ``We can use a shared secret and use it to sign a JWT and verify it`` () =
        let result = verifyJwt (signedToken, secret)
        Assert.True(result.IsVerified)

    // Using the wrong secret should fail
    [<Fact>]
    let ``Using the wrong secret should fail`` () =
        let (result) = verifyJwt (signedToken, "wrong secret")
        Assert.False(result.IsVerified)


    [<Fact>]
    let ``Can generate a JsonWebKey``() = 

        let rsa = new RSACryptoServiceProvider(2048)
        
        

        let privateKey = JsonWebKeyConverter.ConvertFromRSASecurityKey(new RsaSecurityKey(rsa))
        Assert.True(privateKey.Kty = "RSA")
        Assert.True(privateKey.HasPrivateKey, "Should have a private key")


        //  Only get the public key from the private key
        let publicKey = JsonWebKeyConverter.ConvertFromRSASecurityKey(new RsaSecurityKey(rsa.ExportParameters(false)))
        Assert.True(publicKey.Kty = "RSA")
        Assert.False(publicKey.HasPrivateKey, "Should not have a private key")

        // // Use the key to sign a token
        let claims = [| new Claim(ClaimTypes.Name, "John Doe") |]
        let signedToken = signTokenWithWebKey (privateKey, claims)
        Assert.True(signedToken.Length > 0)

        // // Verify the token
        let result = verifyTokenWithWebKey (publicKey, signedToken)
        Assert.True(result.IsVerified)

    