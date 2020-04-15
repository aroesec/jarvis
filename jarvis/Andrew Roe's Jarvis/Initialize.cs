using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Andrew_Roe_s_Jarvis.Classes;

namespace Andrew_Roe_s_Jarvis
{
    class Initialize
    {
        public static void ConsoleHandle()
        {
            ConsoleHandler.Start();
            Console.SetOut(ConsoleHandler.consoleWriter);
        }

        public static async Task Bot()
        {
            await DiscordBot.BuildServices();
            await DiscordBot.HookEvents();
            await DiscordBot.Login();
            await DiscordBot.StartAsync();
            new Thread(new ThreadStart(DiscordBot.LoopConsoleMessage)).Start();
        }

        public static async Task Subroutine()
        {
            var t = new Thread(new ThreadStart(Subroutines.BotGame));
            t.Priority = ThreadPriority.BelowNormal;
            t.Start();
        }
    }
}
