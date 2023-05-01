using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_GPT_Bot
{
    internal class Program
    {
        // Ejecución del bot
        static void Main()
        {
            var bot = new Bot();
            bot.RunAsync().GetAwaiter().GetResult();
        }
    }
}
