using Discord.Commands;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Assistant.Modules.CodeExec
{
    public class CodeSnippet
    {
        public string Language { get; }
        public string Code { get; }

        public CodeSnippet(string language, string code)
        {
            Language = language;
            Code = code;
        }
    }

    public class CodeSnippetTypeReader : TypeReader
    {
        private static readonly Regex SnippetRegex = new Regex("^```([A-Za-z]*)\\n(.+)```$", RegexOptions.Singleline);
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            if (!SnippetRegex.IsMatch(input))
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Invalid code snippet provided."));
            Match match = SnippetRegex.Match(input);
            return Task.FromResult(TypeReaderResult.FromSuccess(new CodeSnippet(match.Groups[1].Value, match.Groups[2].Value)));
        }
    }
}
