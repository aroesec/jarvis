using Andrew_Roe_s_Jarvis.Classes;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andrew_Roe_s_Jarvis
{
    public class UserCommands : ModuleBase
    {
        [Command("Help")]
        [Name("Help")]
        [Summary(@"Syntax: `/>Help`" +
            "Displays this message.")]
        public async Task HelpMenu()
        {
            EmbedBuilder builder = new EmbedBuilder();
            IEnumerable<ModuleInfo> commandModules = DiscordBot.commands.Modules;

            builder.Color = Color.Gold;
            builder.WithTitle("The CyberSec Lounge's command list");

            foreach (ModuleInfo commandModule in commandModules)
            {
                foreach (CommandInfo command in commandModule.Commands)
                {
                    builder.AddField(command.Name, command.Summary);
                }
            }

            await ReplyAsync(null, false, builder.Build());
        }        

        [Command("ServerInfo")]
        [Name("Server Information")]
        [Summary(@"Syntax: `/>ServerInfo`" +
            "\nSearches all the known information about a user in this guild.")]
        public async Task ServerInfo()
        {
            SocketGuild guild = DiscordBot.client.GetGuild(Context.Guild.Id);
            EmbedBuilder builder = new EmbedBuilder();
            string features = "";
            object defaultChannel;
            object systemChannel;

            if (guild.Features.Count > 0)
            {
                foreach (string feature in guild.Features)
                    features += feature;
            }
            else
                features = "This server doesn't have any features.";

            if (guild.DefaultChannel != null)
                defaultChannel = guild.DefaultChannel.Mention;

            else
                defaultChannel = "This server doesn't a default channel.";

            if (guild.SystemChannel != null)
                systemChannel = guild.SystemChannel.Mention;

            else
                systemChannel = "This server doesn't have a welcome channel.";

            builder.Color = Color.Teal;
            builder.WithTitle("The CyberSec Lounge's Server Information");
            builder.WithDescription("The following section contains general information about the server.\n\n");
            builder.AddField("Server name: ", guild.Name, true);
            builder.AddField("Server Id", guild.Id, true);
            builder.AddField("Creation Date", guild.CreatedAt, true);
            builder.AddField("Server Owner", guild.Owner.Mention, true);

            builder.AddField("====== Statistics And Numbers ======", "The following section contains general information about the server.");
            builder.AddField("# Of Roles", guild.Roles.Count, true);
            builder.AddField("# Of Categories", guild.CategoryChannels.Count, true);
            builder.AddField("# Of Text-Channels", guild.TextChannels.Count, true);
            builder.AddField("# Of Voice-Channels", guild.VoiceChannels.Count, true);
            builder.AddField("Total # Of users", guild.Users.Count, true);
            builder.AddField("Total # Of humans", guild.Users.Where(x => x.IsBot == false).Count(), true);
            builder.AddField("Total # Of bots", guild.Users.Where(x => x.IsBot == true).Count(), true);
            builder.AddField("Total # Of online humans", guild.Users.Where(x => x.IsBot == false && x.Status != UserStatus.Offline).Count(), true);
            builder.AddField("Total # Of offline humans", guild.Users.Where(x => x.IsBot == false && x.Status == UserStatus.Offline).Count(), true);


            await ReplyAsync(null, false, builder.Build());
        }

        [Command("WhoIs")]
        [Name("WhoIs")]
        [Summary(@"Syntax: `/>WhoIs <User Id>`" +
            "\nSearches all the known information about a user in this guild.")]
        public async Task WhoIs(string inputUser)
        {
            SocketGuildUser user;
                        
            if (GeneralFunctions.CheckUlong(inputUser) == true && DiscordBot.CheckIsInGuild(Context.Guild.Id, Convert.ToUInt64(inputUser)))
                user = await Context.Guild.GetUserAsync(Convert.ToUInt64(inputUser)) as SocketGuildUser;
            else if (GeneralFunctions.CheckUlong(inputUser) == false && Context.Message.MentionedUserIds.Count >= 1 && DiscordBot.CheckIsInGuild(Context.Guild.Id, Context.Message.MentionedUserIds.First()))
                user = await Context.Guild.GetUserAsync(Context.Message.MentionedUserIds.First()) as SocketGuildUser;
            else if ((GeneralFunctions.CheckUlong(inputUser) == true && DiscordBot.CheckIsInGuild(Context.Guild.Id, Convert.ToUInt64(inputUser)) == false) || (GeneralFunctions.CheckUlong(inputUser) == false && Context.Message.MentionedUserIds.Count >= 1 && DiscordBot.CheckIsInGuild(Context.Guild.Id, Context.Message.MentionedUserIds.First()) == false))
            {
                await ReplyAsync("Error 0x2: This user was not found in this server.");
                return;
            }
            else if (GeneralFunctions.CheckUlong(inputUser) == false || (GeneralFunctions.CheckUlong(inputUser) == false && Context.Message.MentionedUserIds.Count == 0))
            {
                await ReplyAsync("Error 0x3: There was not a valid id or mention found.");
                return;
            }
            else
            {
                await ReplyAsync("Error 0x4: An error occured during command execution please check your input");
                return;
            }

            string roles = "";
            string nickname = "";
            //SocketGuildUser user = DiscordBot.client.GetGuild(Context.Guild.Id).GetUser(userId);

            foreach (var role in user.Roles)
            {
                if (role == user.Roles.Last())
                    roles += $"{role.Name}.";

                else
                    roles += $"{role.Name}, ";
            }

            if (user.Username == null)
                nickname = "There is no nickname used.";

            else
                nickname = user.Nickname;

            var builder = new EmbedBuilder();
            builder.WithColor(Color.Blue);
            builder.WithTitle("User information:");
            builder.WithDescription("The following data is all the data about te user in this guild that could be retrieved.");
            builder.AddField("User data:", $"User Id: {user.Id}.\n" +
                $"Username: {user.Username}#{user.DiscriminatorValue}.\n" +
                $"Nickname: {user.Nickname}.\n" +
                $"User registered at: {user.CreatedAt}.\n" +
                $"User status: {user.Status}.\n");
            builder.AddField("Guild information:", $"User joined at: { user.JoinedAt}.\n" +
                $"User hierarchy: {user.Hierarchy}.\n" +
                $"User roles: {roles}\n" +
                $"User permission  value: {user.GuildPermissions.RawValue}");

            builder.ThumbnailUrl = user.GetAvatarUrl();
            builder.WithFooter(footer => footer.WithText("The CyberSec Lounge"));
            builder.WithCurrentTimestamp();

            await ReplyAsync("This is the information that was retrieved from the database.", false, builder.Build());
        }

        [Command("Ping")]
        [Name("Test The Ability To Respond")]
        [Summary(@"Syntax: `/>Ping`" +
            "\nThis command is used to see if the command can send responses back.")]
        public async Task Test()
        {
            await ReplyAsync($"Pong! My latency is: {DiscordBot.client.Latency}");
        }        
    }

    public class StaffCommands : ModuleBase
    {
        [Command("Purge")]
        [Name("Purge")]
        [Summary(@"Syntax: `/>Purge <amount>`" +
            "\nThis command deletes the given amount of messages send in the channel where the command is used.")]
        [RequireUserPermission(GuildPermission.ManageMessages, ErrorMessage = "Error 0x1: You don't have permissions to execute this command in this guild.")]
        public async Task PurgeMessages(int amount)
        {
            IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
            await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
            IUserMessage botMessage = await ReplyAsync($"I deleted {amount} messages.");
            await Task.Delay(5000);
            await botMessage.DeleteAsync();
        }

        [Command("Ban")]
        [Name("Ban a user from a server")]
        [Summary(@"Syntax: `/>Ban <User Id || Mention> [Reason]`" +
    "\nThis command is used to ban users in the current guild / server." +
    "\nNote that this command has to be executed in a server where you have the rights to ban users.")]
        [RequireUserPermission(GuildPermission.BanMembers, ErrorMessage = "Error 0x1: You don't have permissions to execute this command in this guild.")]
        public async Task BanGuildMember(string user, [Remainder] string reason = null)
        {
            if (string.IsNullOrEmpty(reason))
                reason = "There was no reason given.";
            IGuildUser bannedUser;
            if (GeneralFunctions.CheckUlong(user) == true && DiscordBot.CheckIsInGuild(Context.Guild.Id, Convert.ToUInt64(user)))
                bannedUser = await Context.Guild.GetUserAsync(Convert.ToUInt64(user));
            else if (GeneralFunctions.CheckUlong(user) == false && Context.Message.MentionedUserIds.Count >= 1 && DiscordBot.CheckIsInGuild(Context.Guild.Id, Context.Message.MentionedUserIds.First()))
                bannedUser = await Context.Guild.GetUserAsync(Context.Message.MentionedUserIds.First());
            else if ((GeneralFunctions.CheckUlong(user) == true && DiscordBot.CheckIsInGuild(Context.Guild.Id, Convert.ToUInt64(user)) == false) || (GeneralFunctions.CheckUlong(user) == false && Context.Message.MentionedUserIds.Count >= 1 && DiscordBot.CheckIsInGuild(Context.Guild.Id, Context.Message.MentionedUserIds.First()) == false))
            {
                await ReplyAsync("Error 0x2: This user was not found in this server.");
                return;
            }
            else if (GeneralFunctions.CheckUlong(user) == false || (GeneralFunctions.CheckUlong(user) == false && Context.Message.MentionedUserIds.Count == 0))
            {
                await ReplyAsync("Error 0x3: There was not a valid id or mention found.");
                return;
            }
            else
            {
                await ReplyAsync("Error 0x4: An error occured during command execution please check your input");
                return;
            }

            #region comment
            //Continue if:
            //1.  The user is in the guild.
            //2.  The user id is valid or if the mention is valid.                

            //Errors:

            //Error 1: invalid mention or id
            //Error 2: User not found
            //Error 3: Given too many users
            //Error 4: Unknown Error
            #endregion

            BanInfo bannedUserInfo = new BanInfo
            {
                UserId = bannedUser.Id,
                Username = bannedUser.Username,
                UserDiscriminator = bannedUser.Discriminator,
                UserNickname = bannedUser.Nickname,
                UserCreationDate = bannedUser.CreatedAt,
                JoinDate = bannedUser.JoinedAt,
                StaffId = Context.Message.Author.Id,
                StaffUsername = Context.Message.Author.Username,
                StaffDiscriminator = Context.Message.Author.Discriminator,
                Source = "Ban Command",
                Reason = reason
            };
            await Context.Guild.AddBanAsync(bannedUser, 0, DiscordBot.client.CurrentUser.Id.ToString() + " - " + reason);
            if (await DiscordBot.CheckIfBanned(Context.Guild.Id, bannedUserInfo.UserId) == true)
            {
                await ReplyAsync("The user was banned.");

                await DiscordBot.SendBanMessage(bannedUserInfo);
            }
            else
            {
                await ReplyAsync("Error 0x5: User was not banned for an unknown reason.");
            }
        }

        [Command("Unban")]
        [Name("Unbans a user from a server")]
        [Summary(@"Syntax: `/>Unban <User Id> [Reason]`" +
    "\nThis command is used to ban users in the current guild / server." +
    "\nNote that this command has to be executed in a server where you have the rights to ban users.")]
        [RequireUserPermission(GuildPermission.BanMembers, ErrorMessage = "Error 0x1: You don't have permissions to execute this command in this guild.")]
        public async Task UnbanGuildMember(ulong user, [Remainder] string reason = null)
        {
            if (GeneralFunctions.CheckUlong(user) == true){
                if (await DiscordBot.CheckIfBanned(Context.Guild.Id, user) == true)
                {
                    await Context.Guild.RemoveBanAsync(user);
                    await ReplyAsync($"The user with ID: {user} is unbanned.");
                }
                else
                {
                    await ReplyAsync("Error 0x3: This user doesn't seem to be banned. Please check your command input.");
                }
            }
            else
            {
                await ReplyAsync("Error 0x2: This doesn't seem like a valid user id. Please check your command input.");
            }
        }
    }

    public class OwnerCommands : ModuleBase
    {
        [Command("Send Rolemessage")]
        [Alias("SR")]
        [Name("Send Rolemessage")]
        [Summary(@"Syntax: `/>Send Rolemessage`" +
        "\nThis command is used to send the message in the #roles channel where people can react to get their roles.")]
        [RequireOwner(ErrorMessage = "Error 1x1: This command can only send by the owner of the bot.")]
        public async Task SendRoleMessage()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithColor(0, 255, 255);
            builder.WithTitle("Get your roles here:");
            builder.WithDescription("You can assign yourself roles to get be able to have more permissions, turn off and on notification and view more channels.\n" +
                "You can do this by simply reacting to this message with the appropriate emote.");
            builder.AddField("Stream Notifications", new Emoji("\U0001F514"), true);
            builder.AddField("Cyber Security", new Emoji("\U0001F575"), true);
            builder.AddField("Programming", new Emoji("\U0001F4BB"), true);

            RestUserMessage message = await DiscordBot.client.GetGuild(Context.Guild.Id).GetTextChannel(DiscordBot.roleChannelID).SendMessageAsync(null, false, builder.Build());
            await message.AddReactionsAsync(new Emoji[] { new Emoji("\U0001F514"), new Emoji("\U0001F575"), new Emoji("\U0001F4BB") });

            await ReplyAsync("The message has been send.");
        }
    }
}
