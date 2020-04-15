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
    public class EventHandler
    {
        public static async Task MessageReceived(SocketMessage socketMessage)
        {
            var message = (socketMessage as SocketUserMessage);

            // Don't process the command if it was a System Message.
            if (message == null) return;

            if (message.Channel.Id == DiscordBot.consoleChannelID && message.Author.Id == DiscordBot.client.CurrentUser.Id)
            {
                DiscordBot.consoleMessageReceived.Set();
            }

            else
            {
                // Create a number to track where the prefix ends and the command begins.
                int ArgPos = 0;

                bool HasStringPrefix = message.HasStringPrefix("/>", ref ArgPos);
                bool HasMentionPrefix = message.HasMentionPrefix(DiscordBot.client.CurrentUser, ref ArgPos);


                // Check if the message begins with the mention or character prefix.
                if (HasStringPrefix || HasMentionPrefix)
                {
                    // Create a Command Context
                    var Context = new CommandContext(DiscordBot.client, message);

                    // Execute the command. (result does not indicate a return value,
                    // rather an object stating if the command executed successfully)
                    var Result = await DiscordBot.commands.ExecuteAsync(Context, ArgPos, DiscordBot.services);
                    if (!Result.IsSuccess)
                    {
                        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} Error:      {Result.ErrorReason} \n                     Message received as: {message.Content}");
                        if (Result.ErrorReason.StartsWith("Authorisation error"))
                            await Context.Channel.SendMessageAsync(Result.ErrorReason, false, null);
                        else
                            await Context.Channel.SendMessageAsync("The command did not excecute correctly.", false, null);
                        return;
                    }

                    else if (Result.IsSuccess)
                    {
                        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} Command:    Command executed succesfully: {message.Content}");
                    }
                }
            }
        }

        public static async Task ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction emoji)
        {
            SocketGuild guild = DiscordBot.client.GetGuild(DiscordBot.mainGuildId);
            SocketGuildUser user = guild.GetUser(emoji.UserId);
            SocketGuildUser messageAuthor = guild.GetUser((await channel.GetMessageAsync(message.Id)).Author.Id);
            RestUserMessage replyMessage;
            if (channel.Id == DiscordBot.welcomeChannelID && emoji.Emote.Name == "👍")
            {
                if (messageAuthor.Roles.Count == 1)
                {
                    if (user.Roles.Contains(guild.GetRole(DiscordBot.staffRoleId)))
                    {
                        await messageAuthor.AddRoleAsync(guild.GetRole(DiscordBot.memberRoleId));
                    }
                    else
                    {
                        replyMessage = await channel.SendMessageAsync("Error 0x1: Permission Denied, you're not a staff member.");
                        await Task.Delay(3000);
                        await replyMessage.DeleteAsync();
                    }
                }
                else
                {
                    replyMessage = await channel.SendMessageAsync("Error 0x2: This user already has a role.");
                    await Task.Delay(3000);
                    await replyMessage.DeleteAsync();
                }
            }
            else if (channel.Id == DiscordBot.roleChannelID && message.Id == DiscordBot.roleMessageID && emoji.UserId != DiscordBot.client.CurrentUser.Id)
            {
                switch (emoji.Emote.Name)
                {
                    case "🔔":
                        await user.AddRoleAsync(guild.GetRole(DiscordBot.notificationRoleID));
                        break;
                    case "🕵":
                        await user.AddRoleAsync(guild.GetRole(DiscordBot.cyberSecurityRoleId));
                        break;
                    case "💻":
                        await user.AddRoleAsync(guild.GetRole(DiscordBot.programmingRoleID));
                        break;
                    default:
                        await (await channel.GetMessageAsync(message.Id) as RestUserMessage).RemoveReactionAsync(emoji.Emote, user);
                        break;
                }
            }
            else
                return;
        }

        public static async Task ReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction emoji)
        {
            if (channel.Id == DiscordBot.roleChannelID && message.Id == DiscordBot.roleMessageID && emoji.UserId != DiscordBot.client.CurrentUser.Id)
            {                
                SocketGuild guild = DiscordBot.client.GetGuild(DiscordBot.mainGuildId);
                SocketGuildUser user = guild.GetUser(emoji.UserId);
                switch (emoji.Emote.Name)
                {
                    case "🔔":
                        await user.RemoveRoleAsync(guild.GetRole(DiscordBot.notificationRoleID));
                        break;
                    case "🕵":
                        await user.RemoveRoleAsync(guild.GetRole(DiscordBot.cyberSecurityRoleId));
                        break;
                    case "💻":
                        await user.RemoveRoleAsync(guild.GetRole(DiscordBot.programmingRoleID));
                        break;
                    default:
                        await (await channel.GetMessageAsync(message.Id) as RestUserMessage).RemoveReactionAsync(emoji.Emote, user);
                        break;
                }
            }
            else
                return;
        }

        public static async Task UserBanned(SocketUser user, SocketGuild guild)
        {
            Console.WriteLine("ReceivedBanEvent");
            if ((await guild.GetBanAsync(user.Id)).Reason.Contains(DiscordBot.client.CurrentUser.Id.ToString()) == false)
            {
                BanInfo banInfo = new BanInfo()
                {
                    Source = "Ban Event",                    
                    Reason = (await guild.GetBanAsync(user.Id)).Reason,
                    UserId = user.Id,
                    Username = user.Username,
                    UserDiscriminator = user.Discriminator,
                    UserNickname = guild.GetUser(user.Id).Nickname,
                    UserCreationDate = user.CreatedAt,
                    JoinDate = guild.GetUser(user.Id).JoinedAt
                };
                await DiscordBot.SendBanMessage(banInfo);
            }
        }

        public static async Task UserJoined(SocketGuildUser newUser)
        {
            Console.WriteLine("A new user joined.");
            //await newUser.AddRoleAsync(DiscordBot.client.GetGuild(DiscordBot.mainGuildId).GetRole(595725541009653801));
            //await newUser.AddRoleAsync(DiscordBot.client.GetGuild(DiscordBot.mainGuildId).GetRole(568407055182856192));

            EmbedBuilder builder = new EmbedBuilder();

            builder.WithAuthor("From The Staff:");
            builder.WithColor(Color.Green);
            builder.WithThumbnailUrl(DiscordBot.client.GetGuild(DiscordBot.mainGuildId).IconUrl);
            builder.WithTitle($"Welcome {newUser.Username} to aRoe's Palace!");
            builder.WithDescription($"Hey {newUser.Mention}, in order to get you filtered through unwanted users please introduce yourself.\n\n" +
                $"Please go to {DiscordBot.client.GetGuild(DiscordBot.mainGuildId).GetTextChannel(DiscordBot.welcomeRulesChannelID).Mention} to get yourself up-to-date on our rules.\n\n");
            builder.AddField("A short note:", "Don't forget to check out the Twitch and YouTube channel");

            await DiscordBot.client.GetGuild(DiscordBot.mainGuildId).GetTextChannel(DiscordBot.welcomeChannelID).SendMessageAsync(newUser.Mention, false, builder.Build());
        }
    }
}
