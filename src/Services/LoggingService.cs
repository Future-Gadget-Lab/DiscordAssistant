using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Assistant.Services
{
    public class LoggingService : IInitializable
    {
        public static readonly LogSeverity DefaultSeverity = LogSeverity.Info;

        private DiscordSocketClient _client;
        private LogSeverity _severity;
        private StreamWriter _logFile;

        public LoggingService(DiscordSocketClient client, AssistantConfig config)
        {
            _client = client;
            _severity = config.LogSeverity ?? DefaultSeverity;
            if (config.LogFile != null)
            {
                _logFile = new StreamWriter(config.LogFile, true);
                _logFile.AutoFlush = true;
            }
        }

        public Task Initialize()
        {
            _client.Log += LogAsync;
            return Task.CompletedTask;
        }

        public async Task LogAsync(LogMessage logMessage)
        {
            if (_severity > logMessage.Severity)
                return;

            string message = logMessage.ToString();
            await Console.Out.WriteLineAsync(message);

            if (_logFile == null)
                return;

            await _logFile.WriteLineAsync(message);
        }
    }
}
