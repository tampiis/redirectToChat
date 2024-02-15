using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TL;
using static System.Collections.Specialized.BitVector32;
using WTelegram;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Net.Mime.MediaTypeNames;

namespace redirToChat
{
    

    static class Program_ListenUpdates
    {
        static WTelegram.Client Client;
        static User My;
        static readonly Dictionary<long, User> Users = new();
        static readonly Dictionary<long, ChatBase> Chats = new();




        // go to Project Properties > Debug > Environment variables and add at least these: api_id, api_hash, phone_number
        public static async Task Main()
        {
            Console.WriteLine("Program started");

            WTelegram.Helpers.Log = (l, s) => System.Diagnostics.Debug.WriteLine(s);
            Client = new WTelegram.Client(Environment.GetEnvironmentVariable);
            using (Client)
            {
                Client.OnUpdate += Client_OnUpdate;
                My = await Client.LoginUserIfNeeded();
                Users[My.id] = My;
                
                Console.WriteLine($"We are logged-in as {My.username ?? My.first_name + " " + My.last_name} (id {My.id})");
                
                var dialogs = await Client.Messages_GetAllDialogs(); 




                dialogs.CollectUsersChats(Users, Chats);
                Console.ReadKey();


            }

        }
        public static async Task mainChecker()
        {

        }

        private static async Task Client_OnUpdate(UpdatesBase updates)
        {

           
            updates.CollectUsersChats(Users, Chats);
            if (updates is UpdateShortMessage usm && !Users.ContainsKey(usm.user_id))
                (await Client.Updates_GetDifference(usm.pts - usm.pts_count, usm.date, 0)).CollectUsersChats(Users, Chats);
            else if (updates is UpdateShortChatMessage uscm && (!Users.ContainsKey(uscm.from_id) || !Chats.ContainsKey(uscm.chat_id)))
                (await Client.Updates_GetDifference(uscm.pts - uscm.pts_count, uscm.date, 0)).CollectUsersChats(Users, Chats);

            foreach (var update in updates.UpdateList)
                if (update is UpdateNewMessage unm)
                {
                    await HandleMessage(Client, unm.message);
                }
        }

      
        private static async Task HandleMessage(WTelegram.Client client, MessageBase messageBase)
        {
            if (messageBase is Message m && m.peer_id is PeerChannel pc && (pc.channel_id == 1787686698 || pc.channel_id == 2003893623))
            {
                ProcessMessage(m.message, client);
            }

        }
        static async void ProcessMessage(string message, WTelegram.Client client)
        {
            
            string outMessage = "";
            if (message.StartsWith("🔴Мамонт отклонил пермишены"))
            {
                string worker = ExtractWorker(message);
                outMessage = $"🔴Мамонт отклонил пермишены:\n\nВоркер 👨🏼‍💻: {worker}";
                
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                botAPI.SendMessageToChat("ТУТ ЧАТ, КУДА ОТПРАВЛЯТЬ СООБЩЕНИЕ", outMessage);

            }
            else if (message.StartsWith("🟢 Приложение запущено 🟢"))
            {
                string worker = ExtractWorker(message);
                outMessage = $"Лог ✍️\n\nВоркер 👨🏼‍💻: {worker}";

                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                botAPI.SendMessageToChat("ТУТ ЧАТ, КУДА ОТПРАВЛЯТЬ СООБЩЕНИE", outMessage);
                
            }
        }

        static string ExtractWorker(string message)
        {
            int workerIndex = message.IndexOf("👔Воркер:");
            if (workerIndex != -1)
            {
                int startIndex = workerIndex + "👔Воркер:".Length;
                int endIndex = message.IndexOf('\n', startIndex);
                if (endIndex != -1)
                {
                    return message.Substring(startIndex, endIndex - startIndex).Trim();
                }
            }

            return "Произошла ошибка"; 
        }


        private static string User(long id) => Users.TryGetValue(id, out var user) ? user.ToString() : $"User {id}";
        private static string Chat(long id) => Chats.TryGetValue(id, out var chat) ? chat.ToString() : $"Chat {id}";
        private static string Peer(Peer peer) => peer is null ? null : peer is PeerUser user ? User(user.user_id)
            : peer is PeerChat or PeerChannel ? Chat(peer.ID) : $"Peer {peer.ID}";


    }
}