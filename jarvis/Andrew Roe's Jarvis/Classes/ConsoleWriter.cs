using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Andrew_Roe_s_Jarvis.Classes
{
    public class ConsoleWriterEventArgs : EventArgs
    {
        public string Value { get; private set; }
        public ConsoleWriterEventArgs(string value)
        {
            Value = value;
        }
    }
    public class ConsoleWriter : TextWriter
    {
        public override Encoding Encoding { get { return Encoding.UTF8; } }

        public override void Write(string value)
        {
            WriteEvent?.Invoke(this, new ConsoleWriterEventArgs(value));
        }

        public override void WriteLine(string value)
        {
            WriteLineEvent?.Invoke(this, new ConsoleWriterEventArgs(value));
        }

        public event EventHandler<ConsoleWriterEventArgs> WriteEvent;
        public event EventHandler<ConsoleWriterEventArgs> WriteLineEvent;
    }

    public class ConsoleHandler
    {
        public static ConsoleWriter consoleWriter;
        public static TextWriter defaultConsoleWriter;
        public static Queue<string> pendingMessages = new Queue<string>();



        public static void Start()
        {
            // Creates a new empty queue array for unsend messages.
            pendingMessages = new Queue<string>();

            defaultConsoleWriter = Console.Out;
            consoleWriter = new ConsoleWriter();

            // Adds the Console.Write and Console.WriteLine events for sending them to Discord.
            consoleWriter.WriteEvent += ConsoleWriteMessage;
            consoleWriter.WriteLineEvent += ConsoleWriteLineMessage;
        }

        private static void ConsoleWriteMessage(object sender, ConsoleWriterEventArgs e)
        {
            // The following three lines are used for displaying the console messages in the console.
            Console.SetOut(defaultConsoleWriter);
            Console.Write(e.Value);
            Console.SetOut(consoleWriter);

            // Adds the console message to the queue.
            pendingMessages.Enqueue(e.Value);
            // Send the message to the Discord console channel.
            //Bot.SendConsoleMessage();
        }
        private static void ConsoleWriteLineMessage(object sender, ConsoleWriterEventArgs e)
        {
            // The following three lines are used for displaying the console messages in the console.
            Console.SetOut(defaultConsoleWriter);
            Console.WriteLine(e.Value);
            Console.SetOut(consoleWriter);

            // Adds the console message to the queue.
            pendingMessages.Enqueue(e.Value);
            // Send the message to the Discord console channel.
            //Bot.SendConsoleMessage();
        }
    }
}
