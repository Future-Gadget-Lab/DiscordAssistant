using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assistant.Modules.Help
{
    [Group("help")]
    [Summary("Get help")]
    public class HelpModule : ModuleBase
    {
        private readonly CommandService _commands;

        public HelpModule(CommandService commands)
        {
            _commands = commands;
        }

        [Command]
        [Summary("View info about all available commands")]
        public Task Help()
        {
            EmbedBuilder embed = new EmbedBuilder();
            foreach (ModuleInfo module in _commands.Modules)
            {
                EmbedFieldBuilder field = new EmbedFieldBuilder();
                if (!string.IsNullOrEmpty(module.Summary))
                    field.Value = $"{module.Summary}\n";
                field.Name = $"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(module.Name)} ({string.Join(", ", module.Aliases)})";
                foreach (CommandInfo command in module.Commands)
                    field.Value += $"- {GetCommandSignature(command)}\n";
                embed.AddField(field);
            }
            return ReplyAsync(embed: embed.Build());
        }

        [Command]
        [Summary("Get help with a particular command")]
        public async Task Help([Remainder]string command)
        {
            SearchResult result = _commands.Search(command);
            if (!result.IsSuccess)
            {
                await ReplyAsync($"Could not find any commands matching '{command}'.");
                return;
            }

            EmbedBuilder embed = new EmbedBuilder();
            foreach (CommandMatch cmd in result.Commands)
            {
                if (!(await cmd.CheckPreconditionsAsync(Context)).IsSuccess) continue;
                EmbedFieldBuilder field = new EmbedFieldBuilder()
                    .WithName(cmd.Command.Name.ToLower());

                field.Value += $"**Signature**\n{GetCommandSignature(cmd.Command)}\n";
                field.Value += $"**Aliases**\n{string.Join(", ", cmd.Command.Aliases)}\n";
                field.Value += "**Summary**\n";
                if (!string.IsNullOrEmpty(cmd.Command.Summary))
                    field.Value += $"{cmd.Command.Summary}\n";
                else
                    field.Value += "None\n";

                field.Value += $"**Params**\n";
                if (cmd.Command.Parameters.Count > 0)
                {
                    foreach (ParameterInfo param in cmd.Command.Parameters)
                    {
                        field.Value += $"__{param.Name}__\n" +
                            $"Type: {param.Type.Name}\n" +
                            $"Optional: {param.IsOptional}\n" +
                            $"Remainder: {param.IsRemainder}\n";
                    }
                }
                else
                {
                    field.Value += "None\n";
                }
                
                embed.AddField(field);
            }

            await ReplyAsync(embed: embed.Build());
        }

        private static string GetCommandSignature(CommandInfo command) =>
            $"{GetShortestAlias(command.Aliases)} {GetParamaterDetails(command.Parameters)}";

        private static string GetParamaterDetails(IReadOnlyList<ParameterInfo> parameters)
        {
            StringBuilder details = new StringBuilder();
            foreach (ParameterInfo param in parameters)
            {
                if (param.IsOptional)
                    details.Append($"[{param.Name}: {param.Type.Name}] ");
                else
                    details.Append($"<{param.Name}: {param.Type.Name}> ");
            }
            return details.ToString();
        }

        private static string GetShortestAlias(IReadOnlyList<string> aliases)
        {
            if (aliases.Count < 1)
                throw new ArgumentException($"Cannot get alias from 0 length list.");
            string shortest = aliases.ElementAt(0);
            foreach (string alias in aliases.Skip(1))
            {
                if (alias.Length < shortest.Length)
                    shortest = alias;
            }
            return shortest;
        }
    }
}
