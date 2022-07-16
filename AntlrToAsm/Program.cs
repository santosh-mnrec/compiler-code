using System;
using System.IO;
using Antlr4.Runtime;

namespace AntlrToAsm
{
    class Program
    {
        public static String printProc2 = "myprint proc \n" +
               " cmp ax, 0\n" +
               " jge poz \n" +
               " not ax\n" +
               " add ax,1\n" +
               " push ax\n" +
               " mov dl, '-' ; print '-'\n" +
               " mov ah, 02h\n" +
               " int 21h   \n" +
               " pop ax\n" +
               "poz:\n" +
               " mov si,10d\n" +
               " xor dx,dx\n" +
               " push 10 ; newline\n" +
               " mov cx,1d\n" +
               "nonzero:\n" +
               " div si\n" +
               " add dx,48d\n" +
               " push dx\n" +
               " inc cx\n" +
               " xor dx,dx\n" +
               " cmp ax,0h\n" +
               " jne nonzero\n" +
               "writeloop:\n" +
               " pop dx\n" +
               " mov ah,02h\n" +
               " int 21h\n" +
               " dec cx\n" +
               " jnz writeloop \n" +
               " mov dl, 13 ; carriage return\n" +
               " mov ah, 02h ; \n" +
               " int 21h ;  \n" +
               "ret\n" +
               "endp\n";

        public static String handleDiv = "handlediv proc\n" +
                " cmp ax,0\n" +
                " jg axpos\n" +
                " xor dx, dx\n" +
                " cwd\n" +
                " idiv bx\n" +
                " ret\n" +
                "axpos:\n" +
                " cmp bx,0\n" +
                " jge bxpos\n" +
                " xor dx, dx\n" +
                " idiv bx\n" +
                " ret\n" +
                "bxpos:\n" +
                " xor dx, dx\n" +
                " div bx\n" +
                " ret\n";

        public static String printProc =
                "myprint proc\n" +
                        " mov si,10d\n" +
                        " xor dx,dx\n" +
                        " push 10 ; newline\n" +
                        " mov cx,1d\n" +
                        "nonzero:\n" +
                        " div si\n" +
                        " add dx,48d\n" +
                        " push dx\n" +
                        " inc cx\n" +
                        " xor dx,dx\n" +
                        " cmp ax,0h\n" +
                        " jne nonzero\n" +
                        "writeloop:\n" +
                        " pop dx\n" +
                        " mov ah,02h\n" +
                        " int 21h\n" +
                        " dec cx\n" +
                        " jnz writeloop \n" +
                        " mov dl, 13 ; carriage return\n" +
                        " mov ah, 02h ; \n" +
                        " int 21h ;  \n" +
                        "ret\n" +
                        "endp\n";

        public static String dosTerminate = ";dos terminate\nmov ah,0x4C\nint 0x21\n";
        static void Main(string[] args)
        {
            var input = new AntlrInputStream(File.ReadAllText("a.txt"));
            SimpleLexer lexer = new SimpleLexer(input);
            var tokens = new CommonTokenStream(lexer);
            SimpleParser parser = new SimpleParser(tokens);
            var tree = parser.prog(); // parse

            SimpleToA86Visitor v = new SimpleToA86Visitor();
            var x= "org 100h\n";
                    x=x+v.Visit(tree);
                    x=x+dosTerminate;
                    x=x+"\n";
                    x=x+printProc2;
                    x=x+handleDiv;

            var it = v.variableDefs.GetEnumerator();
            String vardefs = "";
            while (it.Current != null)
            {
                var varName = it.MoveNext();
                vardefs = vardefs + (varName + " dw ?\n");
            }
                    x=x+vardefs;

            File.WriteAllText("out.asm", x);
           
        }
    }
}
