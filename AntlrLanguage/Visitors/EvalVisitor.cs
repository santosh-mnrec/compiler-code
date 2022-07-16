
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using TinyLanguage;
using System;
using System.Collections.Generic;
using System.Linq;

using static TinyLanguage.SimpleLanguageParser;


namespace TinyLanguage
{
    public class EvalVisitor : SimpleLanguageBaseVisitor<Value>
    {

        // used to compare floating point numbers
        private static double SMALL_VALUE = 0.00000000001;
        private static  ReturnValue returnValue = new ReturnValue();
        private Scope scope;
        private  static Dictionary<string, MyFunction> functions =new Dictionary<string, MyFunction>();

        public EvalVisitor(Scope scope, Dictionary<string, MyFunction> functions)
        {
            this.scope = scope;
            
        }
        public override Value VisitStringExpression([NotNull] StringExpressionContext context)
        {
            var text = context.GetText();
            text = text.Substring(1, text.Length - 1).Replace("\\(.)", "$1").Replace("\"","");
            var val = new Value(text);
            if (context.indexes() != null)
            {
                var exps = context.indexes().expr();
                val = resolveIndexes(val, exps);
            }
            return val;
        }
        // Identifier indexes?                      #identifierExpression
        public override Value VisitIdentifierExpression([NotNull] IdentifierExpressionContext context)
        {
            string id = context.ID().GetText();
            var val = scope.resolve(id);

            if (context.indexes() != null)
            {
                var exps = context.indexes().expr();
                val = resolveIndexes(val, exps);
            }
            return val;
        }
        // '(' expression ')' indexes?              #expressionExpression
        public override Value VisitExpressionExpression([NotNull] ExpressionExpressionContext context)
        {
            var val = Visit(context.expr());
            if (context.indexes() != null)
            {
                var exps = context.indexes().expr();
                val = resolveIndexes(val, exps);
            }
            return val;
        }
        // Print '(' expression ')'     #printFunctionCall
        public override Value VisitPrintlnFunctionCall([NotNull] PrintlnFunctionCallContext context)
        {
            Console.Write(this.Visit(context.expr()));
            return Value.VOID;
        }
        // functionDecl
        public override Value VisitFunctionDeclaration([NotNull] FunctionDeclarationContext context)
        {
            var @params = context.idList() != null ? context.idList().ID() : new List<ITerminalNode>().ToArray();
            var block = context.block();
            string id = context.ID().GetText() + @params.Count();
            // TODO: throw exception if function is already defined?
            functions.Add(id, new MyFunction(scope, @params.ToList(), block));
            return Value.VOID;
        }

        // Identifier '(' exprList? ')' #identifierFunctionCall
        public override Value VisitIdentifierFunctionCall([NotNull] IdentifierFunctionCallContext context)
        {
            var @params = context.exprList() != null ? context.exprList().expr() : new List<ExprContext>().ToArray();
            string id = context.ID().GetText() + @params.Count();
            MyFunction function;
            if (functions.TryGetValue(id, out function))
            {
                List<Value> args = new List<Value>();
                foreach (ExprContext param in @params)
                {
                    args.Add(this.Visit(param));
                }
                return function.Invoke(args, functions);
            }
            throw new EvalException(context.ToString());
        }
        // functionCall indexes?                    #functionCallExpression
        public override Value VisitFunctionCallExpression([NotNull] FunctionCallExpressionContext context)
        {
            var val = Visit(context.functionCall());
            if (context.indexes() != null)
            {
                var exps = context.indexes().expr();
                val = resolveIndexes(val, exps);
            }
            return val;
        }

        private Value resolveIndexes(Value val, ExprContext[] indexes)
        {
            foreach (var ec in indexes)
            {
                var idx = Visit(ec);
                if (!idx.IsNumber() || !val.isList() && !val.isString())
                {
                    throw new EvalException("Problem resolving indexes on " + val + " at " + idx);
                }
                int i = idx.AsDouble();
                if (val.isString())
                {
                    val = new Value(val.AsString().Substring(i, i + 1));
                }
                else
                {
                    val = val.AsList()[i];
                }
            }
            return val;
        }
        // list: '[' exprList? ']'
        public override Value VisitListExpression([NotNull] ListExpressionContext context)
        {
            var val = Visit(context.list());
            if (context.indexes() != null)
            {
                var exps = context.indexes().expr();
                val = resolveIndexes(val, exps);
            }
            return val;
        }
      
        // : Identifier indexes? '=' expression
        public override Value VisitAssignment([NotNull] AssignmentContext context)
        {
            var newVal = Visit(context.expr());
            if (context.indexes() != null)
            {
                var val = scope.resolve(context.ID().GetText());
                var exps = context.indexes().expr();
                setAtIndex(context, exps, val, newVal);
            }
            else
            {
                string id = context.ID().GetText();
                scope.Assign(id, newVal);
            }
            return Value.VOID;
        }

        private void setAtIndex(AssignmentContext context, ExprContext[] indexes, Value val, Value newVal)
        {
            if (!val.isList())
            {
                throw new EvalException(context.ToString());
            }
            for (int i = 0; i < indexes.Count() - 1; i++)
            {
                var idx1 = Visit(indexes[i]);
                if (!idx1.IsNumber())
                {
                    throw new EvalException(context.ToString());
                }
                val = val.AsList()[int.Parse(idx1.ToString())];


            }
            var idx = Visit(indexes[indexes.Count() - 1]);
            if (!idx.IsNumber())
            {
                throw new EvalException(context.ToString());
            }
            val.AsList()[int.Parse(idx.AsDouble().ToString())] = newVal;
        }



        public override Value VisitBoolExpression(BoolExpressionContext context)
        {
            return new Value(bool.Parse(context.GetText()));
        }
        public override Value VisitNumberExpression([NotNull] NumberExpressionContext context)
        {
            return new Value(int.Parse(context.GetText()));
        }


        public override Value VisitNullExpression([NotNull] NullExpressionContext context)
        {
            return Value.NULL;
        }


        // list: '[' exprList? ']'

        public override Value VisitList(ListContext context)
        {
            var list = new List<Value>();
            if (context.exprList() != null)
            {
                foreach (var ex in context.exprList().expr())
                {
                    list.Add(this.Visit(ex));
                }
            }
            return new Value(list);
        }

        public override Value VisitBlock([NotNull] BlockContext context)
        {
            scope = new Scope(scope, false); // create new local scope
            foreach (var fdx in context.functionDeclaration())
            {
                Visit(fdx);
            }
            foreach (var sx in context.stat())
            {
                Visit(sx);
            }
            ExprContext ex;
            if ((ex = context.expr()) != null)
            {
                returnValue.Value = Visit(ex);
                scope = scope.Parent;
                throw returnValue;
            }
            scope = scope.Parent;
            return Value.VOID;
        }

        // expr overrides



        public override Value VisitPowExpr(PowExprContext context)
        {
            Value left = Visit(context.expr(0));
            Value right = Visit(context.expr(1));
            return new Value(Math.Pow(left.AsDouble(), right.AsDouble()));
        }


        public override Value VisitUnaryMinusExpr(UnaryMinusExprContext context)
        {
            Value value = Visit(context.expr());
            return new Value(-value.AsDouble());
        }


        public override Value VisitNotExpr(NotExprContext context)
        {
            Value value = Visit(context.expr());
            return new Value(!value.Asbool());
        }


        public override Value VisitMultiplicationExpr(MultiplicationExprContext context)
        {

            Value left = Visit(context.expr(0));
            Value right = Visit(context.expr(1));

            switch (context.op.Text)
            {
                case "*":
                    return new Value(left.AsDouble() * right.AsDouble());
                case "/":
                    return new Value(left.AsDouble() / right.AsDouble());
                case "%":
                    return new Value(left.AsDouble() % right.AsDouble());
                default:
                    throw new EvalException("unknown operator: ");
            }
        }


        public override Value VisitAdditiveExpr(AdditiveExprContext context)
        {

          

            switch (context.op.Text)
            {
                case "+":
                    return Add(context);
                case "-":
                    return Subtract(context);
                default:
                    throw new EvalException("unknown operator type: " + context.op.GetType());
            }
        }

        private Value Subtract(AdditiveExprContext ctx)
        {
            var lhs = this.Visit(ctx.expr(0));
            var rhs = this.Visit(ctx.expr(1));
            if (lhs.IsNumber() && rhs.IsNumber())
            {
                return new Value(lhs.AsDouble() - rhs.AsDouble());
            }
            if (lhs.isList())
            {
                var list = lhs.AsList();
                list.Remove(rhs);
                return new Value(list);
            }
            throw new EvalException(ctx.ToString());
        }

        private Value Add(AdditiveExprContext context)
        {
            var lhs = this.Visit(context.expr(0));
            var rhs = this.Visit(context.expr(1));

            if (lhs == null || rhs == null)
            {
                throw new EvalException(context.ToString());
            }

            // number + number
            if (lhs.IsNumber() && rhs.IsNumber())
            {
                return new Value(lhs.AsDouble() + rhs.AsDouble());
            }

            // list + any
            if (lhs.isList())
            {
                var  list = lhs.AsList();
                list.Add(rhs);
                return new Value(list);
            }

            // string + any
            if (lhs.isString())
            {
                return new Value(lhs.AsString() + "" + rhs.ToString());
            }

            // any + string
            if (rhs.isString())
            {
                return new Value(lhs.ToString() + "" + rhs.AsString());
            }

            return new Value(lhs.ToString() + rhs.ToString());
        }

        public override Value VisitRelationalExpr(RelationalExprContext context)
        {

           

            switch (context.op.Text)
            {
                case "<":
                    return Lt(context);
                case "<=":
                    return gteq(context);
                case ">":
                    return Gt(context);
                case ">=":
                    return gteq(context);
                default:
                    throw new EvalException("unknown operator: ");
            }
        }
        private Value gteq(RelationalExprContext context)
        {
            var lhs = Visit(context.expr(0));
            var rhs = Visit(context.expr(1));
            return new Value(lhs.Equals(rhs));

        }
      
        private Value Gt(RelationalExprContext context)
        {
            var lhs = Visit(context.expr(0));
            var rhs = Visit(context.expr(1));
            if (lhs.IsNumber() && rhs.IsNumber())
            {
                return new Value(lhs.AsDouble() > rhs.AsDouble());
            }
            if (lhs.isString() && rhs.isString())
            {
                return new Value(lhs.AsString().CompareTo(rhs.AsString()) > 0);
            }
            throw new EvalException("context");
        }
        private Value Lt(RelationalExprContext context)
        {
            var lhs = Visit(context.expr(0));
            var rhs = Visit(context.expr(1));
            if (lhs.IsNumber() && rhs.IsNumber())
            {
                return new Value(lhs.AsDouble() < rhs.AsDouble());
            }
            if (lhs.isString() && rhs.isString())
            {
                return new Value(lhs.AsString().CompareTo(rhs.AsString()) < 0);
            }
            throw new EvalException("context");
        }
        public override Value VisitEqualityExpr(EqualityExprContext context)
        {

        

            switch (context.op.Text)
            {
                case "==":
                    return Eq(context);

                case "!=":
                    return NEq(context);
                default:
                    throw new EvalException("unknown operator: ");
            }
        }
        private Value Eq(EqualityExprContext context)
        {
            var lhs = Visit(context.expr(0));
            var rhs = Visit(context.expr(1));
            if (lhs == null)
            {
                throw new EvalException("context");
            }
            return new Value(lhs.Equals(rhs));
        }
        private  Value NEq(EqualityExprContext context)
        {
            Value left = Visit(context.expr(0));
            Value right = Visit(context.expr(1));
            return left.IsNumber() && right.IsNumber() ?
                    new Value(Math.Abs(left.AsDouble() - right.AsDouble()) >= SMALL_VALUE) :
                    new Value(!left.Equals(right));
        }

        public override Value VisitAndExpr(AndExprContext context)
        {
            Value left = Visit(context.expr(0));
            Value right = Visit(context.expr(1));
            return new Value(left.Asbool() && right.Asbool());
        }


        public override Value VisitOrExpr(OrExprContext context)
        {
            Value left = Visit(context.expr(0));
            Value right = Visit(context.expr(1));
            return new Value(left.Asbool() || right.Asbool());
        }

        // log override

        public override Value VisitLog(LogContext context)
        {
            Value value = Visit(context.expr());
            Console.WriteLine(value.ToString());
            return value;
        }

        // if override

        public override Value VisitIfStatement([NotNull] IfStatementContext context)
        {
            // if ...
            if (this.Visit(context.ifStat().expr()).Asbool())
            {
                return this.Visit(context.ifStat().block());
            }

            // else if ...
            for (int i = 0; i < context.elseIfStat().Count(); i++)
            {
                if (this.Visit(context.elseIfStat(i).expr()).Asbool())
                {
                    return this.Visit(context.elseIfStat(i).block());
                }
            }

            // else ...
            if (context.elseStat() != null)
            {
                return this.Visit(context.elseStat().block());
            }

            return Value.VOID;
        }
        // while override

        public override Value VisitWhile_stat(While_statContext context)
        {

            while (Visit(context.expr()).Asbool())
            {

                // evaluate the code block
                var result = Visit(context.block());

                if (result != Value.VOID)
                {
                    return result;
                }
            }

            return Value.VOID;
        }
    }
}