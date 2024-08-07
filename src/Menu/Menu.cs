using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;
using static CounterStrikeSharp.API.Core.Listeners;
using static BlockBuilder.Plugin;
using CounterStrikeSharp.API.Modules.Utils;

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
                Open_Chat(player);
                break;
            case "html":
            case "center":
            case "centerhtml":
            case "hud":
                Open_HTML(player);
                break;
            case "wasd":
            case "wasdmenu":
                Open_WASD(player);
                break;
            default:
                Open_HTML(player);
                break;
        }
    }

    public static void Open_Chat(CCSPlayerController player)
    {
        ChatMenu MainMenu = new("Block Builder");

        MainMenu.AddMenuOption("place", (player, menuOption) =>
        {
            _.Command_CreateBlock(player);
        });

        MainMenu.AddMenuOption("delete", (player, menuOption) =>
        {
            _.Command_DeleteBlock(player);
        });

        MainMenu.AddMenuOption("rotate", (player, menuOption) =>
        {
            ChatMenu RotateMenu = new("Rotate Block");

            RotateMenu.AddMenuOption("Horizontal", (player, menuOption) =>
            {
                _.Command_RotateBlock(player, false);
            });

            RotateMenu.AddMenuOption("Vertical", (player, menuOption) =>
            {
                _.Command_RotateBlock(player, true);
            });

            MenuManager.OpenChatMenu(player, RotateMenu);
        });

        MainMenu.AddMenuOption("select type", (player, menuOption) =>
        {
            ChatMenu BlockMenu = new("Select Block");

            foreach (var block in _.Config.Blocks)
            {
                string blockName = block.Key;

                BlockMenu.AddMenuOption(blockName, (player, menuOption) =>
                {
                    if (_.playerData.TryGetValue(player, out PlayerData data))
                        data.selectedBlock = blockName;

                    _.PrintToChat(player, $"Selected Block: {ChatColors.White}{blockName}");

                    MenuManager.OpenChatMenu(player, MainMenu);
                });
            }

            MenuManager.OpenChatMenu(player, BlockMenu);
        });

        MainMenu.AddMenuOption("select size", (player, menuOption) =>
        {
            ChatMenu SizeMenu = new("Select Size");

            SizeMenu.AddMenuOption("Default", (player, menuOption) =>
            {
                if (_.playerData.TryGetValue(player, out PlayerData data))
                    data.selectedSize = "";

                _.PrintToChat(player, $"Selected Size: {ChatColors.White}Default");

                MenuManager.OpenChatMenu(player, MainMenu);
            });

            SizeMenu.AddMenuOption("Small", (player, menuOption) =>
            {
                if (_.playerData.TryGetValue(player, out PlayerData data))
                    data.selectedSize = "small";

                _.PrintToChat(player, $"Selected Size: {ChatColors.White}Small");

                MenuManager.OpenChatMenu(player, MainMenu);
            });

            SizeMenu.AddMenuOption("Large", (player, menuOption) =>
            {
                if (_.playerData.TryGetValue(player, out PlayerData data))
                    data.selectedSize = "large";

                _.PrintToChat(player, $"Selected Size: {ChatColors.White}Large");

                MenuManager.OpenChatMenu(player, MainMenu);
            });

            SizeMenu.AddMenuOption("Pole", (player, menuOption) =>
            {
                if (_.playerData.TryGetValue(player, out PlayerData data))
                    data.selectedSize = "pole";

                _.PrintToChat(player, $"Selected Size: {ChatColors.White}Pole");

                MenuManager.OpenChatMenu(player, MainMenu);
            });

            MenuManager.OpenChatMenu(player, SizeMenu);
        });

        MainMenu.AddMenuOption("save blocks", (player, menuOption) =>
        {
            _.Command_SaveBlocks(player);
        });

        MenuManager.OpenChatMenu(player, MainMenu);
    }

    public static void Open_HTML(CCSPlayerController player)
    {
        CenterHtmlMenu MainMenu = new("Block Builder", _);

        MainMenu.AddMenuOption("place", (player, menuOption) =>
        {
            _.Command_CreateBlock(player);
        });

        MainMenu.AddMenuOption("delete", (player, menuOption) =>
        {
            _.Command_DeleteBlock(player);
        });

        MainMenu.AddMenuOption("rotate", (player, menuOption) =>
        {
            CenterHtmlMenu RotateMenu = new("Rotate Block", _);

            RotateMenu.AddMenuOption("Horizontal", (player, menuOption) =>
            {
                _.Command_RotateBlock(player, false);
            });

            RotateMenu.AddMenuOption("Vertical", (player, menuOption) =>
            {
                _.Command_RotateBlock(player, true);
            });

            MenuManager.OpenCenterHtmlMenu(_, player, RotateMenu);
        });

        MainMenu.AddMenuOption("select type", (player, menuOption) =>
        {
            CenterHtmlMenu BlockMenu = new("Select Block", _);

            foreach (var block in _.Config.Blocks)
            {
                string blockName = block.Key;

                BlockMenu.AddMenuOption(blockName, (player, menuOption) =>
                {
                    if (_.playerData.TryGetValue(player, out PlayerData data))
                        data.selectedBlock = blockName;

                    _.PrintToChat(player, $"Selected Block: {ChatColors.White}{blockName}");

                    MenuManager.OpenCenterHtmlMenu(_, player, MainMenu);
                });
            }

            MenuManager.OpenCenterHtmlMenu(_, player, BlockMenu);
        });

        MainMenu.AddMenuOption("select size", (player, menuOption) =>
        {
            CenterHtmlMenu SizeMenu = new("Select Size", _);

            SizeMenu.AddMenuOption("Default", (player, menuOption) =>
            {
                if (_.playerData.TryGetValue(player, out PlayerData data))
                    data.selectedSize = "";

                _.PrintToChat(player, $"Selected Size: {ChatColors.White}Default");

                MenuManager.OpenCenterHtmlMenu(_, player, MainMenu);
            });

            SizeMenu.AddMenuOption("Small", (player, menuOption) =>
            {
                if (_.playerData.TryGetValue(player, out PlayerData data))
                    data.selectedSize = "small";

                _.PrintToChat(player, $"Selected Size: {ChatColors.White}Small");

                MenuManager.OpenCenterHtmlMenu(_, player, MainMenu);
            });

            SizeMenu.AddMenuOption("Large", (player, menuOption) =>
            {
                if (_.playerData.TryGetValue(player, out PlayerData data))
                    data.selectedSize = "large";

                _.PrintToChat(player, $"Selected Size: {ChatColors.White}Large");

                MenuManager.OpenCenterHtmlMenu(_, player, MainMenu);
            });

            SizeMenu.AddMenuOption("Pole", (player, menuOption) =>
            {
                if (_.playerData.TryGetValue(player, out PlayerData data))
                    data.selectedSize = "pole";

                _.PrintToChat(player, $"Selected Size: {ChatColors.White}Pole");

                MenuManager.OpenCenterHtmlMenu(_, player, MainMenu);
            });

            MenuManager.OpenCenterHtmlMenu(_, player, SizeMenu);
        });

        MainMenu.AddMenuOption("save blocks", (player, menuOption) =>
        {
            _.Command_SaveBlocks(player);
        });

        MenuManager.OpenCenterHtmlMenu(_, player, MainMenu);
    }

    public static void Open_WASD(CCSPlayerController player)
    {
        IWasdMenu MainMenu = WasdManager.CreateMenu("Block Builder");

        MainMenu.Add("place", (player, menuOption) =>
        {
            _.Command_CreateBlock(player);
        });

        MainMenu.Add("delete", (player, menuOption) =>
        {
            _.Command_DeleteBlock(player);
        });

        MainMenu.Add("rotate", (player, menuOption) =>
        {
            IWasdMenu RotateMenu = WasdManager.CreateMenu("Rotate Block");

            RotateMenu.Add("Horizontal", (player, menuOption) =>
            {
                _.Command_RotateBlock(player, false);
            });

            RotateMenu.Add("Vertical", (player, menuOption) =>
            {
                _.Command_RotateBlock(player, true);
            });

            WasdManager.OpenSubMenu(player, RotateMenu);
        });

        MainMenu.Add("select type", (player, menuOption) =>
        {
            IWasdMenu BlockMenu = WasdManager.CreateMenu("Select Block");

            foreach (var block in _.Config.Blocks)
            {
                string blockName = block.Key;

                BlockMenu.Add(blockName, (player, menuOption) =>
                {
                    if (_.playerData.TryGetValue(player, out PlayerData data))
                        data.selectedBlock = blockName;

                    _.PrintToChat(player, $"Selected Block: {ChatColors.White}{blockName}");
                });
            }

            WasdManager.OpenSubMenu(player, BlockMenu);
        });

        MainMenu.Add("select size", (player, menuOption) =>
        {
            IWasdMenu SizeMenu = WasdManager.CreateMenu("Select Size");

            SizeMenu.Add("Default", (player, menuOption) =>
            {
                if (_.playerData.TryGetValue(player, out PlayerData data))
                    data.selectedSize = "";

                _.PrintToChat(player, $"Selected Size: {ChatColors.White}Default");
            });

            SizeMenu.Add("Small", (player, menuOption) =>
            {
                if (_.playerData.TryGetValue(player, out PlayerData data))
                    data.selectedSize = "small";

                _.PrintToChat(player, $"Selected Size: {ChatColors.White}Small");
            });

            SizeMenu.Add("Large", (player, menuOption) =>
            {
                if (_.playerData.TryGetValue(player, out PlayerData data))
                    data.selectedSize = "large";

                _.PrintToChat(player, $"Selected Size: {ChatColors.White}Large");
            });

            SizeMenu.Add("Pole", (player, menuOption) =>
            {
                if (_.playerData.TryGetValue(player, out PlayerData data))
                    data.selectedSize = "pole";

                _.PrintToChat(player, $"Selected Size: {ChatColors.White}Pole");
            });

            WasdManager.OpenSubMenu(player, SizeMenu);
        });

        MainMenu.Add("save blocks", (player, menuOption) =>
        {
            _.Command_SaveBlocks(player);
        });

        WasdManager.OpenMainMenu(player, MainMenu);
    }
}