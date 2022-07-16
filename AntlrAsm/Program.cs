using System;
using System.IO;
using Antlr4.Runtime;

namespace AntlrAsm
{
    class Program
    {
        static void Main(string[] args)
        {
            var expression = File.ReadAllText("in.txt");
            var inputStream = new AntlrInputStream(expression);
            var lexer = new MyLanguageV0CodeLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new MyLanguageV0CodeParser(tokenStream);


            var query = parser.program();
            var visitor = new DemoVisitor();
            visitor.Visit(query);

            System.Console.WriteLine(query.ToStringTree(parser));

        }
    }
}
