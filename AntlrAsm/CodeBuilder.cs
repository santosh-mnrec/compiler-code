using System.Text;
using static MyLanguageV0CodeParser;
using System.IO;

namespace AntlrAsm
{
    public class CodeBuilder
    {
        private StreamWriter _sb;
        private int _indent;
        private MyLanguageV0CodeBaseVisitor<object> _visitor;

        //constructor
        public CodeBuilder(MyLanguageV0CodeBaseVisitor<object> visitor)
        {
            _sb = new StreamWriter(@"demo.in",false);
            _indent = 0;
            _visitor = visitor;
        }
        // generate assignment assembly code

        //tmov ["+context.GetText()+"], eax

        public CodeBuilder GenerateHeader(ProgramContext context)
        {
            _sb.WriteLine("%include \"asm_io.inc\"");
            _sb.WriteLine("segment .data");
            // foreach (var decl in context.declaration())
            // {
            //     _visitor.VisitDeclaration(decl);
            // }
            // foreach (var stmt in context.statement())
            // {
            //     _visitor.VisitStatement(stmt);
            // }
            _visitor.VisitChildren(context);

            return this;
        }
        public CodeBuilder tmov(AssignstmtContext s)
        {
            _sb.WriteLine("\tmov" + s);
            return this;
        }

        public CodeBuilder Decl(DeclarationContext context)
        {
            _sb.WriteLine("segment .text");
            _sb.WriteLine("\tglobal asm_main");
            _sb.WriteLine("asm_main:");
            _sb.WriteLine("\tenter 0,0");
            _sb.WriteLine("\tpusha");
            return this;
        }
        public CodeBuilder GenerateStatemnt(StatementContext context)
        {
            _sb.WriteLine("\tpopa");
            _sb.WriteLine("\tmov eax,0");
            _sb.WriteLine("\tleave");
            _sb.WriteLine("\tret");
            _visitor.VisitChildren(context);
            return this;
        }

        public string Build()
        {
            _sb.Flush();
            _sb.Close();
            
            return _sb.ToString();
        }



    }
}