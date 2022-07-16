grammar Expression;
root: (stmt ';')*;
stmt: ID '=' expr # Assign | 'print' expr # Print;

expr:
	left = expr op = ('*' | '/') right = expr	# OpExpr
	| left = expr op = ('+' | '-') right = expr	# OpExpr
	| '(' expr ')' # ParenExpr
	| INT	# NumExpr
	| ID	# IdExpr;

INT: ('0' ..'9')+;
ID: [a-zA-Z]+;

WS: [ \t\r\n]+ -> skip;