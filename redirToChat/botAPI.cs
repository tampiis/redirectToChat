using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace redirToChat
{
    internal class botAPI
    {
        private static TelegramBotClient botClient; 

        static botAPI() 
        {
            botClient = new TelegramBotClient(/*тут апи бота для отправки сообщений*/"");
        }

        public static void SendMessageToChat(string chatId, string message)
        {
            botClient.SendTextMessageAsync(chatId, message);
            Console.WriteLine(message);
        }
    }

}
