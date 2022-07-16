using Antlr4.Runtime;
namespace ANTLRVisitor
{
class Program
    {
        static void Main(string[] args)
        {
            var expression = "n=5;print 2 * (3 + 5);";
            AntlrInputStream inputStream = new AntlrInputStream(expression);
            var lexer = new ExpressionLexer(inputStream);
            var parser = new ExpressionParser(new CommonTokenStream(lexer));
            var tree = parser.root();
            var answer = new EvalVisitor().Visit(tree);

        }
    }
}