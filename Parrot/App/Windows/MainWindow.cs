using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
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

        public MainWindow(Plugin plugin, ParrotApp app): base("Parrot", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            this.Size = new System.Numerics.Vector2(400, 200);
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
                    IconOffset = new(2,1),
                    ShowTooltip = () =>
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text("Open Parrot Settings");
                        ImGui.EndTooltip();
                    }
                }
            };
        }

        public void Dispose()
        {
            
        }

        public override void Draw()
        {
            ImGui.Text($"The random config bool is {this.plugin.Configuration.SomePropertyToBeSavedAndWithADefault}");
        }
    }
}
