using Assistant.Services;
using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Discord.Commands;

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
                LogLevel = config.Log?.Severity ?? LoggingService.DefaultSeverity
            };

            _config = config;
            _client = new DiscordSocketClient();
            _services = new ServiceCollection()
                .AddSingleton(_config)
                .AddSingleton(_client)
                .AddSingleton(new CommandService(new CommandServiceConfig { DefaultRunMode = RunMode.Async }))
                .AddSingleton<Random>()
                .AddSingleton<HttpService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<LoggingService>()
                .AddSingleton<IInitializable>(s => s.GetRequiredService<CommandHandler>())
                .AddSingleton<IInitializable>(s => s.GetRequiredService<LoggingService>())
                .BuildServiceProvider();
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
