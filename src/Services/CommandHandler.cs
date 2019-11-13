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
        private readonly IServiceProvider _services;

        public CommandHandler(IServiceProvider services)
        {
            _config = services.GetRequiredService<AssistantConfig>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _commands = services.GetService<CommandService>() ?? new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async
            });
            _services = services;
        }

        public async Task Initialize()
        {
            _client.MessageReceived += HandleCommand;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommand(SocketMessage socketMessage)
        {
            SocketUserMessage message = socketMessage as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;
            if (!(message.HasStringPrefix(_config.Prefix, ref argPos, StringComparison.OrdinalIgnoreCase) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            SocketCommandContext context = new SocketCommandContext(_client, message);

            using (context.Channel.EnterTypingState())
            {
                IResult result = await _commands.ExecuteAsync(context, argPos, _services);
                if (result.IsSuccess)
                    return;

                await message.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}
