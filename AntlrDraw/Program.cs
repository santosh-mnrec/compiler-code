using System;
using System.IO;
using Antlr4.Runtime;

namespace AntlrDraw
{
    class Program
    {
        static void Main(string[] args)
        {
            String inputFile = File.ReadAllText("input.txt");





            //     var inputStream = new AntlrInputStream(inputFile);

            //     var lexer = new DrawLexer(inputStream);
            //     var tokens = new CommonTokenStream(lexer);
            //     var parser = new DrawParser(tokens);
            //     var tree = parser.prog();

            //     var drawV = new DrawDisplayVisitor();
            //     drawV.Visit(tree);
            //     var exp=new AdditionNode(
            //             new NumberNode(2),
            //             new MultiplicationNode(
            //             new NumberNode(3),new NumberNode(4)));
            //    System.Console.WriteLine(new PrintVisitor().Visit(exp));


            var inputStream = new AntlrInputStream("Hello amazon;");
            var lexer = new HelloAntlrLexer(inputStream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new HelloAntlrParser(tokens);
            var tree = parser.greet();
            var visitor = new HelloAntlrVisitor();
            var result = visitor.Visit(tree);
            System.Console.WriteLine(result);



        }
    }
}
