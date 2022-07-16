using System;

namespace AntlrDraw
{
    public  class DrawDisplayVisitor : DrawBaseVisitor<int>
    {
        bool[,] display;
        int x = 0, y = 0;
        int xSize, ySize;

      
    public override int VisitInit(DrawParser.InitContext ctx)
        {
            xSize = int.Parse(ctx.NUMBER(0).GetText());
            ySize = int.Parse(ctx.NUMBER(1).GetText());

            display = new bool[xSize,ySize];
            for (int i = 0; i < xSize; i++)
                for (int j = 0; j < ySize; j++)
                    display[i,j] = false;

            return 0;
        }


      
    public override int VisitUp(DrawParser.UpContext ctx)
        {
            if (y > 0) y = y - 1;

            return 0;
        }

      
    public override int VisitDown(DrawParser.DownContext ctx)
        {
            if (y < ySize - 1) y = y + 1;

            return 0;
        }

      
    public override int VisitRight(DrawParser.RightContext ctx)
        {
            if (x < xSize - 1) x = x + 1;

            return 0;
        }

      
    public override int VisitLeft(DrawParser.LeftContext ctx)
        {
            if (x > 0) x = x - 1;

            return 0;
        }

      
    public override int VisitDraw(DrawParser.DrawContext ctx)
        {
            display[x,y] = true;

            return 0;
        }

      
    public override int VisitDisplay(DrawParser.DisplayContext ctx)
        {
            for (int j = 0; j < ySize; j++)
            {
                for (int i = 0; i < xSize; i++)
                {
                    if (display[i,j] == true)
                        Console.Write('#');

                else
                       Console.Write("-");
                }
                System.Console.WriteLine();
            }

            return 0;
        }
    }
}