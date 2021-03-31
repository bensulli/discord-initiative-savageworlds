using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DiscordInitiative;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using DiscordInitiative.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;

namespace DiscordInitiative
{
    public class Deck
    {
        internal Random r = new Random();
        internal string Variant;
        internal List<Card> Cards = new List<Card>();

        public bool DeckExists { get; }

        public Deck(string deckVariant)
        {
            JObject o1 = JObject.Parse(File.ReadAllText("decks.json"));
            

            Variant = deckVariant;
                foreach (var deckObj in o1["decks"].Children())
                {
                    if (deckObj["deckName"]?.ToString() == Variant)
                    {
                        DeckExists = true;
                        foreach (var cardsObj in deckObj["cards"].Children())
                        {
                            Cards.Add(new Card((int)cardsObj["value"], cardsObj["name"]?.ToString(), cardsObj["suit"]?.ToString()));
                        }
                    }
                }

                if (!DeckExists)
                {
                    Console.WriteLine("Couldn't find the requested deck " + Variant + ". Sorry :(");
                }
        }

        public Card Draw()
        {
            int yourCard;
            while (true)
            {
                yourCard = r.Next(0, 53);
                if (!Cards[yourCard].Drawn)
                    break;
            }

            Cards[yourCard].Drawn = true;
            return Cards[yourCard];

        }

        public int RemainingCardCount()
        {
            int count = 0;
            foreach (var card in Cards)
            {
                if (!card.Drawn)
                    count++;
            }

            return count;
        }

        public void Shuffle()
        {

        }

    }
    public class Card
    {
        public Card(int value, string cardName, string suit)
        {
            this.Value = value;
            this.CardName = cardName;
            this.Suit = suit;
            this.SuitEmoji = this.Suit switch
            {
                "spades" => ":spades:",
                "hearts" => ":hearts:",
                "diamonds" => ":diamonds:",
                "clubs" => ":clubs:",
                _ => ":black_joker:"
            };
        }


        public string SuitEmoji { get; }
        public string Suit { get; }
        public string CardName { get; }
        public int Value { get; }

        public bool Drawn { get; set; }
    }

    public class Order
    {
        private static Dictionary<string, int> orderDictionary = new Dictionary<string, int>();
        public static void Add(string actorName,string actorType)
        {
            Card newCard = Program.Deck.Draw();
            orderDictionary.Add(Program.GlobalConfigStruct._lineFormat
                .Replace("[SUIT]", newCard.SuitEmoji)
                .Replace("[CARD]", newCard.CardName)
                .Replace("[CharacterName]", actorName)
                .Replace("[AlliedStatus]", actorType),newCard.Value);

        }

        public static List<String> GetOrderList()
        {
            List<String> orderList = new List<string>();
            var orderListDictionary = orderDictionary.ToList();
            orderListDictionary.Sort((pair, valuePair) => pair.Value.CompareTo(valuePair.Value));
            foreach (var row in orderListDictionary)
            {
                orderList.Insert(0,row.Key.ToString() + " (" + row.Value + ")");
            }

            return orderList;
        }

    }

    public class Program
    {
        public static Deck Deck;
        public static Order Order;

        /*
		 *
		 * Add your character to the roll (default to your name)
		 * Name your character (allowing enemies)
		 * Draw your cardName for the initiative
		 * Command to end the turn order and recalculate with existing options
		 * Command to edit your name
		 * Command to override your cardName/roll
		 * Handle HOLD cases
		 * Handle surprise cases
		 * Allow identifying enemies
		 *
		 */

        private DiscordSocketClient _client;
        public struct GlobalConfigStruct
        {
            public static string _botToken = "";
            public static string _variantDeck = "";
            public static string _lineFormat = "> [SUIT] [CARD] - **[CharacterName]** - _[AlliedStatus]_";
        }

        public static void SetArgs(string[] args)
        {
            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    if (arg.Contains("--token="))
                    {
                        GlobalConfigStruct._botToken = arg.Split("=")[1];
                    }
                    if (arg.Contains("--deck="))
                    {
                        GlobalConfigStruct._variantDeck = arg.Split("=")[1];
                    }
                }
            }
        }

		public static void Main(string[] args)
			=> new Program().MainAsync(args).GetAwaiter().GetResult();

		public async Task MainAsync(string[] args)
		{
			SetArgs(args);
            Deck = new Deck(GlobalConfigStruct._variantDeck);
            Order = new Order();

            using (var services = ConfigureServices())

                if (Deck.DeckExists)
                {
                    var client = services.GetRequiredService<DiscordSocketClient>();
                _client = client;
                    client.Log += Log;
                client.LoginAsync(TokenType.Bot, GlobalConfigStruct._botToken);
                client.StartAsync();
                await services.GetRequiredService<CommandHandler>().InitializeAsync();
                await Task.Delay(-1);
                }
            Console.WriteLine("Mischief managed. Closing.");
		}

		private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}

        private ServiceProvider ConfigureServices()
        {
            // this returns a ServiceProvider that is used later to call for those services
            // we can add types we have access to here, hence adding the new using statement:
            // using csharpi.Services;
            // the config we build is also added, which comes in handy for setting the command prefix!
            return new ServiceCollection()
                //.AddSingleton(_config)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
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
