using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Assistant
{
    public class Assistant
    {
        private readonly DiscordSocketClient _client;
        private readonly AssistantConfig _config;
        private readonly IServiceProvider _services;

        public Assistant(AssistantConfig config)
        {
            DiscordSocketConfig clientConfig = new DiscordSocketConfig
            {
                LogLevel = config.LogSeverity ?? LogSeverity.Info
            };

            _config = config;
            _client = new DiscordSocketClient();
            _services = new ServiceCollection()
                .AddSingleton(_config)
                .AddSingleton(_client)
                .AddSingleton<CommandHandler>()
                .AddSingleton<IInitializable>(s => s.GetRequiredService<CommandHandler>())
                .BuildServiceProvider();
            _client.Log += Log;
        }

        public Task Log(LogMessage message)
        {
            if (message.Severity > _config.LogSeverity.GetValueOrDefault())
                return Task.CompletedTask;
            return Console.Out.WriteLineAsync(message.ToString());
        }

        public async Task StartAsync()
        {
            foreach (IInitializable service in _services.GetServices<IInitializable>())
                await service.Initialize();
            await _client.LoginAsync(TokenType.Bot, _config.Token);
            await _client.StartAsync();
        }

        public async Task StartAndBlock()
        {
            await StartAsync();
            await Task.Delay(-1);
        }
    }
}
