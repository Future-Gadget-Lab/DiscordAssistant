using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Assistant.Services
{
    public class LoggingService : IInitializable
    {
        public static readonly LogSeverity DefaultSeverity = LogSeverity.Info;

        private DiscordSocketClient _client;
        private LogConfig _config;
        private StreamWriter _logFile;

        public LoggingService(DiscordSocketClient client, AssistantConfig config) :
            this(client, config.Log) { }

        public LoggingService(DiscordSocketClient client, LogConfig config)
        {
            _client = client;
            _config = config;
        }

        public Task Initialize()
        {
            if (_config?.Path != null)
            {
                if (!Directory.Exists(_config.Path))
                    Directory.CreateDirectory(_config.Path);
                _logFile = new StreamWriter(Path.Combine(_config.Path, DateTime.Now.ToString("yyyy-dd-MM")), true);
                _logFile.AutoFlush = true;
            }
            _client.Log += LogAsync;
            return Task.CompletedTask;
        }

        public async Task LogAsync(LogMessage logMessage)
        {
            if (_config?.Severity.GetValueOrDefault(LogSeverity.Info) > logMessage.Severity)
                return;

            string message = logMessage.ToString();
            await Console.Out.WriteLineAsync(message);

            if (_logFile == null)
                return;

            await _logFile.WriteLineAsync(message);
        }
    }
}
