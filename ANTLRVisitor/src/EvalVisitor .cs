namespace ANTLRVisitor
{
    using System;
    using Antlr4.Runtime.Misc;

    public class EvalVisitor : ExpressionBaseVisitor<int>
    {
        private readonly Dictionary<string, int> _variables = new Dictionary<string, int>();
        public override int VisitRoot([NotNull] ExpressionParser.RootContext context)
        {

            return base.VisitRoot(context);
        }
        public override int VisitOpExpr([NotNull] ExpressionParser.OpExprContext context)
        {
            int left = Visit(context.left);
            int right = Visit(context.right);
            String op = context.op.Text;
            switch (op[0])
            {
                case '*': return left * right;
                case '/': return left / right;
                case '+': return left + right;
                case '-': return left - right;
                default: throw new ArgumentException("Unknown operator " + op);
            }
        }

        public override int VisitNumExpr([NotNull] ExpressionParser.NumExprContext context)
        {
            return int.Parse(context.GetText());
        }
        public override int VisitParenExpr([NotNull] ExpressionParser.ParenExprContext context)
        {
            return base.Visit(context.expr());
        }
        public override int VisitAssign([NotNull] ExpressionParser.AssignContext context)
        {
            _variables[context.ID().GetText()] = Visit(context.expr());
            return Visit(context.expr());

        }

        public override int VisitIdExpr([NotNull] ExpressionParser.IdExprContext context)
        {
            return _variables[context.ID().GetText()];
        }

   
        public override int VisitPrint([NotNull] ExpressionParser.PrintContext context)
        {
            Console.WriteLine(Visit(context.expr()));
            return 0;

        }
    }

}