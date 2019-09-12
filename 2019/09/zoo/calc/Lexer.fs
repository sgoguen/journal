module Calc.Lexer
open System
open System.Text

(*
rule lexeme = parse
  | [' ' '\t' '\r']  { lexeme lexbuf }
  | '\n'        { Lexing.new_line lexbuf; lexeme lexbuf }
  | ['0'-'9']+  { Parser.NUMERAL (int_of_string (Lexing.lexeme lexbuf)) }
  | '+'         { Parser.PLUS }
  | '-'         { Parser.MINUS }
  | '*'         { Parser.TIMES }
  | '/'         { Parser.DIVIDE }
  | '('         { Parser.LPAREN }
  | ')'         { Parser.RPAREN }
  | eof         { Parser.EOF }

*)

type Lexeme = 
    | NUMERAL of int
    | PLUS
    | MINUS
    | TIMES
    | DIVIDE
    | LPAREN
    | RPAREN
    | EOF

let toString (chars:char list) = 
    let sb = StringBuilder()
    (chars |> List.fold (fun s c -> sb.Append(c)) sb).ToString()

///  This function takes in a list of characters and a test function and returns the first n characters
///  that test true
let rec beginsWith test = function
    | [] -> None
    | c::rest ->
        if test c then
            match beginsWith test rest with
            | Some(chars, rest) -> Some(c::chars, rest)
            | None -> Some([c], rest)
        else
            None

let (|BeginsWith|_|) test char = beginsWith test char

let (|WhiteSpace|_|) = beginsWith Char.IsWhiteSpace
let (|Numeral|_|) text = match beginsWith Char.IsNumber text with
                         | Some(chars, rest) -> Some(int(toString(chars)), rest)
                         | None              -> None

let (|Plus|_|) = beginsWith (fun c -> c = '+')
let (|Minus|_|) = beginsWith (fun c -> c = '-')
let (|Times|_|) = beginsWith (fun c -> c = '*')
let (|Divide|_|) = beginsWith (fun c -> c = '/')
let (|LParen|_|) = beginsWith (fun c -> c = '(')
let (|RParen|_|) = beginsWith (fun c -> c = ')')

let rec tokenize = function
    | WhiteSpace(_, rest) -> tokenize rest
    | Numeral(x, rest) -> NUMERAL(x)::tokenize rest
    | Plus(_, rest) -> PLUS::tokenize rest
    | Minus(_, rest) -> MINUS::tokenize rest
    | Times(_, rest) -> TIMES::tokenize rest
    | Divide(_, rest) -> DIVIDE::tokenize rest
    | LParen(_, rest) -> LPAREN::tokenize rest
    | RParen(_, rest) -> RPAREN::tokenize rest
    | [] -> []
    | unknown -> raise(exn(sprintf "Oh no '%s'" (toString unknown)))

open Calc.Syntax

// let (|Op|_|) = function
//     | DIVIDE::rest -> Some(DIVIDE, rest)
//     | MINUS::rest  -> Some(MINUS, rest)
//     | PLUS::rest   -> Some(PLUS, rest)
//     | TIMES::rest  -> Some(TIMES, rest)
//     | _      -> None

let rec (|Expr|ExprErr|) = function
    | Term(n, PLUS::Expr(e, rest)) -> Expr(Plus(n, e), rest)
    | Term(n, rest) -> Expr(n, rest)
    | x -> ExprErr(x)
and (|Term|TermErr|) = function
    | Factor(e1, TIMES::Term(e2, rest)) -> Term(Times(e1, e2), rest)
    | x -> TermErr(x)
and (|Factor|FactorErr|) = function
    | LPAREN::Expr(e, RPAREN::rest) -> Factor(e, rest)
    | Num(n)::rest -> Factor(n, rest)
    | x -> FactorErr(x)
and (|Num|_|) = function
    | NUMERAL(n) -> Some(Numeral(n))
    | _ -> None