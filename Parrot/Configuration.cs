using Dalamud.Configuration;
using Dalamud.Game.Text;
using Dalamud.Plugin;
using System;

namespace Parrot
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public string authToken { get; set; } = string.Empty;

        public string channelName { get; set; } = string.Empty;

        public XivChatType sourceChat {  get; set; } = XivChatType.None;

        // the below exist just to make saving less cumbersome
        [NonSerialized]
        private DalamudPluginInterface? PluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.PluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.PluginInterface!.SavePluginConfig(this);
        }
    }
}
