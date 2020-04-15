using Andrew_Roe_s_Jarvis.Classes;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Andrew_Roe_s_Jarvis
{
    public class DiscordBot
    {
        public static DiscordSocketClient client = new DiscordSocketClient();
        public static IServiceProvider services;
        public static CommandService commands;
        // Guild id's.
        public static ulong mainGuildId = 415234632712192010;
        // Channel id's.
        public static ulong consoleChannelID = 677987647251152896;
        public static ulong logChannelID = 677577659919892480;
        public static ulong roleChannelID = 677578540497240064;
        public static ulong welcomeChannelID = 677578889400287253;
        public static ulong welcomeRulesChannelID = 677964771772989460;
        // Role id's.
        public static ulong staffRoleId = 677590782755995658;
        public static ulong memberRoleId = 677583182886993940;
        public static ulong notificationRoleID = 678574505320251427;
        public static ulong cyberSecurityRoleId = 677583221625716757;
        public static ulong programmingRoleID = 677583269373411387;
        // Message id's.
        public static ulong roleMessageID = 678581276634120232;
        // Emotes
        public static string verificationEmote = "👍";
        /* Test Server Values
        public static ulong mainGuildId = 465199415280533517;
        public static ulong consoleChannelID = 677950142883823646;
        public static ulong logChannelID = 677950191244148760;
        public static ulong roleChannelID = 678578635229102083;
        public static ulong roleMessageID = 678580136341405706;
        public static ulong welcomeChannelID = 602495686986039296;
        public static ulong welcomeRulesChannelID = 677973692021735434;
        */
        public static ManualResetEvent consoleMessageReceived = new ManualResetEvent(false);
        public static async Task StartAsync() => await client.StartAsync();

        public static async Task Login()
        {
            // Asks for the token in the console.
            Console.Write("Please enter or paste your bot token here: ");
            string token = Console.ReadLine();
            Console.Clear();

            // Login to Discord as a bot user using Oauth2
            await client.LoginAsync(TokenType.Bot, token);
        }


        public static async Task BuildServices()
        {
            commands = new CommandService();
            services = new ServiceCollection()
                .BuildServiceProvider();

            // Discover all of the commands in this assembly and load them.
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }

        public static async Task HookEvents()
        {
            client.Log += LogAsync;
            client.Ready += ReadyAsync;
            client.MessageReceived += EventHandler.MessageReceived;
            client.ReactionAdded += EventHandler.ReactionAdded;
            client.ReactionRemoved += EventHandler.ReactionRemoved;
            client.UserBanned += EventHandler.UserBanned;
            client.UserJoined += EventHandler.UserJoined;
        }

        private static Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private static async Task ReadyAsync()
        {
            Console.WriteLine($"{DateTime.UtcNow.ToString("HH:mm:ss")} Log         {client.CurrentUser} is connected.");
            foreach (SocketGuild guild in client.Guilds)
                Console.WriteLine($"{DateTime.UtcNow.ToString("HH:mm:ss")} Log         Succesfully connected to {guild.Name}.");
            await Task.CompletedTask;
        }

        public static void LoopConsoleMessage()
        {
            while (true)
            {
                // Check if the client is connected to guilds.
                if (client.Guilds.Count > 0)
                {
                    // Check if the client is connected to the 'Discord Owners Server' guild
                    if (client.GetGuild(mainGuildId).IsConnected == true)
                    {
                        // Keep sending the messages in the queue as long as the queue has at least 1 or more messages in it.
                        while (ConsoleHandler.pendingMessages.Count > 0)
                        {
                            // Sends the message to the Discord console output.
                            client.GetGuild(mainGuildId).GetTextChannel(consoleChannelID).SendMessageAsync($"```{ConsoleHandler.pendingMessages.ToList()[0]}```");

                            consoleMessageReceived.WaitOne();

                            // Dequeue the message.
                            ConsoleHandler.pendingMessages.Dequeue();

                            consoleMessageReceived.Reset();
                        }
                    }
                }
            }
        }

        public static async Task SendBanMessage(BanInfo banInfo)
        {
            // Creates a new EmbededBuilder to send embeded messages.
            EmbedBuilder builder = new EmbedBuilder();
            // The following if-else statement checks if there was a ban reason given.
            if (string.IsNullOrEmpty(banInfo.Reason))
                banInfo.Reason = "There was no reason given.";

            // Sets the message color to red.
            builder.WithColor(Color.Red);
            // Set the title.
            builder.WithTitle("User banned:");
            // Adds the discription.
            builder.WithDescription("A user was banned.");
            builder.AddField("General Information:",
                $"Reason: {banInfo.Reason}.\n" +
                $"Source: {banInfo.Source}\n");

            // Gets the avatar (profile picture) of the banned user.
            builder.ThumbnailUrl = client.GetUser(banInfo.UserId).GetAvatarUrl();
            if (banInfo.StaffId != 0)
                builder.AddField("Staff information", $"Username: { banInfo.StaffUsername}#{banInfo.StaffDiscriminator}\n" +
                    $"User Id: {banInfo.StaffId}");
            // The folowing two AddField methods adds aditional information about the user.
            builder.AddField("User information", $"Username: {banInfo.Username}#{banInfo.UserDiscriminator}\n" +
                $"User id: {banInfo.UserId}\n" +
                $"User created at: {banInfo.UserCreationDate}\n" +
                $"User joined the guild at: {banInfo.JoinDate}");
            builder.WithCurrentTimestamp();

            // Sends the embeded message to the Discord ban-list channel.
            await client.GetGuild(mainGuildId).GetTextChannel(logChannelID).SendMessageAsync(null, false, builder.Build());
        }

        public static bool CheckIsInGuild(ulong guildId, ulong userId)
        {
            if (client.GetGuild(guildId).GetUser(userId) != null)
                return true;
            else
                return false;
        }

        public static async Task<bool> CheckIfBanned(ulong guildId, ulong UserId)
        {
            try
            {
                RestBan ban = await client.GetGuild(guildId).GetBanAsync(UserId);
                return true;
            }
            catch (HttpException e) when (e.DiscordCode == 10026)
            {
                return false;

            }
        }
    }
}
