grammar MyLanguageV0Code;

// Root non-terminal symbol A program is a bunch of declarations followed by a bunch of statements
// The Java code outputs the necessary NASM code around these declarations

program: declaration* statement*;

// Parse rule for variable declarations

declaration: INT a = NAME SEMICOLON;

// Parse rule for statements

statement: ifstmt | printstmt | assignstmt;

// Parse rule for if statements

ifstmt:
	IF LPAREN identifier EQUAL integer RPAREN statement* ENDIF;

// Parse rule for print statements

printstmt: PRINT term SEMICOLON;

// Parse rule for assignment statements

assignstmt: NAME ASSIGN expression SEMICOLON;

// Parse rule for expressions

expression: term | term PLUS term;

// Parse rule for terms

term: identifier | integer;

// Parse rule for identifiers

identifier: NAME;

// Parse rule for numbers 

integer: INTEGER;

// Reserved Keywords //////////////////////////////

IF: 'if';
ENDIF: 'endif';
PRINT: 'print';
INT: 'int';

// Operators
PLUS: '+';
EQUAL: '==';
ASSIGN: '=';
NOTEQUAL: '!=';

// Semicolon and parentheses
SEMICOLON: ';';
LPAREN: '(';
RPAREN: ')';

// Integers
INTEGER: [0-9][0-9]*;

// Variable names
NAME: [a-z]+;

// Ignore all white spaces 
WS: [ \t\r\n]+ -> skip;