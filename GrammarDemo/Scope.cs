using System;
using System.Collections.Generic;
using System.Text;

public class Scope
{

    private Scope Parent { get; set; }
    private Dictionary<String, Value> variables;
    private bool isFunction;

    public Scope() : this(null, false)
    {
        // only for the global scope, the parent is null

    }

   public  Scope(Scope p, bool function)
    {
        Parent = p;
        variables = new Dictionary<string, Value>();
        isFunction = function;
    }

    public void assignParam(String var, Value value)
    {
        variables.Add(var, value);
    }

    public void assign(String var, Value value)
    {
        if (resolve(var, !isFunction) != null)
        {
            // There is already such a variable, re-assign it
            this.reAssign(var, value);
        }
        else
        {
            // A newly declared variable
            variables.Add(var, value);
        }
    }

    private bool isGlobalScope()
    {
        return Parent == null;
    }


    private void reAssign(String identifier, Value value)
    {
        if (variables.ContainsKey(identifier))
        {
            // The variable is declared in this scope
            variables.Add(identifier, value);
        }
        else if (Parent != null)
        {
            // The variable was not declared in this scope, so let
            // the parent scope re-assign it
            Parent.reAssign(identifier, value);
        }
    }

    public Value resolve(String var)
    {
        return resolve(var, true);
    }

    private Value resolve(String var, bool checkParent)
    {
        Value value = variables[var];
        if (value != null)
        {
            // The variable resides in this scope
            return value;
        }
        else if (checkParent && !isGlobalScope())
        {
            // Let the parent scope look for the variable
            return Parent.resolve(var, !Parent.isFunction);
        }
        else
        {
            // Unknown variable
            return null;
        }
    }


    public override String ToString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (KeyValuePair<string, Value> item in variables)
        {
            sb.Append(item.Key).Append("->").Append(item.Value).Append(",");
        }
        return sb.ToString();
    }
}