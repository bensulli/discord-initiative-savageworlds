using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using static DiscordInitiative.Program;

namespace DiscordInitiative.Modules
{
    public class InitiativeCommands : ModuleBase
    {
        [Command("init")]
        public async Task InitCommand()
        {
            var sb = new StringBuilder();
            
            string actorType = "PC";
            string actorName = Context.User.Username;

            Order.Add(actorName, actorType);

            // build out the reply
            foreach (var line in Order.GetOrderList())
            {
                sb.AppendLine(line);
            }
            
            // send simple string reply
            await ReplyAsync(sb.ToString());
        }

        [Command("init")]
        public async Task InitCommandWithArgs([Remainder] string args)
        {
            var sb = new StringBuilder();

            string actorType = "PC";
            var argList = args.Split(" ");
            string actorName = Context.User.Username;
            if (argList.Length == 1)
            {
                actorName = argList[0];
            }
            if (argList.Length == 2)
            {
                actorName = argList[0];
                actorType = argList[1];
            }

            if (argList.Length > 2)
            {
                actorName = "";
                for (int i = 0;  i < argList.Length - 1; i++)
                {
                    actorName += argList[i] + " ";
                }

                actorType = argList[^1];
            }
            Order.Add(actorName,actorType);

            // build out the reply
            foreach (var line in Order.GetOrderList())
            {
                sb.AppendLine(line);
            }

            // send simple string reply
            await ReplyAsync(sb.ToString());
        }

    //    [Command("8ball")]
    //    [Alias("ask")]
    //    [RequireUserPermission(GuildPermission.KickMembers)]
    //    public async Task AskEightBall([Remainder] string args = null)
    //    {
    //        // I like using StringBuilder to build out the reply
    //        var sb = new StringBuilder();
    //        // let's use an embed for this one!
    //        var embed = new EmbedBuilder();

    //        // now to create a list of possible replies
    //        var replies = new List<string>();

    //        // add our possible replies
    //        replies.Add("yes");
    //        replies.Add("no");
    //        replies.Add("maybe");
    //        replies.Add("hazzzzy....");

    //        // time to add some options to the embed (like color and title)
    //        embed.WithColor(new Color(0, 255, 0));
    //        embed.Title = "Welcome to the 8-ball!";

    //        // we can get lots of information from the Context that is passed into the commands
    //        // here I'm setting up the preface with the user's name and a comma
    //        sb.AppendLine($",");
    //        sb.AppendLine();

    //        // let's make sure the supplied question isn't null 
    //        if (args == null)
    //        {
    //            // if no question is asked (args are null), reply with the below text
    //            sb.AppendLine("Sorry, can't answer a question you didn't ask!");
    //        }
    //        else
    //        {
    //            // if we have a question, let's give an answer!
    //            // get a random number to index our list with (arrays start at zero so we subtract 1 from the count)
    //            var answer = replies[new Random().Next(replies.Count - 1)];

    //            // build out our reply with the handy StringBuilder
    //            sb.AppendLine($"You asked: [****]...");
    //            sb.AppendLine();
    //            sb.AppendLine($"...your answer is [****]");

    //            // bonus - let's switch out the reply and change the color based on it
    //            switch (answer)
    //            {
    //                case "yes":
    //                    {
    //                        embed.WithColor(new Color(0, 255, 0));
    //                        break;
    //                    }
    //                case "no":
    //                    {
    //                        embed.WithColor(new Color(255, 0, 0));
    //                        break;
    //                    }
    //                case "maybe":
    //                    {
    //                        embed.WithColor(new Color(255, 255, 0));
    //                        break;
    //                    }
    //                case "hazzzzy....":
    //                    {
    //                        embed.WithColor(new Color(255, 0, 255));
    //                        break;
    //                    }
    //            }
    //        }

    //        // now we can assign the description of the embed to the contents of the StringBuilder we created
    //        embed.Description = sb.ToString();

    //        // this will reply with the embed
    //        await ReplyAsync(null, false, embed.Build());
    //    }
    }
}
