using System.Threading.Tasks;

namespace Andrew_Roe_s_Jarvis
{
    class Program
    {
        static void Main() => new Program().Start().GetAwaiter().GetResult();

        public async Task Start()
        {
            // Initialize the discord bot. (Login to Discord, setup various services and handlers such as a message handler, a user joined handler and command handler)
            await Initialize.Bot();

            // Initialize the CMH (Console Message Handler).
            Initialize.ConsoleHandle();

            // Initialize other things here. Add the initialisation code to the Initialize.cs.
            Initialize.Subroutine();

            // Make sure the Start function doesn't return a result to the Main function causing it to close the program and all it's sub processes.
            await Task.Delay(-1);
        }
    }    
}
