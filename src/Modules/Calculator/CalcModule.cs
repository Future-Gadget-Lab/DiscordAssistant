using Discord.Commands;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace Assistant.Modules.Calculator
{
    [Group("calc")]
    public class CalcModule : ModuleBase
    {
        [Command]
        [Priority(-1)]
        public async Task Calc([Remainder]string expression)
        {
            try
            {
                double result = ExpressionSolver.SolveExpression(expression);
                await ReplyAsync(result.ToString());
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
            }
        }

        // TODO: show all quadrants
        [Command("graph")]
        public async Task Graph(int width, int height, [Remainder]string expression)
        {
            if (height * width > 1000000)
            {
                await ReplyAsync($"Please use a grid with an area less than or equal to 1,000,000.");
                return;
            }

            try
            {
                using Stream stream = await Task.Run(() =>
                {
                    using Bitmap graph = new Bitmap(width % 10 == 0 ? width + 1 : width, height % 10 == 0 ? height + 1 : height);
                    using Graphics graphics = Graphics.FromImage(graph);

                    for (int x = 0; x < graph.Width; x += width / 10)
                        graphics.DrawLine(new Pen(Color.Gray, 1), x, 0, x, graph.Height - 1);

                    for (int y = 0; y < graph.Width; y += height / 10)
                        graphics.DrawLine(new Pen(Color.Gray, 1), 0, y, graph.Width - 1, y);

                    for (double x = 0; x < graph.Height; x += 1)
                    {
                        double y = Math.Round(ExpressionSolver.SolveExpression(expression.Replace("x", x.ToString())));
                        if (y >= graph.Height || double.IsNaN(y)) continue;
                        DrawDot(graph, Color.Red, new Point((int)x, graph.Height - (int)y - 1), 4);
                    }

                    MemoryStream stream = new MemoryStream();
                    graph.Save(stream, ImageFormat.Png);
                    stream.Position = 0;
                    return stream;
                });

                await Context.Channel.SendFileAsync(stream, "Graph.png");
            }
            catch (ParseException e)
            {
                await ReplyAsync(e.Message);
            }
        }

        private static void DrawDot(Bitmap bitmap, Color color, Point center, int radius)
        {
            for (int x = -radius; x < radius; x++)
            {
                int height = (int)Math.Round(Math.Sqrt(radius * radius - x * x));

                for (int y = -height; y < height; y++)
                {
                    int xp = x + center.X;
                    int yp = y + center.Y;
                    if (xp < 0 || xp >= bitmap.Width || yp < 0 || yp >= bitmap.Height)
                        continue;
                    bitmap.SetPixel(xp, yp, Color.Red);
                }
            }
        }
    }
}
