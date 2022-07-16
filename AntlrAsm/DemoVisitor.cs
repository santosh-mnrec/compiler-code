using System;
using Antlr4.Runtime.Misc;

namespace AntlrAsm
{
    public class DemoVisitor : MyLanguageV0CodeBaseVisitor<object>
    {


        private CodeBuilder _codeBuilder;
        //constuctor
        public DemoVisitor(){
            _codeBuilder = new CodeBuilder(this);

        }
        public override object VisitProgram(MyLanguageV0CodeParser.ProgramContext context)
        {
            // Console.WriteLine("%include \"asm_io.inc\"");
            // Console.WriteLine("segment .data");
            // foreach (var decl in context.declaration())
            // {
            //     VisitDeclaration(decl);
            // }
            // foreach (var stmt in context.statement())
            // {
            //     VisitStatement(stmt);
            // }
            _codeBuilder.GenerateHeader(context);

            var s=_codeBuilder.Build();
            System.Console.WriteLine(s);

            return null;
        }

        public override object VisitDeclaration(MyLanguageV0CodeParser.DeclarationContext context)
        {

            // Console.WriteLine("segment .text");
            // Console.WriteLine("\tglobal asm_main");
            // Console.WriteLine("asm_main:");
            // Console.WriteLine("\tenter 0,0");
            // Console.WriteLine("\tpusha");
            _codeBuilder.Decl(context);
            //visit statements


            return null;
        }
        public override object VisitExpression([NotNull] MyLanguageV0CodeParser.ExpressionContext context)
        {
            return base.VisitExpression(context);
        }

        public override object VisitStatement(MyLanguageV0CodeParser.StatementContext context)
        {
            // Console.WriteLine("\tpopa");
            // Console.WriteLine("\tmov eax,0");
            // Console.WriteLine("\tleave");
            // Console.WriteLine("\tret");
            _codeBuilder.GenerateStatemnt(context);

            // VisitChildren(context);
            return null;
        }
        public override object VisitAssignstmt(MyLanguageV0CodeParser.AssignstmtContext context)
        {
            // System.Console.WriteLine("Visting assigment");
            //  {Console.WriteLine("\tmov ["+context.GetText()+"], eax");} 
            _codeBuilder.tmov(context);

            return null;
        }
    }
}