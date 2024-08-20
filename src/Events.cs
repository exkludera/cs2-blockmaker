using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using static CounterStrikeSharp.API.Core.Listeners;

namespace BlockMaker;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    public delegate HookResult EntityOutputHandler(CEntityIOOutput output, string name, CEntityInstance activator, CEntityInstance caller, CVariant value, float delay);

    public void RegisterEvents()
    {
        RegisterListener<OnTick>(OnTick);
        RegisterListener<OnMapStart>(OnMapStart);
        RegisterListener<OnMapEnd>(OnMapEnd);
        RegisterListener<OnServerPrecacheResources>(OnServerPrecacheResources);

        RegisterEventHandler<EventPlayerConnectFull>(EventPlayerConnectFull);
        RegisterEventHandler<EventRoundStart>(EventRoundStart);
        RegisterEventHandler<EventRoundEnd>(EventRoundEnd);
        RegisterEventHandler<EventPlayerDeath>(EventPlayerDeath);

        HookEntityOutput("*", "OnStartTouch", OnStartTouch, HookMode.Pre);
        HookEntityOutput("*", "OnTouching", OnTouching, HookMode.Pre);
        HookEntityOutput("*", "OnEndTouch", OnEndTouch, HookMode.Pre);
    }

    public void UnregisterEvents()
    {
        RemoveListener<OnTick>(OnTick);
        RemoveListener<OnMapStart>(OnMapStart);
        RemoveListener<OnMapEnd>(OnMapEnd);
        RemoveListener<OnServerPrecacheResources>(OnServerPrecacheResources);

        DeregisterEventHandler<EventPlayerConnectFull>(EventPlayerConnectFull);
        DeregisterEventHandler<EventRoundStart>(EventRoundStart);
        DeregisterEventHandler<EventRoundEnd>(EventRoundEnd);
        DeregisterEventHandler<EventPlayerDeath>(EventPlayerDeath);

        UnhookEntityOutput("*", "OnStartTouch", OnStartTouch, HookMode.Pre);
        UnhookEntityOutput("*", "OnTouching", OnTouching, HookMode.Pre);
        UnhookEntityOutput("*", "OnEndTouch", OnEndTouch, HookMode.Pre);
    }

    private HookResult OnStartTouch(CEntityIOOutput output, string name, CEntityInstance activator, CEntityInstance caller, CVariant value, float delay)
    {
        return HookResult.Continue;
    }

    private HookResult OnTouching(CEntityIOOutput output, string name, CEntityInstance activator, CEntityInstance caller, CVariant value, float delay)
    {
        return HookResult.Continue;
    }

    private HookResult OnEndTouch(CEntityIOOutput output, string name, CEntityInstance activator, CEntityInstance caller, CVariant value, float delay)
    {
        return HookResult.Continue;
    }

    public void OnServerPrecacheResources(ResourceManifest manifest)
    {
        foreach (var block in BlockModels.Values)
        {
            if (!string.IsNullOrEmpty(block.Small))
                manifest.AddResource(block.Small);

            if (!string.IsNullOrEmpty(block.Medium))
                manifest.AddResource(block.Medium);

            if (!string.IsNullOrEmpty(block.Large))
                manifest.AddResource(block.Large);

            if (!string.IsNullOrEmpty(block.Pole))
                manifest.AddResource(block.Pole);
        }
    }

    public void OnMapStart(string mapname)
    {
        savedBlocksPath = Path.Combine(blocksFolder, $"{GetMapName()}.json");

        if (Config.Settings.AutoSave)
        {
            AddTimer(Config.Settings.SaveTime, () => {
                PrintToChatAll("Auto-Saving Blocks");
                SaveBlocks();
            }, TimerFlags.STOP_ON_MAPCHANGE);
        }

        if (Config.Settings.BuildModeConfig)
        {
            string[] commands =
                {
                    "sv_cheats 1", "mp_join_grace_time 3600", "mp_timelimit 60",
                    "mp_roundtime 60", "mp_freezetime 0", "mp_warmuptime 0", "mp_maxrounds 99"
                };

            foreach (string command in commands)
                Server.ExecuteCommand(command);
        }
    }
    public void OnMapEnd()
    {
        playerData.Clear();
    }

    HookResult EventPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        var player = @event.Userid;

        if (player == null || player.NotValid())
            return HookResult.Continue;

        if (buildMode)
        {
            playerData[player] = new();
            PlayerHolds[player] = new();

            if (HasPermission(player))
                playerData[player].Builder = true;
        }

        return HookResult.Continue;
    }

    HookResult EventRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        Reset();

        SpawnBlocks();

        return HookResult.Continue;
    }

    HookResult EventRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        if (buildMode && Config.Settings.AutoSave)
            SaveBlocks();

        return HookResult.Continue;
    }

    HookResult EventPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        var player = @event.Userid;

        if (@event == null || player.NotValid())
            return HookResult.Continue;

        if (buildMode)
            AddTimer(1.0f, player!.RespawnClient);

        return HookResult.Continue;
    }
}