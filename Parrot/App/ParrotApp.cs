using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using Parrot.App.Common;
using Parrot.App.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public bool IsActive { get; set; } = false;

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

        }

        private void ChatGui_ChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
        {
            if (!IsActive) return;
            if (type == plugin.Configuration.sourceChat)
            {
                Logger.Info("Got message: \"" + message.TextValue + "\"");
            }
        }

        public void Dispose()
        {
            if (chatGui != null)
            {
                chatGui.ChatMessage -= ChatGui_ChatMessage;
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
