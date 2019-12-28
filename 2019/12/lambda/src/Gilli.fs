module Gilli

open System
open System.Text

type Token =
    | LParen
    | RParen
    | Lambda
    | Dot
    | Variable of char

// let alphabet = Set.ofSeq "abcdefghijklmnopqrstuvwxyz"

let (|Letter|_|) c =
    if Char.IsLetter(c) then Some(c) else None

let (|WhiteSpace|_|) c =
    if Char.IsWhiteSpace(c) then Some(c) else None

let stringToList (str: string) = List.ofSeq str

let listToString (list: char list) = list |> List.map string |> String.concat ""

let rec tokenize = function
    | [] -> []
    | '(' :: rest -> LParen :: tokenize rest
    | ')' :: rest -> RParen :: tokenize rest
    | '.' :: rest -> Dot :: tokenize rest
    | '\\' :: rest -> Lambda :: tokenize rest
    | 'λ' :: rest -> Lambda :: tokenize rest
    | Letter(c) :: rest -> Variable(c) :: tokenize rest
    | WhiteSpace(_) :: rest -> tokenize rest
    | rest -> failwithf "Failed to parse: '%s'" (listToString rest)

// module TokenizeTests =
//     let tokenizeT = stringToList >> tokenize
//     let t1 = tokenizeT "λxyz.xx y y zz"

type Term =
    | VariableT of char
    | LambdaT of char * Term
    | ClosureT of char * Term * Env
    | ApplicationT of Term * Term
and Env = (char * Term) list


let rec parseSingle = function
    | Variables(vars, rest)                              -> applyLeftToRight vars, rest
    | Lambda :: Variables(args, Dot :: Term(body, rest)) -> buildLambda body args, rest
    // | Lambda :: Variable arg :: Dot :: Term(body, rest)  -> LambdaT(arg, body), rest
    | Parens(t, rest)                                    -> t, rest
    // | LParen :: Term(fn, Term(value, RParen :: rest))    -> ApplicationT(fn, value), rest
    | _ -> failwith "Bad parse"
and (|Term|) = parseSingle
and (|Parens|_|) = function
    | LParen :: Term(t, RParen :: rest) -> Some(t, rest)
    | _ -> None 
and (|Variables|_|) = function
    | Variable(v) :: Variables(vList, rest) -> Some(v::vList, rest)
    | Variable(v) :: rest -> Some([v], rest)
    | _ -> None
and buildLambda body = function
    | var::vars -> LambdaT(var, (buildLambda body vars))
    | [] -> body
and applyLeftToRight = function
    | [var] -> VariableT(var)
    | var::vars -> List.fold (fun s c -> ApplicationT(s, VariableT(c))) (VariableT(var)) vars
    | [] -> failwithf "Needed"

// and (|Applications|_|) = function
//     | Variable(v1) :: Variable(v2) :: rest   -> Some(ApplicationT(VariableT(v1), VariableT(v2)), rest)
//     | Applications(a,  Variable(v2) :: rest) -> Some(ApplicationT(a, VariableT(v2)), rest)
//     | _                                      -> None


let parse tokens = fst <| parseSingle tokens

// module ParserTests =
//     let parseT (s:string) = s |> stringToList |> tokenize |> parse
//     let t1 = parseT "λxyz.xx y y zz"

// let tokenizeStr: string -> Token list = List.ofSeq >> tokenize

// let parseStr: string -> Term =
//     List.ofSeq
//     >> tokenize
//     >> parse

// module Examples1 =
//     tokenizeStr "\\a.aa" |> printfn "%A"
//     parseStr "\\x.(x x)" |> printfn "%A"

// let rec evalInEnv (env: Env) (term: Term): Term =
//     match term with
//     | VariableT name ->
//         match List.tryFind (fun (aName, term) -> aName = name) env with
//         | Some(_, term) -> term
//         | None -> failwith "Couldn't find a term by name"
//     | LambdaT(arg, body) -> ClosureT(arg, body, env)
//     | ApplicationT(fn, value) ->
//         match evalInEnv env fn with
//         | ClosureT(arg, body, closedEnv) ->
//             let evaluatedValue = evalInEnv env value

//             let newEnv = (arg, evaluatedValue) :: closedEnv @ env

//             evalInEnv newEnv body
//         | _ -> failwith "Cannot apply something given"
//     | closure -> closure

// let eval (term: Term): Term = evalInEnv [] term

let rec pretty (term: Term): char list =
    match term with
    | VariableT name -> [ name ]
    | LambdaT(arg, body) -> [ 'λ'; arg; '.' ] @ pretty body
    | ClosureT(arg, body, _) -> [ 'λ'; arg; '.' ] @ pretty body
    | ApplicationT(fn, value) -> [ '(' ] @ pretty fn @ [ ' ' ] @ pretty value @ [ ')' ]

// let interp: char list -> char list =
//     tokenize
//     >> parse
//     >> eval
//     >> pretty

// let interpString: string -> string =
//     List.ofSeq
//     >> interp
//     >> List.map string
//     >> String.concat ""


// let TRUE = parseStr "\\x.\\y . x"
// let FALSE = parseStr "\\x.\\y . y"
// let AND = parseStr "\\p.\\q . ((p q) p)"

// let x =
//     ApplicationT(AND, FALSE)
//     |> eval
//     |> pretty
//     |> List.map string
//     |> String.concat ""