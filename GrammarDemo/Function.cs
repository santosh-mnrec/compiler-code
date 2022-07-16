
using System;
using System.Collections.Generic;
using Antlr4.Runtime.Tree;

public class MyFunction
{

    private Scope parentScope;
    private List<ITerminalNode> @params;
    private IParseTree block;

   public MyFunction(Scope parentScope, List<ITerminalNode> @params, IParseTree block)
    {
        this.parentScope = parentScope;
        this.@params = @params;
        this.block = block;
    }

    public Value invoke(List<Value> args, Dictionary<String, MyFunction> functions)
    {
        if (args.Count != this.@params.Count)
        {
            throw new System.Exception("Illegal Function call");
        }
        Scope scopeNext = new Scope(parentScope, true); // create function scope

        for (int i = 0; i < this.@params.Count; i++)
        {
            Value value = args[i];
            scopeNext.assignParam(this.@params[i].GetText(), value);
        }
           EvalVisitor evalVistorNext = new EvalVisitor(scopeNext,functions);
        //EvalVisitor evalVistorNext = new EvalVisitor();


        Value ret = Value.VOID;
        try
        {
            evalVistorNext.Visit(this.block);
        }
        catch (ReturnValue returnValue)
        {
            ret = returnValue.Value;
        }
        return ret;
    }
}
