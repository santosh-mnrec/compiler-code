grammar HelloAntlr;

/* This will be the entry point of our parser. */
greet
    :    'Hello' name SEMI
    ;

name:ID;
SEMI:';';
ID:[a-zA-z]+;
WS  
    :   [ \t\n\r]+->skip;