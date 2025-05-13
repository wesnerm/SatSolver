# Congruence Closure Decision Procedure.

The program accepts formulas with the following syntactic constraints. 

* Formulas are conjunctions of literals
* Literals are applications of equality predicates
* The only predicates allowed are equality predicates.
* Terms can be variables or function applications
* Function applications must take at least 1 argument
 
The input formulas expect equality predicates and functions to be in infix format e.g `(f1(x1, x2) = x2)` or
 `(f1(x2, x1) != x4)`. Function constants are of the form `fN` and variables are of the form `xN`,
where N is a positive integer (i.e., `N > 0`).

The grammar is as follows:

```
formula
    : formula COMMA lequals
    | formula COMMA lnequals
    | lnequals
    | lequals
    ;


obj
    : FUNC LPAR objlist RPAR
    | VAR
    ;

lequals
    : LPAR  obj EQUALS obj RPAR
    ;

lnequals
    : LPAR  obj NEQUALS obj RPAR
    ;

objlist
    : obj COMMA objlist
    | obj
    ;


VAR
    : 'x' ('1' .. '9')('0' .. '9')*
    ;

FUNC
    : 'f' ('1' .. '9')('0'..'9')*
    ;

LPAR
    : '('
    ;

RPAR
    : ')'
    ;

COMMA
    : ','
    ;

EQUALS
    : '='
    ;

NEQUALS
    : '!='
    ;

WS
   : [ \r\n\t]+ -> skip
   ;
```