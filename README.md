# Discord Assistant
This is a Discord bot whose job is to provide quick references
without needing to search it yourself.

## Hosting
After the bot is built, run it as you would any other application and make sure that there is a `config.json` file
in your current working directory. The config file should look similar to this:
```json
{
  "token": "xxxxxxxxxxxxxxxxxxxxxxxx.xxxxxx.xxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "prefix": "$",
  "log": {
    "severity": "info",
    "path": "logs",
    "ext": ".log"
  }
}
```
The fields are as follows:

Property | Purpose 
-------- | ------- 
token (required) | The [token](https://discordapp.com/developers/docs/topics/oauth2#bots) the bot will login with
prefix | The command prefix
log.severity | The [severity](https://discord.foxbot.me/stable/api/Discord.LogSeverity.html) a message must be to log
log.path | The path log files will go, no value means no log files
log.extension | The extension to suffix log files with (ex .log)
log.ext | Short name for log.extension
exec.memory | Max memory usage for containers
exec.cpus | Max CPU resources for containers
exec.timeout | Max time an exec command can take
