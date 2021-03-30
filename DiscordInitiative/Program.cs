using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordInitiative
{
    public class Program
    {
        public struct GlobalConfigStruct
        {
            public static string _botToken = "";
		}

        public static void SetArgs(string[] args)
        {
            if (args.Length > 0)
            {

                foreach (string arg in args)
                {
                    Console.WriteLine("Evaluating: " + arg);
                    if (arg.Contains("--token="))
                    {
                        GlobalConfigStruct._botToken = arg.Split("=")[1];
                    }

                }
            }
        }

		public static void Main(string[] args)
			=> new Program().MainAsync(args).GetAwaiter().GetResult();

		public async Task MainAsync(string[] args)
		{
			SetArgs(args);
			DiscordSocketClient client = new DiscordSocketClient();
			client.Log += Log;
			client.LoginAsync(TokenType.Bot, GlobalConfigStruct._botToken);
			client.StartAsync();
			await Task.Delay(-1);
		}

		private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}

	}

	public class LoggingService
    {
		public LoggingService(DiscordSocketClient client, CommandService command)
		{
			client.Log += LogAsync;
			command.Log += LogAsync;
		}
		private Task LogAsync(LogMessage message)
		{
			if (message.Exception is CommandException cmdException)
			{
				Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}"
					+ $" failed to execute in {cmdException.Context.Channel}.");
				Console.WriteLine(cmdException);
			}
			else
				Console.WriteLine($"[General/{message.Severity}] {message}");

			return Task.CompletedTask;
		}
	}


}
