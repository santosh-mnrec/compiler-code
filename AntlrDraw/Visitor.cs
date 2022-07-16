using System;

namespace AntlrDraw
{
    public interface IVisitor
    {

        int Visit(AdditionNode node);
        int Visit(SubtractionNode node);
        int Visit(MultiplicationNode node);
        //visit number
        int Visit(NumberNode node);

    }

    public class NumberNode : Expression
    {
        public int Value { get; set; }
        public NumberNode(int value)
        {
            Value = value;
        }
        public int Accept(IVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class MultiplicationNode : Expression
    {
        public Expression Left { get; set; }
        public Expression Right { get; set; }
        public MultiplicationNode(Expression left, Expression right)
        {
            Left = left;
            Right = right;
        }



        public int Accept(IVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class SubtractionNode
    {

        public Expression Left { get; set; }

        public SubtractionNode(Expression left, Expression right)
        {
            Left = left;
            Right = right;
        }

        public Expression Right { get; set; }

        public int Accept(IVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class AdditionNode
    {

        public Expression Left { get; set; }
        public Expression Right { get; set; }

        public AdditionNode(Expression right, Expression left)
        {
            Right = right;
            Left = left;
        }

        public int Accept(IVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }

    public interface Expression
    {
        int Accept(IVisitor visitor);

    }

    public class CalculatorVisitor : IVisitor
    {
        public int result = 0;
        public int Visit(AdditionNode node)
        {
            return node.Left.Accept(this) + node.Right.Accept(this);
        }

        public int Visit(SubtractionNode node)
        {
            return node.Left.Accept(this) - node.Right.Accept(this);
        }

        public int Visit(MultiplicationNode node)
        {
            return node.Left.Accept(this) * node.Right.Accept(this);
        }

        public int Visit(NumberNode node)
        {
            return node.Value;
        }
    }

    //print visitor
    public class PrintVisitor : IVisitor
    {
        public int Visit(AdditionNode node)
        {
            Console.WriteLine("+\t");
            return node.Left.Accept(this) + node.Right.Accept(this);
        }

        public int Visit(SubtractionNode node)
        {
            Console.Write("-\t ");
            return node.Left.Accept(this) - node.Right.Accept(this);
        }

        public int Visit(MultiplicationNode node)
        {
            Console.Write("*\t ");
            return node.Left.Accept(this) * node.Right.Accept(this);
        }

        public int Visit(NumberNode node)
        {
            Console.Write(node.Value + "->");
            return node.Value;
        }


    }
    
    

}