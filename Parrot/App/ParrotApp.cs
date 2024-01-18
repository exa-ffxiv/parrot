using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.Havok;
using Parrot.App.Common;
using Parrot.App.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Parrot.App
{
    public class ParrotApp: IDisposable
    {
        private Plugin plugin { get; init; }
        private IChatGui? chatGui { get; init; }

        public MainWindow? MainWindow { get; init; }
        public ConfigWindow? ConfigWindow { get; init; }

        public bool IsActive { get; private set; } = false;

        private Task? activeTask = null;
        private CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        StreamReader? reader = null;
        StreamWriter? writer = null;

        Random rand = new Random();

        public ParrotApp(Plugin plugin)
        {
            if (plugin == null)
            {
                Logger.Error("Plugin null, Parrot in bad state and unable to work.");
            }
            else if (plugin.chatGui == null)
            {
                Logger.Error("ChatGui null, Parrot in bad state and unable to work.");
            }

            ArgumentNullException.ThrowIfNull(plugin);
            ArgumentNullException.ThrowIfNull(plugin.chatGui);

            this.plugin = plugin;
            this.chatGui = plugin.chatGui;
            this.MainWindow = new MainWindow(plugin, this);
            this.ConfigWindow = new ConfigWindow(plugin);

            chatGui.ChatMessage += ChatGui_ChatMessage;
        }

        public void LoginAndEnable()
        {
            IsActive = true;
            cancelTokenSource = new CancellationTokenSource();
            activeTask = Task.Run(() => TriggerChatBot(cancelTokenSource.Token));
        }

        public void LogoutAndDisable()
        {
            Logger.Info("Logging out of twitch and disabling parroting.");
            cancelTokenSource.Cancel();
            IsActive = false;
            activeTask = null;
            writer?.WriteLine($"QUIT :Gone to have lunch.");
        }

        private async Task TriggerChatBot(CancellationToken cancelToken)
        {
            Logger.Info("Signing in to twitch and enabling parroting.");
            var ip = "irc.chat.twitch.tv";
            var port = 6667;

            var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(ip, port);

            reader = new StreamReader(tcpClient.GetStream());
            writer = new StreamWriter(tcpClient.GetStream()) { NewLine = "\r\n", AutoFlush = true };

            await writer.WriteLineAsync($"PASS {plugin.Configuration.authToken}");
            await writer.WriteLineAsync($"NICK {plugin.Configuration.botAccountName}");

            while (IsActive)
            {
                var line = await reader.ReadLineAsync(cancelToken);
                if (line != null)
                {
                    Logger.Debug(line);

                    if (line.StartsWith("PING"))
                    {
                        var split = line.Split(" ");
                        Logger.Debug($"PING PONG {split[1]}");
                        await writer.WriteLineAsync($"PONG {split[1]}");
                    }
                }

                if (cancelToken.IsCancellationRequested)
                {
                    Logger.Debug("Quiting bot loop."); // Why the fuck doesn't this log?
                    LogoutAndDisable();
                }
            }
        }

        private void ChatGui_ChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            if (!IsActive) return;
            if (type == plugin.Configuration.sourceChat)
            {
                Logger.Debug("Got message: \"" + message.TextValue + "\"");
                var messageText = message.TextValue;
                Task.Run(() => SendChatMessage(messageText));
                //writer?.WriteLineAsync($"PRIVMSG #{plugin.Configuration.channelName} :{message.TextValue}");
            }
        }

        private async void SendChatMessage(String message)
        {
            await Task.Delay(rand.Next(plugin.Configuration.delayMin, plugin.Configuration.delayMax));
            writer?.WriteLineAsync($"PRIVMSG #{plugin.Configuration.channelName} :{message}");
        }

        public void Dispose()
        {
            if (chatGui != null)
            {
                chatGui.ChatMessage -= ChatGui_ChatMessage;
            }
            if (IsActive)
            {
                LogoutAndDisable();
            }
        }

        public void DrawMainUI()
        {
            if (MainWindow != null) MainWindow.IsOpen = true;
        }

        public void DrawConfigUI()
        {
            if (ConfigWindow != null) ConfigWindow.IsOpen = true;
        }
    }
}
