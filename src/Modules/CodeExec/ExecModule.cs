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
        // TOOD: configurable
        private static readonly string SnippetDirectory = "submitted_code";
        private static readonly List<ILanguage> Languages = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => typeof(ILanguage).IsAssignableFrom(t) && !t.IsInterface)
            .Select(t => (ILanguage)Activator.CreateInstance(t))
            .ToList();

        private static ILanguage GetLanguage(string name) =>
            Languages.FirstOrDefault(l => l.Name.ToLower() == name.ToLower() || l.Aliases.Any(a => a.ToLower() == name.ToLower()));

        private static string ContainerPath(string path) =>
            Path.Combine("/home", path).Replace('\\', '/');

        [Command]
        public async Task Exec([Remainder]CodeSnippet snippet)
        {
            ILanguage language = GetLanguage(snippet.Language);
            if (language == null)
            {
                await ReplyAsync("Unsupported language.");
                return;
            }
            await ReplyAsync(await RunAsync(language, snippet.Code));
        }

        private async Task<string> RunAsync(ILanguage lang, string code)
        {
            using DockerContainer container = await DockerContainer.CreateContainerAsync(lang.DockerImage);
            string sourceDirectory = Path.Combine(SnippetDirectory, Context.User.Id.ToString());
            string codeFile = Path.Combine(sourceDirectory, Path.GetRandomFileName() + $".{lang.Extension}");
            Directory.CreateDirectory(sourceDirectory);
            await container.ExecAsync($"mkdir -p {ContainerPath(sourceDirectory)}");
            await File.WriteAllTextAsync(codeFile, string.Format(lang.SourceFile, code));
            await container.CopyAsync(codeFile, ContainerPath(codeFile));

            foreach (string file in lang.Environment.Keys)
            {
                string filePath = Path.Combine(sourceDirectory, file);
                await File.WriteAllTextAsync(filePath, lang.Environment[file]);
                await container.CopyAsync(filePath, ContainerPath(filePath));
            }

            return await container.ExecOutAsync(string.Format(lang.RunCommand, ContainerPath(sourceDirectory)));
        }
    }
}
