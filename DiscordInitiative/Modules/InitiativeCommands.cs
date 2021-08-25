using Discord.Commands;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DiscordInitiative.Modules
{
    public class InitiativeCommands : ModuleBase
    {
        [Command("init")]
        public async Task InitCommand()
        {
            string actorName = Context.User.Username;

            ActorList.Add(actorName, 0);

            // Acknowledge
            await ReplyAsync(actorName + " added to the initiative order.");
        }

        [Command("init")]
        public async Task InitCommandWithArgs([Remainder] string args)
        {
            var sb = new StringBuilder();

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
            actorName = actorName.Trim();
            ActorList.Add(actorName, actorType);

            // send simple string reply
            await ReplyAsync(actorName + " added to the initiative order.");
        }

        [Command("initHidden")]
        public async Task InitHiddenCommand()
        {
            await ReplyAsync("!initHidden must be called with arguments for name and (optionally) allegiance).");
        }

        [Command("initHidden")]
        public async Task InitHiddenCommandWithArgs([Remainder] string args)
        {
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

            ActorList.Add(actorName, actorType, true);

            // send simple string reply
            await ReplyAsync(actorName + " added to the initiative order as hidden.");
        }

        [Command("round")]
        public async Task RoundCommand()
        {
            var sb = new StringBuilder();

            if (Program.Deck.RemainingCardCount() < ActorList.ActorCount() || Program.Deck.JokerHit)
            {
                Program.Deck.Shuffle();
            }

            ActorList.NewRound();
            foreach (var line in ActorList.GetInitList())
            {
                sb.AppendLine(line);
            }

            await ReplyAsync(sb.ToString());
        }

        [Command("hold")]
        public async Task HoldCommand()
        {
            await ReplyAsync("Please specify an actor to hold their card for next round.");
        }

        [Command("hold")]
        public async Task HoldCommandWithArguments([Remainder] string args)
        {
            var argList = args.Split(" ");
            string response;
            string actorName = "";

            if (argList.Length == 1)
            {
                actorName = argList[0];
            }
            else if (argList.Length == 0)
            {
                response = "";
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
            response = ActorList.HoldCard(actorName);

            await ReplyAsync(response);
        }

        [Command("remove")]
        public async Task RemoveCommand()
        {
            await ReplyAsync("Please specify an actor to remove from initiative.");
        }

        [Command("remove")]
        public async Task RemoveCommandWithArguments([Remainder] string args)
        {
            var argList = args.Split(" ");
            string response;
            string actorName = "";

            if (argList.Length == 1)
            {
                actorName = argList[0];
            }
            else if (argList.Length == 0)
            {
                response = "Please specify an actor to remove from initiative.";
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
            response = ActorList.RemoveActor(actorName);

            await ReplyAsync(response);
        }

        [Command("kill")]
        public async Task KillCommand()
        {
            await ReplyAsync("Please specify an actor to kill.");
        }

        [Command("kill")]
        public async Task KillCommandWithArguments([Remainder] string args)
        {
            var argList = args.Split(" ");
            string response;
            string actorName = "";

            if (argList.Length == 1)
            {
                actorName = argList[0];
            }
            else if (argList.Length == 0)
            {
                response = "Please specify an actor to kill.";
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
            response = ActorList.KillActor(actorName);

            await ReplyAsync(response);
        }

        [Command("show")]
        public async Task ShowCommandWithArguments([Remainder] string args)
        {
            var argList = args.Split(" ");
            string response;
            string actorName = "";

            if (argList.Length == 1)
            {
                actorName = argList[0];
            }
            else if (argList.Length == 0)
            {
                response = "Please specify an actor to show.";
            }
            else
            {
                actorName = argList[0];

                for (int i = 1; i <= argList.Length; i++)
                {
                    actorName = actorName + " " + argList[i];
                }
            }

            response = ActorList.ShowActor(actorName);

            await ReplyAsync(response);
        }

        [Command("set")]
        public async Task SetCommand()
        {
            await ReplyAsync("Please specify a command, actor and value. For instance !set Sullitude init 99.");
        }

        [Command("set")]
        public async Task SetCommandWithArguments([Remainder] string args)
        {
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
                if (argList[^2] != "init" && argList[^2] != "allegiance")
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
                        response = ActorList.SetActorInit(actorName, value);
                    }
                    else if (command == "allegiance")
                    {
                        response = ActorList.SetActorAllegiance(actorName, value);
                    }
                }
            }

            foreach (var line in ActorList.GetInitList())
            {
                sb.AppendLine(line);
            }

            await ReplyAsync(response + "\r\n" + sb.ToString());
        }

        [Command("edge")]
        public async Task EdgeCommand()
        {
            await ReplyAsync("Please specify the actor's name, the edge and, if appropriate, your skill bonus.");
        }
        [Command("edge")]
        public async Task EdgeCommandWithArguments([Remainder] string args)
        {
            var argList = args.Split(" ");
            string response;
            string actorName = "";

            if (argList.Length == 1)
            {
                actorName = argList[0];
            }
            else if (argList.Length == 0)
            {
                response = "";
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
            response = ActorList.HoldCard(actorName);

            await ReplyAsync(response);
        }

    }
}
