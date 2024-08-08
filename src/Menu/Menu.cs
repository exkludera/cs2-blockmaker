using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using static CounterStrikeSharp.API.Core.Listeners;
using static BlockBuilder.Plugin;

namespace BlockBuilder;

public static class Menu
{
    public static readonly Dictionary<int, WasdMenuPlayer> Players = new();

    public static void Load(bool hotReload)
    {
        _.RegisterListener<OnTick>(OnTick);

        _.RegisterEventHandler<EventPlayerActivate>((@event, info) =>
        {
            CCSPlayerController? player = @event.Userid;

            if (player == null || !player.IsValid || player.IsBot)
                return HookResult.Continue;

            Players[player.Slot] = new WasdMenuPlayer
            {
                player = player,
                Buttons = 0
            };

            return HookResult.Continue;
        });

        _.RegisterEventHandler<EventPlayerDisconnect>((@event, info) =>
        {
            CCSPlayerController? player = @event.Userid;

            if (player == null || !player.IsValid || player.IsBot)
                return HookResult.Continue;

            Players.Remove(player.Slot);

            return HookResult.Continue;
        });

        if (hotReload)
        {
            foreach (CCSPlayerController player in Utilities.GetPlayers())
            {
                if (player.IsBot)
                    continue;

                Players[player.Slot] = new WasdMenuPlayer
                {
                    player = player,
                    Buttons = player.Buttons
                };
            }
        }
    }

    public static void Unload()
    {
        _.RemoveListener<OnTick>(OnTick);
    }

    public static void OnTick()
    {
        foreach (WasdMenuPlayer? player in Players.Values.Where(p => p.MainMenu != null))
        {
            if ((player.Buttons & PlayerButtons.Forward) == 0 && (player.player.Buttons & PlayerButtons.Forward) != 0)
                player.ScrollUp();

            else if ((player.Buttons & PlayerButtons.Back) == 0 && (player.player.Buttons & PlayerButtons.Back) != 0)
                player.ScrollDown();

            else if ((player.Buttons & PlayerButtons.Moveright) == 0 && (player.player.Buttons & PlayerButtons.Moveright) != 0)
                player.Choose();

            else if ((player.Buttons & PlayerButtons.Moveleft) == 0 && (player.player.Buttons & PlayerButtons.Moveleft) != 0)
                player.CloseSubMenu();

            if (((long)player.player.Buttons & 8589934592) == 8589934592)
                player.OpenMainMenu(null);

            player.Buttons = player.player.Buttons;

            if (player.CenterHtml != "")
            {
                Server.NextFrame(() =>
                    player.player.PrintToCenterHtml(player.CenterHtml)
                );
            }
        }
    }


    [CommandHelper(minArgs: 0, whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public static void Command_OpenMenus(CCSPlayerController player, CommandInfo info)
    {
        if (!_.BuildMode(player))
            return;

        switch (_.Config.Menu.Type.ToLower())
        {
            case "chat":
            case "text":
                MenuChat.OpenMenu(player);
                break;
            case "html":
            case "center":
            case "centerhtml":
            case "hud":
                MenuHTML.OpenMenu(player);
                break;
            case "wasd":
            case "wasdmenu":
                MenuWASD.OpenMenu(player);
                break;
            default:
                MenuHTML.OpenMenu(player);
                break;
        }
    }
}