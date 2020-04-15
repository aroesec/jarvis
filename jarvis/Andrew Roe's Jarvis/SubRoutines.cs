using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Andrew_Roe_s_Jarvis
{
    public class Subroutines
    {

        public static async void BotGame()
        {
            Thread.Sleep(10000);
            Random random = new Random();

            while (true)
            {
                int randomNumber = random.Next(0, 4);

                switch (randomNumber)
                {
                    case 0:
                        IReadOnlyCollection<Discord.WebSocket.SocketGuild> guilds = DiscordBot.client.Guilds;
                        var enumGuild = guilds.GetEnumerator();
                        enumGuild.MoveNext();
                        await DiscordBot.client.SetGameAsync($"over {enumGuild.Current.Users.Count} cool users!", null, ActivityType.Watching);
                        break;
                    case 1:
                        await DiscordBot.client.SetGameAsync($"Don't forget to check out the Twitch and YouTube!", null, ActivityType.Watching);
                        break;
                    case 2:
                        await DiscordBot.client.SetGameAsync($"My latency is: {DiscordBot.client.Latency} ms.", null, ActivityType.Watching);
                        break;
                    case 3:
                        await DiscordBot.client.SetGameAsync($"Invite more people!");
                        break;
                    case 4:
                        await DiscordBot.client.SetGameAsync("Tcl Help", null, ActivityType.Listening);
                        break;
                }
                Thread.Sleep(30000);
            }
        }
    }
}
