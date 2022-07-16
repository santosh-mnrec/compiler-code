using Antlr4.Runtime;
using TinyLanguage;
using System;
using System.Collections.Generic;

namespace TinyLanguage
{
   public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    args = new string[] { "input.txt" };
                }

                Console.WriteLine("parsing: " + args[0]);

                SimpleLanguageLexer lexer = new SimpleLanguageLexer(new AntlrFileStream(args[0]));
                SimpleLanguageParser parser = new SimpleLanguageParser(new CommonTokenStream(lexer));
                var tree = parser.parse();
                Scope scope = new Scope();
                var functions = new Dictionary<string, MyFunction>();
                EvalVisitor visitor = new EvalVisitor(scope, functions);
                visitor.Visit(tree);
                //System.Console.WriteLine(tree);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
        }
    }
}
