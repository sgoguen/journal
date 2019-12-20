module Calc.Syntax

(** Abstract syntax. *)

(** Arithmetical expressions. *)
type Expression =
  | Numeral of int (** non-negative integer constant *)
  | Plus of Expression * Expression  (** Addition [e1 + e2] *)
  | Minus of Expression * Expression (** Difference [e1 - e2] *)
  | Times of Expression * Expression (** Product [e1 * e2] *)
  | Divide of Expression * Expression (** Quotient [e1 / e2] *)
  | Negate of Expression (** Opposite value [-e] *)

module Expression = 

    (** Conversion of expresions to strings. *)
    let toString e =
      let rec toStrRec n e =
        let (m, str) = 
            match e with
              | Numeral n       ->    (3, string(n))
              | Negate e        ->    (2, "-" + (toStrRec 0 e))
              | Times (e1, e2)  ->    (1, (toStrRec 1 e1) + " * " + (toStrRec 2 e2))
              | Divide (e1, e2) ->    (1, (toStrRec 1 e1) + " / " + (toStrRec 2 e2))
              | Plus (e1, e2)   ->    (0, (toStrRec 0 e1) + " + " + (toStrRec 1 e2))
              | Minus (e1, e2)  ->    (0, (toStrRec 0 e1) + " - " + (toStrRec 1 e2))
        in
          if m < n then "(" + str + ")" else str
      in
        toStrRec (-1) e

    let rec eval = function
      | Numeral n -> n
      | Plus (e1, e2) -> eval e1 + eval e2
      | Minus (e1, e2) -> eval e1 - eval e2
      | Times (e1, e2) -> eval e1 * eval e2
      | Divide (e1, e2) ->
          let n2 = eval e2 in
          eval e1 / n2  //  This will throw an exception if n2 is zero
      | Negate e -> - (eval e)