using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Assistant.Services
{
    public class CommandHandler : IInitializable
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly AssistantConfig _config;
        private readonly LoggingService _logger;
        private readonly IServiceProvider _services;

        public CommandHandler(IServiceProvider services)
        {
            _config = services.GetRequiredService<AssistantConfig>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _logger = services.GetRequiredService<LoggingService>();
            _commands = services.GetService<CommandService>() ?? new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async,
            });
            _services = services;
        }

        public async Task Initialize()
        {
            _client.MessageReceived += HandleCommand;
            _commands.CommandExecuted += CommandExecuted;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task CommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (result.IsSuccess)
                return;

            if (result.Error >= CommandError.Exception)
            {
                await _logger.LogAsync(new LogMessage(LogSeverity.Error, "CommandHandler", $"An exception or runtime error was encountered: {result.ErrorReason}"));
                return;
            }
            await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        private async Task HandleCommand(SocketMessage socketMessage)
        {
            SocketUserMessage message = socketMessage as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;
            if (!((_config.Prefix != null && message.HasStringPrefix(_config.Prefix, ref argPos, StringComparison.OrdinalIgnoreCase)) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            SocketCommandContext context = new SocketCommandContext(_client, message);

            using (context.Channel.EnterTypingState())
            {
                await _commands.ExecuteAsync(context, argPos, _services);
            }
        }
    }
}
