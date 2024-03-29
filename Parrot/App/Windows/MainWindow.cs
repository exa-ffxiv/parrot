using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using Parrot.App.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parrot.App.Windows
{
    public class MainWindow : Window, IDisposable
    {
        private Plugin plugin {  get; init; }
        private ParrotApp app { get; init; }

        private Vector4 disabledColor = new Vector4(100, 0, 0, 1);
        private Vector4 enabledColor = new Vector4(0, 100, 0, 1);

        public MainWindow(Plugin plugin, ParrotApp app): base("Parrot", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            this.Size = new System.Numerics.Vector2(225, 100);
            this.SizeCondition = ImGuiCond.Appearing;

            this.plugin = plugin;
            this.app = app;

            TitleBarButtons = new()
            {
                new TitleBarButton()
                {
                    Icon = FontAwesomeIcon.Cog,
                    Click = (msg) =>
                    {
                        app.DrawConfigUI();
                    },
                    IconOffset = new(3,1),
                    ShowTooltip = () =>
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text("Open Parrot Settings");
                        ImGui.EndTooltip();
                    }
                }
            };
        }

        public void Dispose() { }

        public override void Draw()
        {
            ImGui.Text($"Parroting '{plugin.Configuration.sourceChat}'");
            ImGui.Text($" to '{plugin.Configuration.channelName}' is");

            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Text, app.IsActive ? enabledColor : disabledColor);
            if (app.IsActive)
            {
                if (ImGui.Button("ENABLED"))
                {
                    app.LogoutAndDisable();
                }
            }
            else
            {
                if (ImGui.Button("DISABLED"))
                {
                    app.LoginAndEnable();
                }
            }
            ImGui.PopStyleColor();
        }

        public override void OnClose()
        {
            base.OnClose();
            if (plugin.Configuration.mainWindowRequired && app.IsActive)
            {
                Logger.Info("Closing mainWindow while required, disabling parroting.");
                app.LogoutAndDisable();
            }
        }
    }
}
