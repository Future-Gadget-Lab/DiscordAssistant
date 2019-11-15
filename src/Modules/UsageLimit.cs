using Assistant.Services;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace Assistant.Modules
{
    public class UsageLimitAttribute : PreconditionAttribute
    {
        private readonly ConcurrentDictionary<ulong, int> _usages = new ConcurrentDictionary<ulong, int>();
        private readonly int _limit;
        private readonly int _time;

        public UsageLimitAttribute(int seconds, int limit)
        {
            _limit = limit;
            _time = seconds;
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (_usages.GetValueOrDefault(context.User.Id, 0) >= _limit)
                return Task.FromResult(PreconditionResult.FromError("You're using this command too frequently."));
            _usages.AddOrUpdate(context.User.Id, (id) =>
            {
                Timer timer = new Timer();
                timer.Interval = _time * 1000;
                timer.Elapsed += async (object source, ElapsedEventArgs e) =>
                {
                    if (!_usages.TryRemove(context.User.Id, out int usages))
                        await services.GetRequiredService<LoggingService>().LogAsync(new LogMessage(LogSeverity.Warning, "UsageLimit", "Remove failed."));
                };
                timer.Enabled = true;
                timer.AutoReset = false;
                return 1;
            }, (id, usage) => usage + 1);
            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
