
using System.Collections.Generic;
using System;
using System.Collections;

namespace TinyLanguage
{
#pragma warning disable S1210 // "Equals" and the comparison operators should be overridden when implementing "IComparable"
    public class Value : IComparable<Value>
#pragma warning restore S1210 // "Equals" and the comparison operators should be overridden when implementing "IComparable"
    {
        public static readonly Value NULL = new Value();
        public static readonly Value VOID = new Value();

        private readonly object value;

        public Value()
        {
            // private constructor: only used for NULL and VOID
            value = new object();
        }

        public Value(object input)
        {
            if (input == null)
            {
                throw new EvalException("v == null");
            }
            value = input;
            // only accept bool, list, number or string types
            if (!(Isbool() || isList() || IsNumber() || isString()))
            {
                throw new EvalException("invalid data type: " + input + " (" + input.GetType() + ")");
            }
        }

        public bool Asbool()
        {
            var result = false;
            if (value is bool)
            {
                result = (bool)value;
                //Do something
            }
            else
            {
                //It's not a bool
            }
            return result;
        }

        public int AsDouble()
        {
            return int.Parse(value.ToString());
        }

        public long AsLong()
        {
            return (long)value;
        }



        public List<Value> AsList()
        {
            return (List<Value>)value;
        }

        public string AsString()
        {
            return value.ToString();
        }



        public int CompareTo(Value that)
        {
            if (that is null)
            {
                throw new ArgumentNullException(nameof(that));
            }

            if (IsNumber() && that.IsNumber())
            {
                if (Equals(that))
                {
                    return 0;
                }
                else
                {
                    return AsDouble().CompareTo(that.AsDouble());
                }
            }
            else if (isString() && that.isString())
            {
                return AsString().CompareTo(that.AsString());
            }
            else
            {
                throw new EvalException("illegal expression: can't compare `" + this + "` to `" + that + "`");
            }
        }


        public override bool Equals(object o)
        {
            if (this == VOID || o == VOID)
            {
                _ = new EvalException("can't use VOID: " + this + " ==/!= " + o);
            }
            if (this == o)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }
            var that = (Value)o;
            if (IsNumber() && that.IsNumber())
            {
                var result = AsDouble() == that.AsDouble();
                return result;

            }
            else
            {
                return value.Equals(that.value);
            }
        }



        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public bool Isbool()
        {
            return value is bool;
        }

        public bool IsNumber()
        {
            return value is int;

        }

        public bool isList()
        {
            if (value == null) return false;
            return value is IList &&
                   value.GetType().IsGenericType &&
                   value.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        public bool isNull()
        {
            return this == NULL;
        }

        public bool isVoid()
        {
            return this == VOID;
        }

        public bool isString()
        {
            return value is string;
        }



        public override string ToString()
        {
            return isNull() ? "NULL" : isVoid() ? "VOID" : value.ToString();
        }
    }
}