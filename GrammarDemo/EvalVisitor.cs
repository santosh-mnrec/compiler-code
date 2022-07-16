
using Antlr4.Runtime;
using System.Collections.Generic;
using System;
using Antlr4.Runtime.Misc;
using static SimpleLanguageParser;
using Antlr4.Runtime.Tree;
using System.Linq;

public class EvalVisitor : SimpleLanguageBaseVisitor<Value>
{

    // used to compare floating point numbers
    public static double SMALL_VALUE = 0.00000000001;

    // store variables (there's only one global scope!)
    private Dictionary<String, Value> memory = new Dictionary<String, Value>();
    private static ReturnValue returnValue = new ReturnValue();
    private Scope scope;
    private Dictionary<String, MyFunction> functions;

   public EvalVisitor(Scope scope, Dictionary<String, MyFunction> functions)
    {
        this.scope = scope;
        this.functions = new Dictionary<string, MyFunction>();
    }
    // assignment/id overrides


    public override Value VisitFunctionDecl(FunctionDeclContext ctx)
    {
        var data = ctx.idList() != null ? ctx.idList().ID() : new List<ITerminalNode>().ToArray();
        var block = ctx.block();
        String id = ctx.ID().GetText() + data.Length.ToString();
        // TODO: throw exception if function is already defined?
        functions.Add(id, new MyFunction(scope, data.ToList(), block));
        return Value.VOID;
    }

    // list: '[' exprList? ']'

    public override Value VisitAssignment(SimpleLanguageParser.AssignmentContext ctx)
    {
        String id = ctx.ID().GetText();
        Value value = this.Visit(ctx.expr());
        var exists = memory.TryGetValue(id, out var value2);
        if (exists)
        {
            memory[id] = value;
        }
        else
        {
            memory.Add(id, value);
        }
        return value;
    }


    public override Value VisitIdAtom(SimpleLanguageParser.IdAtomContext ctx)
    {
        String id = ctx.GetText();
        memory.TryGetValue(id, out var value);
        if (value == null)
        {
            throw new Exception("no such variable: " + id);
        }
        return value;
    }

    // atom overrides

    public override Value VisitStringAtom(SimpleLanguageParser.StringAtomContext ctx)
    {
        String str = ctx.GetText();
        // strip quotes
        str = str.Substring(1, str.Length - 1).Replace("\"", "");
        return new Value(str);
    }


    public override Value VisitNumberAtom(SimpleLanguageParser.NumberAtomContext ctx)
    {
        return new Value(Double.Parse(ctx.GetText()));
    }


    public override Value VisitBooleanAtom(SimpleLanguageParser.BooleanAtomContext ctx)
    {
        return new Value(Boolean.Parse(ctx.GetText()));
    }


    public override Value VisitNilAtom(SimpleLanguageParser.NilAtomContext ctx)
    {
        return new Value(null);
    }

    // expr overrides

    public override Value VisitParExpr(SimpleLanguageParser.ParExprContext ctx)
    {
        return this.Visit(ctx.expr());
    }


    public override Value VisitPowExpr(SimpleLanguageParser.PowExprContext ctx)
    {
        Value left = this.Visit(ctx.expr(0));
        Value right = this.Visit(ctx.expr(1));
        return new Value(Math.Pow(left.asDouble(), right.asDouble()));
    }


    public override Value VisitUnaryMinusExpr(SimpleLanguageParser.UnaryMinusExprContext ctx)
    {
        Value value = this.Visit(ctx.expr());
        return new Value(-value.asDouble());
    }


    public override Value VisitNotExpr(SimpleLanguageParser.NotExprContext ctx)
    {
        Value value = this.Visit(ctx.expr());
        return new Value(!value.asBoolean());
    }


    public override Value VisitMultiplicationExpr(@SimpleLanguageParser.MultiplicationExprContext ctx)
    {

        Value left = this.Visit(ctx.expr(0));
        Value right = this.Visit(ctx.expr(1));

        switch (ctx.op.Text)
        {
            case "*":
                return new Value(left.asDouble() * right.asDouble());
            case "/":
                return new Value(left.asDouble() / right.asDouble());
            case "%":
                return new Value(left.asDouble() % right.asDouble());
            default:
                throw new Exception("unknown operator: ");
        }
    }


    public override Value VisitAdditiveExpr(SimpleLanguageParser.AdditiveExprContext ctx)
    {

        Value left = this.Visit(ctx.expr(0));
        Value right = this.Visit(ctx.expr(1));

        switch (ctx.op.Text)
        {
            case "+":
                return left.isDouble() && right.isDouble() ?
                        new Value(left.asDouble() + right.asDouble()) :
                        new Value(left.asString() + right.asString());
            case "-":
                return new Value(left.asDouble() - right.asDouble());
            default:
                throw new Exception("unknown operator: ");
        }
    }


    public override Value VisitRelationalExpr(SimpleLanguageParser.RelationalExprContext ctx)
    {

        Value left = this.Visit(ctx.expr(0));
        Value right = this.Visit(ctx.expr(1));

        switch (ctx.op.Text)
        {
            case "<":
                return new Value(left.asDouble() < right.asDouble());
            case "<=":
                return new Value(left.asDouble() <= right.asDouble());
            case ">":
                return new Value(left.asDouble() > right.asDouble());
            case ">=":
                return new Value(left.asDouble() >= right.asDouble());
            default:
                throw new Exception("unknown operator: ");
        }
    }


    public override Value VisitEqualityExpr(SimpleLanguageParser.EqualityExprContext ctx)
    {

        Value left = this.Visit(ctx.expr(0));
        Value right = this.Visit(ctx.expr(1));

        switch (ctx.op.Text)
        {
            case "==":
                return left.isDouble() && right.isDouble() ?
                        new Value(Math.Abs(left.asDouble() - right.asDouble()) < SMALL_VALUE) :
                        new Value(left.Equals(right));
            case "!=":
                return left.isDouble() && right.isDouble() ?
                        new Value(Math.Abs(left.asDouble() - right.asDouble()) >= SMALL_VALUE) :
                        new Value(!left.Equals(right));
            default:
                throw new Exception("unknown operator: ");
        }
    }


    public override Value VisitAndExpr(SimpleLanguageParser.AndExprContext ctx)
    {
        Value left = this.Visit(ctx.expr(0));
        Value right = this.Visit(ctx.expr(1));
        return new Value(left.asBoolean() && right.asBoolean());
    }


    public override Value VisitOrExpr(SimpleLanguageParser.OrExprContext ctx)
    {
        Value left = this.Visit(ctx.expr(0));
        Value right = this.Visit(ctx.expr(1));
        return new Value(left.asBoolean() || right.asBoolean());
    }

    // log override

    public override Value VisitLog(SimpleLanguageParser.LogContext ctx)
    {
        Value value = this.Visit(ctx.expr());
        System.Console.WriteLine(value.ToString());
        return value;
    }

    // if override

    public override Value VisitIf_stat(SimpleLanguageParser.If_statContext ctx)
    {

        var conditions = ctx.condition_block();

        var evaluatedBlock = false;

        foreach (SimpleLanguageParser.Condition_blockContext condition in conditions)
        {

            Value evaluated = this.Visit(condition.expr());

            if (evaluated.asBoolean())
            {
                evaluatedBlock = true;
                // evaluate this block whose expr==true
                this.Visit(condition.stat_block());
                break;
            }
        }

        if (!evaluatedBlock && ctx.stat_block() != null)
        {
            // evaluate the else-stat_block (if present == not null)
            this.Visit(ctx.stat_block());
        }

        return Value.VOID;
    }

    // while override

    public override Value VisitWhile_stat(SimpleLanguageParser.While_statContext ctx)
    {

        Value value = this.Visit(ctx.expr());

        while (value.asBoolean())
        {

            // evaluate the code block
            this.Visit(ctx.stat_block());

            // evaluate the expression
            value = this.Visit(ctx.expr());
        }

        return Value.VOID;
    }
}

public class Value
{

    public static Value VOID = new Value(new Object());

    Object value;

    public Value(Object v)
    {
        value = v;
    }

    public Boolean asBoolean()
    {
        return ((Boolean)(this.value));
    }

    public Double asDouble()
    {
        return ((Double)(this.value));
    }

    public String asString()
    {
        return new String(this.value.ToString());
    }

    public bool isDouble()
    {
        return (this.value is Double);
    }

    public override bool Equals(object o)
    {
        if (value == o)
        {
            return true;
        }

        if (value == null || o == null || o.GetType() != value.GetType())
        {
            return false;
        }

        Value that = (Value)o;

        return this.value.Equals(that.value);
    }
    public override string ToString()
    {
        return value.ToString();
    }
    // override object.GetHashCode
    public override int GetHashCode()
    {
        if (value == null)
        {
            return 0;
        }

        return this.value.GetHashCode();
    }



}
