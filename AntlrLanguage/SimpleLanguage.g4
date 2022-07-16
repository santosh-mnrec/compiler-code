grammar SimpleLanguage;
parse: block EOF;

block: ( stat | functionDeclaration)* ( Return expr ';')?;

stat:
	assignment ';'
	| functionCall ';'
	| ifStatement
	| while_stat
	| log;
assignment: ID indexes? '=' expr;



functionCall:
	ID OPAR exprList? CPAR	# identifierFunctionCall
	| log OPAR expr? CPAR	# printlnFunctionCall;





ifStatement
 : ifStat elseIfStat* elseStat? End
 ;

ifStat
 : IF expr Do block
 ;

elseIfStat
 : ELSE IF expr Do block
 ;

elseStat
 : ELSE Do block
 ;
functionDeclaration: Def ID OPAR idList? CPAR block End;
while_stat: WHILE expr Do block End;
idList: ID ( ',' ID)*;
exprList: expr ( ',' expr)*;
log: LOG expr SCOL;

expr:
	expr POW <assoc = right> expr				# powExpr
	| MINUS expr								# unaryMinusExpr
	| NOT expr									# notExpr
	| expr op = (MULT | DIV | MOD) expr			# multiplicationExpr
	| expr op = (PLUS | MINUS) expr				# additiveExpr
	| expr op = (LTEQ | GTEQ | LT | GT) expr	# relationalExpr
	| expr op = (EQ | NEQ) expr					# equalityExpr
	| expr AND expr								# andExpr
	| expr OR expr								# orExpr
	| Number									# numberExpression
	| Bool										# boolExpression
	| Null										# nullExpression
	| functionCall indexes?						# functionCallExpression
	| list indexes?								# listExpression
	| ID indexes?								# identifierExpression
	| String indexes?							# stringExpression
	| '(' expr ')' indexes?						# expressionExpression;

list: '[' exprList? ']';

indexes: ( '[' expr ']')+;

Bool: 'true' | 'false';

Number: Int ( '.' Digit*)?;

OR: '||';
AND: '&&';
EQ: '==';
NEQ: '!=';
GT: '>';
LT: '<';
GTEQ: '>=';
LTEQ: '<=';
PLUS: '+';
MINUS: '-';
MULT: '*';
DIV: '/';
MOD: '%';
POW: '^';
NOT: '!';

SCOL: ';';
ASSIGN: '=';
OPAR: '(';
CPAR: ')';
OBRACE: '{';
CBRACE: '}';
Null: 'null';

NIL: 'nil';
IF: 'if';
ELSE: 'else';
WHILE: 'while';
LOG: 'log';
Return: 'return';
Def: 'dell';
End: 'end';
Do: 'do';

Println: 'println';
ID: [a-zA-Z_] [a-zA-Z_0-9]*;

Identifier: [a-zA-Z_] [a-zA-Z_0-9]*;

String:
	["] (~["\r\n\\] | '\\' ~[\r\n])* ["]
	| ['] ( ~['\r\n\\] | '\\' ~[\r\n])* ['];
Comment: ( '//' ~[\r\n]* | '/*' .*? '*/') -> skip;
Space: [ \t\r\n\u000C] -> skip;
fragment Int: [1-9] Digit* | '0';

fragment Digit: [0-9];