using System;
using System.Collections.Generic;
using System.Text;

namespace TinyLanguage
{
    public class Scope
    {

        public Scope Parent { get; set; }
        private readonly Dictionary<string, Value> variables;
        private readonly bool isFunction;

        public Scope() : this(null, false)
        {
            // only for the global scope, the parent is null

        }

        public Scope(Scope scope, bool function)
        {
            Parent = scope;
            variables = new Dictionary<string, Value>();

            isFunction = function;
        }

        public void AssignParam(string var, Value value)
        {
            variables.AddOrUpdate(var, value);
        }

        public void Assign(string var, Value value)
        {
            if (resolve(var, !isFunction) != null)
            {
                // There is already such a variable, re-assign it
                ReAssign(var, value);
            }
            else
            {
                // A newly declared variable
                variables.AddOrUpdate(var, value);
            }
        }

        private bool IsGlobal()
        {
            return Parent == null;
        }


        private void ReAssign(string identifier, Value value)
        {
            if (variables.ContainsKey(identifier))
            {
                // The variable is declared in this scope
                variables.AddOrUpdate(identifier, value);
            }
            else if (Parent != null)
            {
                // The variable was not declared in this scope, so let
                // the parent scope re-assign it
                Parent.ReAssign(identifier, value);
            }
        }

        public Value resolve(string var)
        {
            return resolve(var, true);
        }

        private Value resolve(string var, bool checkParent)
        {
            Value value;
            variables.TryGetValue(var, out value);
            if (value != null)
            {
                // The variable resides in this scope
                return value;
            }
            else if (checkParent && !IsGlobal())
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


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, Value> item in variables)
            {
                sb.Append(item.Key).Append("->").Append(item.Value).Append(",");
            }
            return sb.ToString();
        }
    }
}