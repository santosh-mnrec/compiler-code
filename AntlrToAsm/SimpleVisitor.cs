using System;
using System.Collections.Generic;
using System.Text;

namespace AntlrToAsm
{
    public class SimpleToA86Visitor : SimpleBaseVisitor<string>
    {
        /**
         * As the tree is walked its necessary to keep track of variables.
         * When we see a ID type we check this set, if it is not here, we are dealing with an assigment statement.
         * Otherwise we create code to get the value of the variable.
         */
        public HashSet<string> variableDefs = new HashSet<string>();

        /**
         * Variable to keep track of labels corresponding to if and while statements.
         */
        public int labelIndex = 0;



        public override string VisitProg(SimpleParser.ProgContext ctx)
        {
            var stmts = ctx.stm();
            StringBuilder sb = new StringBuilder();
            foreach (SimpleParser.StmContext stmt in stmts)
            {
                var ret = Visit(stmt);
                sb.Append(ret);
            }
            return sb.ToString();
        }


        public override string VisitAssign(SimpleParser.AssignContext ctx)
        {
            String varName = ctx.ID().GetText();
            if (!variableDefs.Contains(varName))
                variableDefs.Add(varName);
            String code = "push offset " + ctx.ID().GetText() + "\n";
            code = code + Visit(ctx.expr());
            code = code + "pop ax\npop bx\nmov [bx],ax\n";
            return code;
        }


        public override string VisitInt(SimpleParser.IntContext ctx)
        {
            //return "push "+ctx.GetText()+"\n";
            return "mov ax, " + ctx.GetText() + "\npush ax\n";
        }


        public override string VisitId(SimpleParser.IdContext ctx)
        {
            return "push [" + ctx.GetText() + "]\n";
        }


        public override string VisitAdd(SimpleParser.AddContext ctx)
        {
            String code = Visit(ctx.expr(0));
            code = code + Visit(ctx.expr(1));
            code = code + "pop ax\npop bx\nadd ax,bx\npush ax\n";
            return code;
        }


        public override string VisitMul(SimpleParser.MulContext ctx)
        {
            String code = Visit(ctx.expr(0));
            code = code + Visit(ctx.expr(1));
            code = code + "pop ax\npop bx\nmul bx\npush ax\n";
            return code;
        }


        public override string VisitSub(SimpleParser.SubContext ctx)
        {
            String code = Visit(ctx.expr(0));
            code = code + Visit(ctx.expr(1));
            code = code + "pop bx\npop ax\nsub ax,bx\npush ax\n";
            return code;
        }


        public override string VisitDiv(SimpleParser.DivContext ctx)
        {
            var code = Visit(ctx.expr(0));
            code = code + Visit(ctx.expr(1));
            code = code + "pop bx\npop ax\ncall handlediv\npush ax\n";
            return code;
        }


        public override string VisitMod(SimpleParser.ModContext ctx)
        {
            String code = Visit(ctx.expr(0));
            code = code + Visit(ctx.expr(1));
            code = code + "pop bx\npop ax\nxor dx, dx\ndiv bx\npush dx\n";
            return code;
        }


        public override string VisitParens(SimpleParser.ParensContext ctx)
        {
            return Visit(ctx.expr());
        }


        public override string VisitPrint(SimpleParser.PrintContext ctx)
        {
            String code = Visit(ctx.expr());
            code = code + "pop ax\n";
            code = code + "call myprint\n";
            return code;
        }


        /**
         * Generates code according to this template:
         *
         * code for expr
         * pop ax
         * cmp ax,0
         * jz outlabel
         * code for stm
         * outlabel:
         * nop
         * @param ctx
         * @return a86 code for this if statement
         */

        public override string VisitIfthen(SimpleParser.IfthenContext ctx)
        {
            int currentLabelIndex = labelIndex;
            labelIndex = labelIndex + 1;
            String code = Visit(ctx.expr());
            code = code + "pop ax\ncmp ax,0\njz outlabel" + currentLabelIndex.ToString() + "\n";
            code = code + Visit(ctx.stm());
            code = code + "outlabel" + currentLabelIndex.ToString() + ":\nnop\n";
            return code;
        }



        /**
         * Generates code according to this template:
         *
         * whiletestlbl:
         *  nop
         *  code for expr
         *  pop ax
         *  cmp ax,0
         *  jz whilelbl
         *  code for stm
         *  jmp testlabel
         * whilelbl:
         *  nop
         * @param ctx
         * @return a86 code for this while statement
         */

        public override string VisitWhiledo(SimpleParser.WhiledoContext ctx)
        {
            int currentLabelIndex = labelIndex;
            String ci = (currentLabelIndex.ToString());
            labelIndex = labelIndex + 1;

            String code = "whiletestlbl" + ci + ":\nnop\n";
            code = code + Visit(ctx.expr());
            code = code + "pop ax\ncmp ax,0\njz whilelbl" + ci + "\n";
            code = code + Visit(ctx.stm());
            code = code + "jmp whiletestlbl" + ci + "\n" + "whilelbl" + ci + ":\nnop\n";
            return code;
        }


        public override string VisitBeginend(SimpleParser.BeginendContext ctx)
        {
            return Visit(ctx.opt_stmts());
        }


        public override string VisitOpt_stmts(SimpleParser.Opt_stmtsContext ctx)
        {
            return Visit(ctx.stmt_list());
        }


        public override string VisitStmt_list(SimpleParser.Stmt_listContext ctx)
        {
            var stmts = ctx.stm();
            StringBuilder sb = new StringBuilder();
            foreach (SimpleParser.StmContext stmt in stmts)
            {
                String ret = Visit(stmt);
                sb.Append(ret);
            }
            return sb.ToString();
        }

    }
}