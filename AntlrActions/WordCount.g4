grammar WordCount;

@header {
/* If we have package declarations, it should go here. */
using System;
using Antlr4.Runtime;

}

@members {
  public int wordcount = 0;
  public int linecount = 0;

  public static void Main(string[] args) {
    var input = new AntlrInputStream("Hello world\nIt's a lexer.\n");
    Token t;
    var lexer = new WordCountLexer(input);
    while((t=lexer.NextToken() != Token.EOF_TOKEN && t.GetType() >= 0)
    {
    ;
    }

   Console.WriteLine("Total words:" + lexer.wordcount);
   Console.WriteLine("Total lines:" + lexer.linecount);
  }
}


WORD: LETTER+ {this.wordcount ++;};

WS: (' ' | '\t')+ ->skip;

NL: '\n' {this.linecount ++;};

fragment LETTER: ~(' ' | '\t' | '\n');