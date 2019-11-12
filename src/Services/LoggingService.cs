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
        private FileStream _logFile;

        public LoggingService(DiscordSocketClient client, AssistantConfig config)
        {
            _client = client;
            _severity = config.LogSeverity ?? DefaultSeverity;
            if (config.LogFile != null)
                _logFile = new FileStream(config.LogFile, FileMode.Append, FileAccess.Write);
        }

        public Task Initialize()
        {
            _client.Log += LogAsync;
            return Task.CompletedTask;
        }

        public async Task LogAsync(LogMessage message)
        {
            if (_severity > message.Severity)
                return;

            await Console.Out.WriteLineAsync(message.ToString());

            if (_logFile == null)
                return;

            await _logFile.WriteAsync(Encoding.UTF8.GetBytes(message.ToString()));
        }
    }
}
