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
        private LogSeverity _severity;
        private string _logDir;
        private StreamWriter _logFile;

        public LoggingService(DiscordSocketClient client, AssistantConfig config)
        {
            _client = client;
            _severity = config.LogSeverity ?? DefaultSeverity;
            if (config.LogDir != null)
                _logDir = config.LogDir;
        }

        public Task Initialize()
        {
            if (_logDir != null)
            {
                if (!Directory.Exists(_logDir))
                    Directory.CreateDirectory(_logDir);
                _logFile = new StreamWriter(Path.Combine(_logDir, DateTime.Now.ToString("yyyy-dd-MM")), true);
                _logFile.AutoFlush = true;
            }
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
