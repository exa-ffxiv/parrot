using System;
using System.Numerics;
using Dalamud.Game.Text;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace Parrot.App.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration configuration;

    public ConfigWindow(Plugin plugin) : base(
        "Parrot Configuration",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.Size = new Vector2(300, 300);
        this.SizeCondition = ImGuiCond.Always;

        this.configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
        var channelName = configuration.channelName;
        ImGui.SetNextItemWidth(150);
        if (ImGui.InputText("Channel Name", ref channelName, 99))
        {
            configuration.channelName = channelName;
            configuration.Save();
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("Twitch channel to parrot to. Captilization doesn't matter.");
        }

        var botAccountName = configuration.botAccountName;
        ImGui.SetNextItemWidth(150);
        if (ImGui.InputText("Twitch Account Name", ref botAccountName, 99))
        {
            configuration.botAccountName = botAccountName;
            configuration.Save();
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("Name of the twitch bot account.");
        }

        var authToken = configuration.authToken;
        ImGui.SetNextItemWidth(150);
        if (ImGui.InputText("Twitch Auth Token", ref authToken, 99))
        {
            configuration.authToken = authToken;
            configuration.Save();
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("Auth token to log into the twitch bot account.");
        }

        var sourceChat = configuration.sourceChat;
        ImGui.SetNextItemWidth(175);
        if (ImGui.BeginCombo("Parroted Chat", getChatName(sourceChat)))
        {
            if (ImGui.Selectable("None", sourceChat == XivChatType.None))
            {
                configuration.sourceChat = XivChatType.None;
                configuration.Save();
            }
            if (ImGui.Selectable("Crossworld Linkshell 1", sourceChat == XivChatType.CrossLinkShell1))
            {
                configuration.sourceChat = XivChatType.CrossLinkShell1;
                configuration.Save();
            }
            if (ImGui.Selectable("Crossworld Linkshell 2", sourceChat == XivChatType.CrossLinkShell2))
            {
                configuration.sourceChat = XivChatType.CrossLinkShell2;
                configuration.Save();
            }
            if (ImGui.Selectable("Crossworld Linkshell 3", sourceChat == XivChatType.CrossLinkShell3))
            {
                configuration.sourceChat = XivChatType.CrossLinkShell3;
                configuration.Save();
            }
            if (ImGui.Selectable("Crossworld Linkshell 4", sourceChat == XivChatType.CrossLinkShell4))
            {
                configuration.sourceChat = XivChatType.CrossLinkShell4;
                configuration.Save();
            }
            if (ImGui.Selectable("Crossworld Linkshell 5", sourceChat == XivChatType.CrossLinkShell5))
            {
                configuration.sourceChat = XivChatType.CrossLinkShell5;
                configuration.Save();
            }
            if (ImGui.Selectable("Crossworld Linkshell 6", sourceChat == XivChatType.CrossLinkShell6))
            {
                configuration.sourceChat = XivChatType.CrossLinkShell6;
                configuration.Save();
            }
            if (ImGui.Selectable("Crossworld Linkshell 7", sourceChat == XivChatType.CrossLinkShell7))
            {
                configuration.sourceChat = XivChatType.CrossLinkShell7;
                configuration.Save();
            }
            if (ImGui.Selectable("Crossworld Linkshell 8", sourceChat == XivChatType.CrossLinkShell8))
            {
                configuration.sourceChat = XivChatType.CrossLinkShell8;
                configuration.Save();
            }
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("The FFXIV chat channel to parrot.");
        }

        var mainWindowRequired = configuration.mainWindowRequired;
        if (ImGui.Checkbox("Main Window Required", ref mainWindowRequired))
        {
            configuration.mainWindowRequired = mainWindowRequired;
            configuration.Save();
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("If enabled, closing the main window will disable parroting.");
        }

        var delayEnabled = configuration.delayEnabled;
        if (ImGui.Checkbox("Message Delay Enabled", ref delayEnabled))
        {
            configuration.delayEnabled = delayEnabled;
            configuration.Save();
        }
        var delayMin = configuration.delayMin;
        var delayMax = configuration.delayMax;
        ImGui.SetNextItemWidth(100);
        if (ImGui.InputInt("Minimum Delay ms", ref delayMin, 1000))
        {
            if (delayMin > configuration.delayMax) delayMin = configuration.delayMax;
            configuration.delayMin = delayMin;
            configuration.Save();
        }
        ImGui.SetNextItemWidth(100);
        if (ImGui.InputInt("Maximum Delay ms", ref delayMax, 1000))
        {
            if (delayMax < configuration.delayMin) delayMax = configuration.delayMin;
            configuration.delayMax = delayMax;
            configuration.Save();
        }

        ImGui.Text("Required Prefix");
        var prefixEnabled = configuration.prefixEnabled;
        if (ImGui.Checkbox("###Prefix Enabled", ref prefixEnabled))
        {
            configuration.prefixEnabled = prefixEnabled;
            configuration.Save();
        }

        ImGui.SameLine();
        var prefix = configuration.prefix;
        if (ImGui.InputText("###Prefix", ref prefix, 50))
        {
            configuration.prefix = prefix;
            configuration.Save();
        }
    }

    private string getChatName(XivChatType sourceChat)
    {
        if (sourceChat == XivChatType.None)
        {
            return "None";
        }
        else
        {
            return sourceChat.GetDetails().FancyName;
        }
    }
}
