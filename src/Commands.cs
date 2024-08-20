using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

namespace BlockMaker;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    public void Commands()
    {
        Command_BuildMode(null, true);
        Command_ManageBuilder(null, true, null!);
        Command_BuildMenu(null, true, null!);
        Command_CreateBlock(null, true);
        Command_DeleteBlock(null, true);
        Command_RotateBlock(null, true, null!);
        Command_SaveBlocks(null, true);
        Command_Snapping(null, true);
        Command_Grid(null, true);
    }

    public void ToggleCommand(CCSPlayerController player, ref bool commandStatus, string commandName)
    {
        commandStatus = !commandStatus;

        string status = commandStatus ? "Enabled" : "Disabled";
        char color = commandStatus ? ChatColors.Green : ChatColors.Red;

        PrintToChat(player, $"{commandName}: {color}{status}");
    }

    public void Command_BuildMode(CCSPlayerController? player, bool load)
    {
        if (load)
        {
            foreach (var cmd in Config.AdminCommands.BuildMode.Split(','))
            {
                AddCommand($"css_{cmd}", "Toggle build mode", (player, command) => Command_BuildMode(player, false));
            }

            return;
        }

        if (player == null)
            return;

        if (!HasPermission(player))
        {
            PrintToChatAll($"{ChatColors.Red}You don't have permission to change Build Mode");
            return;
        }

        if (!buildMode)
        {
            buildMode = true;
            foreach (var target in Utilities.GetPlayers().Where(p => !p.IsBot && !playerData.ContainsKey(p) && !PlayerHolds.ContainsKey(p)))
            {
                playerData[target] = new();
                PlayerHolds[target] = new();
                if (HasPermission(target))
                    playerData[target].Builder = true;
            }
        }
        else
        {
            buildMode = false;
            playerData.Clear();
            PlayerHolds.Clear();
        }

        string status = buildMode ? "Enabled" : "Disabled";
        char color = buildMode ? ChatColors.Green : ChatColors.Red;

        PrintToChatAll($"Build Mode: {color}{status} {ChatColors.Grey}by {ChatColors.LightPurple}{player.PlayerName}");
    }

    public void Command_ManageBuilder(CCSPlayerController? player, bool load, CommandInfo command)
    {
        if (load)
        {
            foreach (var cmd in Config.AdminCommands.ManageBuilder.Split(','))
            {
                AddCommand($"css_{cmd}", "Manage builder", (player, command) => Command_ManageBuilder(player, false, command));
            }

            return;
        }

        if (player == null)
            return;

        if (!BuildMode(player))
            return;

        if (!HasPermission(player))
        {
            PrintToChat(player, $"{ChatColors.Red}You don't have permission to manage Builders");
            return;
        }

        string input = command.ArgString;

        if (string.IsNullOrEmpty(input))
        {
            PrintToChat(player, $"{ChatColors.Red}No player specified");
            return;
        }

        var targetPlayer = Utilities.GetPlayers()
            .FirstOrDefault(target => target.PlayerName.Contains(input, StringComparison.OrdinalIgnoreCase) || target.SteamID.ToString() == input);

        if (targetPlayer == null)
        {
            PrintToChat(player, $"{ChatColors.Red}Player not found");
            return;
        }

        var builderStatus = playerData[targetPlayer].Builder;
        playerData[targetPlayer].Builder = !builderStatus;

        var action = builderStatus ? "removed" : "granted";
        var color = builderStatus ? ChatColors.Red : ChatColors.Green;

        PrintToChat(targetPlayer, $"{ChatColors.LightPurple}{player.PlayerName} {color}{action} your access to Build");
        PrintToChat(player, $"{color}You {action} {ChatColors.LightPurple}{targetPlayer.PlayerName} {color}access to Build");
    }

    public void Command_BuildMenu(CCSPlayerController? player, bool load, CommandInfo command)
    {
        if (load)
        {
            foreach (var cmd in Config.BuildCommands.BuildMenu.Split(','))
            {
                AddCommand($"css_{cmd}", "Open build menu", (player, command) => Command_BuildMenu(player, false, command));
            }

            return;
        }

        if (player == null)
            return;

        if (!BuildMode(player))
            return;

        Menu.Command_OpenMenus(player, command);
    }

    public void Command_CreateBlock(CCSPlayerController? player, bool load)
    {
        if (load)
        {
            foreach (var cmd in Config.BuildCommands.CreateBlock.Split(','))
            {
                AddCommand($"css_{cmd}", "Create block", (player, command) => Command_CreateBlock(player, false));
            }

            return;
        }

        if (player == null)
            return;

        if (!BuildMode(player))
            return;

        CreateBlock(player);
    }

    public void Command_DeleteBlock(CCSPlayerController? player, bool load)
    {
        if (load)
        {
            foreach (var cmd in Config.BuildCommands.DeleteBlock.Split(','))
            {
                AddCommand($"css_{cmd}", "Delete block", (player, command) => Command_DeleteBlock(player, false));
            }

            return;
        }

        if (player == null)
            return;

        if (!BuildMode(player))
            return;

        DeleteBlock(player);
    }

    public void Command_RotateBlock(CCSPlayerController? player, bool load, string rotateOption)
    {
        if (load)
        {
            foreach (var cmd in Config.BuildCommands.RotateBlock.Split(','))
            {
                AddCommand($"css_{cmd}", "Rotate block", (player, command) => Command_RotateBlock(player, false, command.ArgByIndex(1)));
            }

            return;
        }

        if (player == null)
            return;

        if (!BuildMode(player))
            return;

        RotateBlock(player, rotateOption);
    }

    public void Command_SaveBlocks(CCSPlayerController? player, bool load)
    {
        if (load)
        {
            foreach (var cmd in Config.BuildCommands.SaveBlocks.Split(','))
            {
                AddCommand($"css_{cmd}", "Save blocks", (player, command) => Command_SaveBlocks(player, false));
            }

            return;
        }

        if (player == null)
            return;

        if (!BuildMode(player))
            return;

        SaveBlocks();
    }

    public void Command_Snapping(CCSPlayerController? player, bool load)
    {
        if (load)
        {
            foreach (var cmd in Config.BuildCommands.Snapping.Split(','))
            {
                AddCommand($"css_{cmd}", "Toggle block snapping", (player, command) => Command_Snapping(player, false));
            }

            return;
        }

        if (player == null)
            return;

        if (!BuildMode(player))
            return;

        ToggleCommand(player, ref playerData[player].Snapping, "Block Snapping");
    }

    public void Command_Grid(CCSPlayerController? player, bool load)
    {
        if (load)
        {
            foreach (var cmd in Config.BuildCommands.Grid.Split(','))
                AddCommand($"css_{cmd}", "Toggle block grid", (player, command) => Command_Grid(player, false));

            return;
        }

        if (player == null)
            return;

        if (!BuildMode(player))
            return;

        ToggleCommand(player, ref playerData[player].Grid, "Block Grid");
    }
}