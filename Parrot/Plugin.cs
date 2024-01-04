using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Parrot.App.Common;
using Parrot.App;

namespace Parrot
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Parrot";
        private const string CommandName = "/parrot";

        private DalamudPluginInterface PluginInterface { get; init; }
        private ICommandManager CommandManager { get; init; }
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("Parrot");
        public IChatGui chatGui { get; init; }

        private ParrotApp app { get; init; }

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] ICommandManager commandManager,
            [RequiredVersion("1.0")] IChatGui chatGui,
            [RequiredVersion("1.0")] IPluginLog logger)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;
            this.chatGui = chatGui;
            Logger.initialize(logger);

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Open the main Parrot window."
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawPluginUI;

            this.app = new ParrotApp(this);

            WindowSystem.AddWindow(app.MainWindow);
            WindowSystem.AddWindow(app.ConfigWindow);
        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();
            
            this.CommandManager.RemoveHandler(CommandName);
        }

        private void OnCommand(string command, string args)
        {
            this.DrawPluginUI();
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void DrawPluginUI()
        {
            app.DrawMainUI();
        }
    }
}
