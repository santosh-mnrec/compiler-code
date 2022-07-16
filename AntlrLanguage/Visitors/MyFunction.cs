using System;
using System.Collections.Generic;
using Antlr4.Runtime.Tree;

namespace TinyLanguage
{
    public class MyFunction
    {

        private  Scope parentScope;
        private  List<ITerminalNode> @params;
        private  IParseTree block;

        public MyFunction(Scope parentScope, List<ITerminalNode> @params, IParseTree block)
        {
            this.parentScope = parentScope;
            this.@params = @params;
            this.block = block;
        }

        public Value Invoke(List<Value> args, Dictionary<string, MyFunction> functions)
        {
            if (args.Count != @params.Count)
            {
                throw new EvalException("Illegal Function call");
            }
            Scope scopeNext = new Scope(parentScope, true); // create function scope

            for (int i = 0; i < @params.Count; i++)
            {
                Value value = args[i];
                scopeNext.AssignParam(@params[i].GetText(), value);
            }
            EvalVisitor evalVistorNext = new EvalVisitor(scopeNext, functions);
          


            Value ret = Value.VOID;
            try
            {
                evalVistorNext.Visit(block);
            }
            catch (ReturnValue returnValue)
            {
                ret = returnValue.Value;
            }
            return ret;
        }
    }
}