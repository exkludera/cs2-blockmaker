using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;

namespace BlockMaker;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "Block Maker";
    public override string ModuleVersion => "0.0.3";
    public override string ModuleAuthor => "exkludera";

    public static Plugin _ { get; set; } = new();
    public Dictionary<CCSPlayerController, PlayerData> playerData = new Dictionary<CCSPlayerController, PlayerData>();
    public bool buildMode = false;

    public override void Load(bool hotReload)
    {
        _ = this;

        Files();

        RegisterEvents();

        Commands();

        Menu.Load(hotReload);

        if (hotReload)
        {
            savedBlocksPath = Path.Combine(blocksFolder, $"{GetMapName()}.json");

            foreach (var player in Utilities.GetPlayers().Where(p => !p.IsBot && !playerData.ContainsKey(p)))
            {
                playerData[player] = new();

                if (HasPermission(player))
                    playerData[player].Builder = true;
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
        Config.Settings.Prefix = StringExtensions.ReplaceColorTags(config.Settings.Prefix);

        buildMode = config.Settings.BuildMode;
    }
}