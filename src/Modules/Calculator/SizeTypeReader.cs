using Discord.Commands;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Assistant.Modules.Calculator
{
    public class SizeTypeReader : TypeReader
    {
        private static readonly string FailMessage = "Invalid size provided, please provide a size in format HEIGHTxWIDTH or a single number for a square graph.";

        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            ReadOnlySpan<char> chars = input;
            int xPos = chars.IndexOf('x');
            if (xPos < 0)
            {
                if (!int.TryParse(chars, out int size))
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, FailMessage));
                return Task.FromResult(TypeReaderResult.FromSuccess(new Size(size, size)));
            }

            if (!int.TryParse(chars.Slice(0, xPos), out int height) || !int.TryParse(chars.Slice(xPos + 1), out int width))
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, FailMessage));
            return Task.FromResult(TypeReaderResult.FromSuccess(new Size(width, height)));
        }
    }
}
