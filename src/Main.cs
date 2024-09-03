using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "Block Maker";
    public override string ModuleVersion => "0.0.6";
    public override string ModuleAuthor => "exkludera";

    public static Plugin Instance { get; set; } = new();
    public Dictionary<int, PlayerData> playerData = new Dictionary<int, PlayerData>();
    public bool buildMode = false;

    public override void Load(bool hotReload)
    {
        Instance = this;

        Files();

        RegisterEvents();

        Commands();

        Blocks.Load();

        Menu.Load(hotReload);

        if (hotReload)
        {
            savedBlocksPath = Path.Combine(blocksFolder, $"{GetMapName()}.json");

            foreach (var player in Utilities.GetPlayers().Where(p => !p.IsBot && !playerData.ContainsKey(p.Slot)))
            {
                playerData[player.Slot] = new();

                if (HasPermission(player))
                    playerData[player.Slot].Builder = true;
            }

            foreach (var block in Utilities.GetAllEntities().Where(b => b.DesignerName == "prop_physics_override"))
                block.Remove();

            SpawnBlocks();
        }
    }

    public override void Unload(bool hotReload)
    {
        UnregisterEvents();

        playerData.Clear();
    }

    public Config Config { get; set; } = new Config();
    public void OnConfigParsed(Config config)
    {
        Config = config;
        Config.Settings.Main.Prefix = StringExtensions.ReplaceColorTags(config.Settings.Main.Prefix);

        buildMode = config.Settings.Building.BuildMode;
    }
}