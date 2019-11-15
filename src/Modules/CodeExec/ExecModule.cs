using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Assistant.Modules.CodeExec
{
    [Group("exec")]
    public class ExecModule : ModuleBase
    {
        private readonly string SnippetDirectory = "submitted_code";
        private static readonly string ContainerDirectory = "/home/submitted_code";
        private static readonly List<ILanguage> Languages = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => typeof(ILanguage).IsAssignableFrom(t) && !t.IsInterface)
            .Select(t => (ILanguage)Activator.CreateInstance(t))
            .ToList();

        public ExecModule(AssistantConfig config)
        {
            SnippetDirectory = config.SnippetPath ?? "submitted_code";
        }

        private static ILanguage GetLanguage(string name) =>
            Languages.FirstOrDefault(l => l.Name.ToLower() == name.ToLower() || l.Aliases.Any(a => a.ToLower() == name.ToLower()));

        private static Embed GetOutputEmbed(ILanguage lang, ProcessResult result)
        {
            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle($"{lang.Name} - Run Results")
                .AddField("Exit code", result.ExitCode)
                .WithColor(result.ExitCode == 0 ? Color.Green : Color.Red);

            if (!string.IsNullOrEmpty(result.StandardOutput))
                embed.AddField("Out", $"```\n{result.StandardOutput}```");

            if (!string.IsNullOrEmpty(result.StandardError))
                embed.AddField("Error", $"```\n{result.StandardError}```");

            return embed.Build();
        }

        [Command]
        public async Task Exec([Remainder]CodeSnippet snippet)
        {
            ILanguage language = GetLanguage(snippet.Language);
            if (language == null)
            {
                await ReplyAsync("Unsupported language.");
                return;
            }
            await ReplyAsync(embed: GetOutputEmbed(language, await RunAsync(language, snippet.Code)));
        }

        private async Task<ProcessResult> RunAsync(ILanguage lang, string code)
        {
            using DockerContainer container = await DockerContainer.CreateContainerAsync(lang.DockerImage);
            string localDir = Path.Combine(SnippetDirectory, Context.User.Id.ToString());
            string codeFile = Path.GetRandomFileName() + $".{lang.Extension}";
            string codeFilePath = Path.Combine(localDir, codeFile);
            Directory.CreateDirectory(localDir);
            await File.WriteAllTextAsync(codeFilePath, string.Format(lang.SourceFile, code));
            await container.CopyAsync(codeFilePath, Path.Combine(ContainerDirectory, codeFile));

            return await container.ExecAsync(string.Format(lang.RunCommand, ContainerDirectory));
        }
    }
}
