using Discord.Commands;
using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordInitiative.Modules
{
    public class InitiativeCommands : ModuleBase
    {
        [Command("init")]
        public async Task InitCommand()
        {
            BotInstance instance = Program.GetInstanceById(Context.Channel.Id.ToString());
            string actorName = Context.User.Username;
            if (instance.GetSavedActorName(Context.User.Username) != null)
            {
                actorName = instance.GetSavedActorName(Context.User.Username);
            }
            else
            {
                instance.UserUpdate(Context.User.Username, Context.User.Username);
            }
            
            
            instance.AddActor(actorName,0);

            // Acknowledge
            await ReplyAsync(actorName + " added to the initiative order.");
        }

        [Command("init")]
        public async Task InitCommandWithArgs([Remainder] string args)
        {
            BotInstance instance = Program.GetInstanceById(Context.Channel.Id.ToString());
            int actorAllegiance = 0;
            bool success = false;
            var argList = args.Split(" ");
            string actorName = Context.User.Username;
            if (argList.Length == 1)
            {
                actorName = argList[0];
            }

            if (argList.Length >= 1)
            {
                actorName = "";
                if (int.TryParse(argList.Last(), out actorAllegiance))
                {
                    actorAllegiance = Convert.ToInt16(argList.Last());
                    if(actorAllegiance <= 2)
                    {
                        for (int i = 0; i < argList.Length - 1; i++)
                        {
                            actorName += argList[i] + " ";
                        }

                        success = true;
                    }
                    else
                    {
                        await ReplyAsync(
                            "Couldn't understand that allegiance value. Use 0 for PC, 1 for Allied NPC, 2 for Enemy NPC");
                        success = false;
                    }
                }
                else
                {
                    for (int i = 0; i < argList.Length; i++)
                    {
                        actorName += argList[i] + " ";
                    }
                    success = true;
                }
            }

            if (success)
            {
                actorName = actorName.Trim();
                instance.UserUpdate(Context.User.Username, actorName);
                instance.AddActor(actorName, actorAllegiance);

                // send simple string reply
                await ReplyAsync(actorName + " added to the initiative order.");
            }

        }

        [Command("list")]
        public async Task ListCommand()
        {
            BotInstance instance = Program.GetInstanceById(Context.Channel.Id.ToString());
            var sb = new StringBuilder();
            int actorsWithDrawnCards = 0;
            int actorsWithoutDrawnCards = 0;
            foreach (var actor in instance.ActorList)
            {
                if (actor.Card != null)
                    actorsWithDrawnCards++;
                else
                {
                    actorsWithoutDrawnCards++;
                }
            }

            if (instance.RoundCount == 0)
            {
                await ReplyAsync("Combat has not started so no cards have been drawn yet. Use !round to start the first round of combat.");
            }

            if (actorsWithDrawnCards == 0)
            {
                await ReplyAsync("No actors have drawn cards yet. Use !init to add them and !round to start a round and draw cards.");
            }
            else
            {
                foreach (var line in instance.GetInitList())
                {
                    sb.AppendLine(line);
                }

                if (actorsWithoutDrawnCards > 0)
                {
                    sb.AppendLine(actorsWithoutDrawnCards.ToString() +
                                  " actors were added but won't draw cards until the next !round.");
                }

                await ReplyAsync(sb.ToString());
            }

        }

        [Command("draw")]
        public async Task DrawCommand()
        {
            BotInstance instance = Program.GetInstanceById(Context.Channel.Id.ToString());
            var sb = new StringBuilder();
            string actorName;
            if (instance.GetSavedActorName(Context.User.Username) != null)
            {
                actorName = instance.GetSavedActorName(Context.User.Username);
            }
            else
            {
                actorName = Context.User.Username;
            }
            if (instance.DrawCardForActor(actorName))
            {
                foreach (var line in instance.GetInitList())
                {
                    sb.AppendLine(line);
                }

                await ReplyAsync(sb.ToString());
            }
            else
            {
                await ReplyAsync("Failed to draw a card for " + actorName +
                                 ". Are they active in the initiative list?");
            }

        }

        [Command("draw")]
        public async Task DrawCommandWithArgs([Remainder] string args)
        {
            BotInstance instance = Program.GetInstanceById(Context.Channel.Id.ToString());
            var sb = new StringBuilder();

            var argList = args.Split(" ");
            string actorName = Context.User.Username;
            if (argList.Length == 1)
            {
                actorName = argList[0];
            }

            if (argList.Length == 2)
            {
                actorName = argList[0];
            }

            if (argList.Length > 2)
            {
                actorName = "";
                for (int i = 0; i < argList.Length - 1; i++)
                {
                    actorName += argList[i] + " ";

                }
            }
            actorName = actorName.Trim();

            if (instance.DrawCardForActor(actorName))
            {
                foreach (var line in instance.GetInitList())
                {
                    sb.AppendLine(line);
                }

                await ReplyAsync(sb.ToString());
            }
            else
            {
                await ReplyAsync("Failed to draw a card for " + actorName +
                                 ". Are they active in the initiative list?");
            }
        }

        [Command("initHidden")]
        public async Task InitHiddenCommand()
        {
            await ReplyAsync("!initHidden must be called with arguments for name and (optionally) allegiance).");
        }

        [Command("initHidden")]
        public async Task InitHiddenCommandWithArgs([Remainder] string args)
        {
            BotInstance instance = Program.GetInstanceById(Context.Channel.Id.ToString());
            int actorType = 0;
            var argList = args.Split(" ");
            string actorName = Context.User.Username;
            if (argList.Length == 1)
            {
                actorName = argList[0];
            }

            if (argList.Length == 2)
            {
                actorName = argList[0];
                try
                {
                    actorType = Convert.ToInt16(argList[1]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    await ReplyAsync(
                        "Couldn't understand that allegiance value. Use 0 for PC, 1 for Allied NPC, 2 for Enemy NPC");
                    throw;
                }

            }

            if (argList.Length > 2)
            {
                actorName = "";
                for (int i = 0; i < argList.Length - 1; i++)
                {
                    actorName += argList[i] + " ";
                }

                try
                {
                    actorType = Convert.ToInt16(argList[^1]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    await ReplyAsync(
                        "Couldn't understand that allegiance value. Use 0 for PC, 1 for Allied NPC, 2 for Enemy NPC");
                    throw;
                }
            }

            instance.AddActor(actorName, actorType, true);

            // send simple string reply
            await ReplyAsync(actorName + " added to the initiative order as hidden.");
        }

        [Command("round")]
        public async Task RoundCommand()
        {
            BotInstance instance = Program.GetInstanceById(Context.Channel.Id.ToString());
            instance.RoundCount++;
            var sb = new StringBuilder();

            if (instance.Deck.RemainingCardCount() < instance.ActorCount() || instance.Deck.JokerHit)
            {
                instance.Deck.Shuffle();
            }

            instance.NewRound();
            foreach (var line in instance.GetInitList())
            {
                sb.AppendLine(line);
            }
            instance.RemoveActorHolds();

            await ReplyAsync(sb.ToString());
        }

        [Command("hold")]
        public async Task HoldCommand()
        {
            BotInstance instance = Program.GetInstanceById(Context.Channel.Id.ToString());
            string actorName;
            string response;
            if (instance.GetSavedActorName(Context.User.Username) != null)
            {
                actorName = instance.GetSavedActorName(Context.User.Username);
            }
            else
            {
                actorName = Context.User.Username;
            }
            response = instance.HoldCard(actorName);

            await ReplyAsync(response);
        }

        [Command("hold")]
        public async Task HoldCommandWithArguments([Remainder] string args)
        {
            BotInstance instance = Program.GetInstanceById(Context.Channel.Id.ToString());

            var argList = args.Split(" ");
            string response;
            string actorName;

            if (argList.Length == 1)
            {
                actorName = argList[0];
            }
            else
            {
                actorName = argList[0];

                for (int i = 1; i < argList.Length; i++)
                {
                    actorName = actorName + " " + argList[i];

                }
            }
            actorName = actorName.Trim();
            response = instance.HoldCard(actorName);

            await ReplyAsync(response);
        }

        [Command("remove")]
        public async Task RemoveCommand()
        {
            BotInstance instance = Program.GetInstanceById(Context.Channel.Id.ToString());
            string actorName;
            string response;
            if (instance.GetSavedActorName(Context.User.Username) != null)
            {
                actorName = instance.GetSavedActorName(Context.User.Username);
            }
            else
            {
                actorName = Context.User.Username;
            }
            response = instance.RemoveActor(actorName);

            await ReplyAsync(response);
        }

        [Command("remove")]
        public async Task RemoveCommandWithArguments([Remainder] string args)
        {
            BotInstance instance = Program.GetInstanceById(Context.Channel.Id.ToString());
            var argList = args.Split(" ");
            string response;
            string actorName = "";

            if (argList.Length == 1)
            {
                actorName = argList[0];
            }
            else if (argList.Length == 0)
            {
                await ReplyAsync("Please specify an actor to remove from initiative.");
            }
            else
            {
                actorName = argList[0];

                for (int i = 1; i < argList.Length; i++)
                {
                    actorName = actorName + " " + argList[i];

                }
            }
            actorName = actorName.Trim();
            response = instance.RemoveActor(actorName);

            await ReplyAsync(response);
        }

        [Command("kill")]
        public async Task KillCommand()
        {
            BotInstance instance = Program.GetInstanceById(Context.Channel.Id.ToString());
            string actorName;
            string response;
            if (instance.GetSavedActorName(Context.User.Username) != null)
            {
                actorName = instance.GetSavedActorName(Context.User.Username);
            }
            else
            {
                actorName = Context.User.Username;
            }
            response = instance.KillActor(actorName);

            await ReplyAsync(response);
        }

        [Command("kill")]
        public async Task KillCommandWithArguments([Remainder] string args)
        {
            BotInstance instance = Program.GetInstanceById(Context.Channel.Id.ToString());
            var argList = args.Split(" ");
            string response;
            string actorName = "";

            if (argList.Length == 1)
            {
                actorName = argList[0];
            }
            else if (argList.Length == 0)
            {
                await ReplyAsync("Please specify an actor to kill.");
            }
            else
            {
                actorName = argList[0];

                for (int i = 1; i < argList.Length; i++)
                {
                    actorName = actorName + " " + argList[i];

                }
            }
            actorName = actorName.Trim();
            response = instance.KillActor(actorName);

            await ReplyAsync(response);
        }

        [Command("show")]
        public async Task ShowCommandWithArguments([Remainder] string args)
        {
            BotInstance instance = Program.GetInstanceById(Context.Channel.Id.ToString());
            var argList = args.Split(" ");
            string response;
            string actorName = "";

            if (argList.Length == 1)
            {
                actorName = argList[0];
            }
            else if (argList.Length == 0)
            {
                await ReplyAsync("Please specify an actor to show.");
            }
            else
            {
                actorName = argList[0];

                for (int i = 1; i <= argList.Length; i++)
                {
                    actorName = actorName + " " + argList[i];
                }
            }

            response = instance.ShowActor(actorName);

            await ReplyAsync(response);
        }

        [Command("set")]
        public async Task SetCommand()
        {
            await ReplyAsync("Please specify a command, actor and value. For instance !set Sullitude init 99. Valid commands are 'allegiance', 'hidden', and 'init'.");
        }

        [Command("set")]
        public async Task SetCommandWithArguments([Remainder] string args)
        {
            BotInstance instance = Program.GetInstanceById(Context.Channel.Id.ToString());
            var sb = new StringBuilder();
            var argList = args.Split(" ");
            string response = "";
            string actorName = "";
            int value;
            string command;

            if (argList.Length < 3)
            {
                response = "Please specify a command, actor and value. For instance !set Sullitude init 99 or !set Sullitude allegiance 0.";
            }
            else
            {
                if (argList[^2] != "init" && argList[^2] != "allegiance" && argList[^2] != "hidden" && argList[^2] != "Allegiance" && argList[^2] != "Hidden")
                {
                    response = "Please specify a command, actor and value. For instance !set Sullitude init 99 or !set Sullitude allegiance 0.";
                }
                else if (!int.TryParse(argList[^1], out value))
                {
                    response = "Invalid value to set (must be a number). For instance !set Sullitude init 99 or !set Sullitude allegiance 0.";
                }
                else
                {
                    command = argList[^2];
                    value = Int16.Parse(argList[^1]);
                    for (int i = 0; i < argList.Length - 2; i++)
                    {
                        actorName = actorName + " " + argList[i];
                    }

                    actorName = actorName.Trim();
                    if (command == "init")
                    {
                        response = instance.SetActorInit(actorName, value);
                    }
                    else if (command == "allegiance" || command == "Allegiance")
                    {
                        response = instance.SetActorAllegiance(actorName, value);
                    }
                    else if (command == "hidden" || command == "Hidden")
                    {
                        if (value == 1)
                            instance.SetActorVisibility(actorName, true);
                        else if (value == 0)
                            instance.SetActorVisibility(actorName, false);
                    }
                }
            }

            foreach (var line in instance.GetInitList())
            {
                sb.AppendLine(line);
            }

            await ReplyAsync(response + "\r\n" + sb);
        }

        [Command("end")]
        public async Task EndCommand()
        {
            BotInstance instance = Program.GetInstanceById(Context.Channel.Id.ToString());
            instance.EndCombat();
            instance.RoundCount = 0;
            await ReplyAsync("Combat has resolved. I hope some of you are still alive.");
        }

    }
}
