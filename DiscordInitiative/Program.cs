using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DiscordInitiative;
using Newtonsoft.Json.Linq;
using DiscordInitiative.Services;
using Microsoft.Extensions.DependencyInjection;

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
                        Cards.Add(new Card((int)cardsObj["value"], cardsObj["name"]?.ToString(),
                            cardsObj["suit"]?.ToString()));
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
            foreach (var card in Program.Deck.Cards)
            {
                if (!card.Held)
                    card.Drawn = false;
            }

            JokerHit = false;
        }

        public bool JokerHit { get; set; }

        public void RemoveHolds()
        {
            foreach (var card in Program.Deck.Cards)
            {
                card.Held = false;
            }
        }

        public void MarkCardAsHeld(Card actorCard)
        {
            foreach (var card in Program.Deck.Cards)
            {
                if (card == actorCard)
                    card.Held = true;
            }
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
        public bool Held { get; set; }
        public bool Drawn { get; set; }
    }

    public class Actor
    {
        public string Name { get; set; }
        public Card Card { get; set; }
        public int ValueOverride { get; set; }
        public int AlliedStatus { get; set; } // 0 - PC, 1 - Allied NPC, 2 - Enemy NPC, 3+ TBD
        public bool Hidden { get; set; }
        public int HealthStatus { get; set; } // 0 - Dead, 1 - Alive, 2+ TBD
        public bool HoldCard { get; set; }
        public int Status { get; set; } // 0 - Removed, 1 - Active

        public int GetCardValue()
        {
            if (HasCard())
            {
                return Card.Value;
            }
            else
            {
                return 0;
            }
        }
        public string GetOrderString()
        {

            string alliedString;
            string emoji;
            string card;

            switch (AlliedStatus)
            {
                case 0:
                    alliedString = "PC";
                    break;
                case 1:
                    alliedString = "Allied NPC";
                    break;
                case 2:
                    alliedString = "Enemy";
                    break;
                default:
                    alliedString = "Unknown Allegiance (This is probably a bug)";
                    break;
            }

            if (this.HealthStatus == 0)
            {
                emoji = ":skull_and_crossbones:";
                card = "";
            }
            else
            {
                emoji = Card.SuitEmoji;
                card = Card.CardName;
            }

            string orderString = Program.GlobalConfigStruct.LineFormat
                .Replace("[SUIT]", emoji)
                .Replace("[CARD]", card)
                .Replace("[CharacterName]", Name)
                .Replace("[VALUE]", GetInitValue().ToString())
                .Replace("[AlliedStatus]", alliedString);

            return orderString;
        }

        public void DrawCard()
        {
            Card newCard = Program.Deck.Draw();
            if (newCard.CardName == "")
            {
                Program.Deck.JokerHit = true;
            }

            Card = newCard;
        }

        public int GetInitValue()
        {
            if (HealthStatus != 0 && HasCard() && ValueOverride < 0)
            {
                return Card.Value;
            }
            else if (ValueOverride > 0)
            {
                return ValueOverride;
            }
            else
            {
                return 0;
            }
        }

        public Actor(string name, int alliedStatus, bool hidden)
        {
            this.Name = name;
            this.AlliedStatus = alliedStatus;
            this.Hidden = hidden;
            Card = null;
            HealthStatus = 1;
            Status = 1;
            ValueOverride = -1;
        }

        public bool HasCard()
        {
            if (Card == null)
            {
                return false;
            }

            return true;
        }
    }

    public class ActorList
    {
        private static readonly List<Actor> initList = new List<Actor>();

        public static void Add(string actorName, int alliedStatus)
        {
            Actor newActor = new Actor(actorName, alliedStatus, false);
            initList.Add(newActor);
        }
        public static void Add(string actorName, int alliedStatus, bool hidden)
        {
            Actor newActor = new Actor(actorName, alliedStatus, hidden);
            initList.Add(newActor);
        }

        public static string HoldCard(string actorName)
        {

            bool foundActor = false;
            foreach (var actor in initList)
            {
                if (actor.Name == actorName)
                {
                    actor.HoldCard = true;
                    Program.Deck.MarkCardAsHeld(actor.Card);
                    foundActor = true;
                }
            }

            if (!foundActor)
                return "Couldn't find actor " + actorName + ". Please check spelling.";

            return actorName + " is holding their card for next round.";
        }
        public static string RemoveActor(string actorName)
        {

            bool foundActor = false;
            foreach (var actor in initList)
            {
                if (actor.Name == actorName)
                {
                    actor.Status = 0;
                    foundActor = true;
                }
            }

            if (!foundActor)
                return "Couldn't find actor " + actorName + ". Please check spelling.";

            return actorName + " has been removed from initiative.";
        }
        public static string KillActor(string actorName)
        {

            bool foundActor = false;
            foreach (var actor in initList)
            {
                if (actor.Name == actorName)
                {
                    actor.HealthStatus = 0;
                    actor.Card = null;
                    foundActor = true;
                }
            }

            if (!foundActor)
                return "Couldn't find actor " + actorName + ". Please check spelling.";

            return actorName + " has been marked as killed.";
        }

        public static string ShowActor(string actorName)
        {

            bool foundActor = false;
            foreach (var actor in initList)
            {
                if (actor.Name == actorName)
                {
                    actor.Hidden = false;
                    foundActor = true;
                }
            }

            if (!foundActor)
                return "Couldn't find actor " + actorName + ". Please check spelling.";

            return actorName + " has appeared.";
        }

        public static List<String> GetInitList()
        {
            List<String> initListString = new List<string>();
            if (initList.Count > 0)
            {
                var orderedActorList = initList.OrderByDescending(value => value.GetCardValue());
                foreach (var actor in orderedActorList)
                {
                    if (!actor.Hidden && ((actor.Status == 1 && actor.HasCard()) ||
                                          (actor.Status == 1 && actor.HealthStatus == 0)))
                        initListString.Add(actor.GetOrderString());
                }

                if (Program.Deck.JokerHit)
                {
                    initListString.Add("(Someone hit a joker this round. Deck will reshuffle.)");
                }
                else
                {
                    initListString.Add("(There are " + Program.Deck.RemainingCardCount() + " cards left in the deck.)");
                }
            }
            else
            {
                initListString.Add("There are no actors in the initiative order. Use !init to add them.");
            }
            initListString.Add("Round #" + Program.RoundCount);
            RemoveActorHolds();
            return initListString;
        }

        public static void RemoveActorHolds()
        {
            foreach (var actor in initList)
            {
                actor.HoldCard = false;
            }
        }

        public static int ActorCount()
        {
            return initList.Count();
        }

        public static void NewRound()
        {
            foreach (var actor in initList)
            {
                actor.ValueOverride = -1;
                    if(!actor.HoldCard && actor.HealthStatus != 0 && actor.Status != 0)
                    actor.DrawCard();
                Program.Deck.RemoveHolds();
            }
        }

        public static string SetActorInit(string actorName, int value)
        {

            bool foundActor = false;
            foreach (var actor in initList)
            {
                if (actor.Name == actorName)
                {
                    actor.ValueOverride = value;
                    foundActor = true;
                }
            }

            if (!foundActor)
                return "Couldn't find actor " + actorName + ". Please check spelling.";

            return actorName + " has been given an initiative of " + value + ".";
        }

        public static string SetActorAllegiance(string actorName, int value)
        {

            bool foundActor = false;
            foreach (var actor in initList)
            {
                if (actor.Name == actorName)
                {
                    actor.AlliedStatus = value;
                    foundActor = true;
                }
            }

            if (!foundActor)
                return "Couldn't find actor " + actorName + ". Please check spelling.";

            return actorName + " has been given an allegiance of " + value + ".";
        }

        public static string SetActorVisibility(string actorName, bool hidden)
        {

            bool foundActor = false;
            foreach (var actor in initList)
            {
                if (actor.Name == actorName)
                {
                    actor.Hidden = hidden;
                    foundActor = true;
                }
            }

            if (!foundActor)
                return "Couldn't find actor " + actorName + ". Please check spelling.";

            return actorName + " has their visibility updated.";
        }

        public static bool DrawCardForActor(string actorName)
        {
            foreach (var actor in initList)
            {
                if (actor.Name == actorName)
                {
                    actor.DrawCard();
                    return true;
                }
            }

            return false;

        }

        public static void EndCombat()
        {
            initList.Clear();
        }
    }
}

public class Program
    {
        public static Deck Deck;
        public static ActorList ActorList;
        public static int RoundCount = 1;

        public struct GlobalConfigStruct
        {
            public static string BotToken = "";
            public static string VariantDeck = "";
            public static string LineFormat = "> [SUIT] [CARD] ([VALUE]) - **[CharacterName]** - _[AlliedStatus]_";
        }

        public static void SetArgs(string[] args)
        {
            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    if (arg.Contains("--token="))
                    {
                        GlobalConfigStruct.BotToken = arg.Split("=")[1];
                    }
                    if (arg.Contains("--deck="))
                    {
                        GlobalConfigStruct.VariantDeck = arg.Split("=")[1];
                    }
                }
            }
        }

		public static void Main(string[] args)
			=> new Program().MainAsync(args).GetAwaiter().GetResult();

		public async Task MainAsync(string[] args)
		{
			SetArgs(args);
            Deck = new Deck(GlobalConfigStruct.VariantDeck);
            ActorList = new ActorList();

            using (var services = ConfigureServices())

                if (Deck.DeckExists)
                {
                    var client = services.GetRequiredService<DiscordSocketClient>();
                    client.Log += Log;
                client.LoginAsync(TokenType.Bot, GlobalConfigStruct.BotToken);
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

