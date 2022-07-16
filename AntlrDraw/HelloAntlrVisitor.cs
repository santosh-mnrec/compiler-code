using Antlr4.Runtime.Misc;
using static HelloAntlrParser;

namespace AntlrDraw
{
    public class HelloAntlrVisitor : HelloAntlrBaseVisitor<object>
    {

        
        public override object VisitGreet([NotNull] GreetContext context){

            System.Console.WriteLine(context.GetChild(1).Parent);
            return Visit(context.name());
            
        }

        public override object VisitName([NotNull] NameContext context){

            System.Console.WriteLine(context.Parent.GetType());
            return VisitChildren(context);
            
        }

    }
}