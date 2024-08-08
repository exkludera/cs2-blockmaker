using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using static CounterStrikeSharp.API.Core.Listeners;

namespace BlockBuilder;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "Block Builder";
    public override string ModuleVersion => "0.0.2";
    public override string ModuleAuthor => "exkludera";

    public static Plugin _ { get; set; } = new();
    private string directoryPath = string.Empty;
    private string savedBlocksPath = string.Empty;
    public Dictionary<CCSPlayerController, PlayerData> playerData = new Dictionary<CCSPlayerController, PlayerData>();
    public bool buildMode = false;

    public override void Load(bool hotReload)
    {
        _ = this;

        directoryPath = Path.Combine(ModuleDirectory, "blocks");

        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);
        
        savedBlocksPath = Path.Combine(directoryPath, $"{Server.MapName}.json");

        RegisterListener<OnTick>(OnTick);
        RegisterListener<OnMapStart>(OnMapStart);
        RegisterListener<OnMapEnd>(OnMapEnd);
        RegisterListener<OnServerPrecacheResources>(OnServerPrecacheResources);

        foreach (var command in Config.Menu.Command.Split(','))
            AddCommand($"{command}", "Open build menu", Menu.Command_OpenMenus!);

        foreach (var command in Config.BuildMode.Command.Split(','))
            AddCommand($"{command}", "Toggle build mode", Command_ToggleBuildMode!);

        Menu.Load(hotReload);

        if (hotReload)
        {
            foreach (var player in Utilities.GetPlayers().Where(p => !p.IsBot && !playerData.ContainsKey(p)))
                playerData[player] = new();

            foreach (var block in Utilities.GetAllEntities().Where(b => b.DesignerName == "prop_physics_override"))
                block.Remove();

            SpawnBlocks();
        }
    }

    public override void Unload(bool hotReload)
    {
        RemoveListener<OnTick>(OnTick);
        RemoveListener<OnMapStart>(OnMapStart);
        RemoveListener<OnMapEnd>(OnMapEnd);
        RemoveListener<OnServerPrecacheResources>(OnServerPrecacheResources);
    }

    public Config Config { get; set; } = new Config();
    public void OnConfigParsed(Config config)
    {
        Config = config;
        Config.Prefix = StringExtensions.ReplaceColorTags(config.Prefix);

        buildMode = config.BuildMode.Enabled;
    }

    public void OnMapStart(string mapname)
    {
        savedBlocksPath = Path.Combine(ModuleDirectory, $"blocks/{Server.MapName}.json");

        if (Config.AutoSave.Enabled)
        {
            AddTimer(Config.AutoSave.Time, () => {
                PrintToChatAll("Auto-Saving Blocks");
                SaveBlocks();
            }, TimerFlags.STOP_ON_MAPCHANGE);
        }
    }
    public void OnMapEnd()
    {
        playerData.Clear();
    }

    public void OnServerPrecacheResources(ResourceManifest manifest)
    {
        foreach (var block in Config.Blocks.Values)
        {
            PrecacheResource(manifest, block.Small);
            PrecacheResource(manifest, block.Medium);
            PrecacheResource(manifest, block.Large);
            PrecacheResource(manifest, block.Pole);
        }
    }
}