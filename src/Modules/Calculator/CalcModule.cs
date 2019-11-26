using Assistant.Modules.Calculator.Expressions;
using Discord.Commands;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace Assistant.Modules.Calculator
{
    [Group("calculate"), Alias("calc")]
    [Summary("Do math stuff")]
    public class CalcModule : ModuleBase
    {
        [Command, Priority(-1)]
        [Summary("Solve a mathematical expression")]
        public async Task Calc([Remainder]string expression)
        {
            try
            {
                double result = ExpressionSolver.SolveExpression(expression);
                await ReplyAsync(result.ToString());
            }
            catch (ParseException e)
            {
                await ReplyAsync(e.Message);
            }
        }

        [Command("graph")]
        [Summary("Graph a given y-intercept formula")]
        public async Task Graph(Size size, [Remainder]string expression)
        {
            int height = size.Height;
            int width = size.Width;

            if (height * width > 1000000)
            {
                await ReplyAsync($"Please use a grid with an area less than or equal to 1,000,000.");
                return;
            }
            else if (width < 100 || height < 100)
            {
                await ReplyAsync($"Graphs must have a width and height of at least 100");
                return;
            }

            try
            {
                using Stream stream = await Task.Run(() =>
                {
                    using Bitmap graph = new Bitmap(width % 10 == 0 ? width + 1 : width, height % 10 == 0 ? height + 1 : height);
                    using Graphics graphics = Graphics.FromImage(graph);
                    graphics.Clear(Color.White);

                    for (int x = 0; x < graph.Width; x += width / 10)
                        graphics.DrawLine(new Pen(Color.Gray, 1), x, 0, x, graph.Height - 1);

                    for (int y = 0; y < graph.Width; y += height / 10)
                        graphics.DrawLine(new Pen(Color.Gray, 1), 0, y, graph.Width - 1, y);

                    ExpressionSolver solver = new ExpressionSolver(expression, true);
                    IExpression expr = solver.Parse();
                    for (double x = -graph.Width / 2; x < graph.Width / 2; x += 0.01)
                    {
                        solver.SetVariable('x', new Number(x));
                        double y = Math.Round(expr.Evaluate()) + graph.Height / 2;
                        int yPlot = graph.Height - (int)y - 1;
                        if (yPlot < 0 || yPlot >= graph.Height || double.IsNaN(y)) continue;
                        graph.SetPixel((int)x + graph.Width / 2, yPlot, Color.Red);
                    }

                    MemoryStream stream = new MemoryStream();
                    graph.Save(stream, ImageFormat.Png);
                    stream.Position = 0;
                    return stream;
                });


                // Using Discord namespace conflicts with System.Drawing classes
                Discord.Embed embed = new Discord.EmbedBuilder()
                    .WithTitle("Graphing Calculator")
                    .WithDescription($"Expression: y = {expression}\nSize: {height}x{width}")
                    .WithImageUrl("attachment://graph.png")
                    .Build();
                await Context.Channel.SendFileAsync(stream, "graph.png", embed: embed);
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
                    bitmap.SetPixel(xp, yp, color);
                }
            }
        }
    }
}
