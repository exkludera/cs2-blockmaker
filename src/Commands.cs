using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

namespace BlockMaker;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    public void Commands()
    {
        if (Config.Commands.Enabled)
        {
            //menu
            foreach (var command in Config.Commands.BuildMenu.Split(','))
                AddCommand($"css_{command}", "Open build menu", Menu.Command_OpenMenus!);

            //build mode
            foreach (var command in Config.Commands.BuildMode.Split(','))
                AddCommand($"css_{command}", "Toggle build mode", (CCSPlayerController? player, CommandInfo command) => Command_ToggleBuildMode(player!));

            //create
            foreach (var command in Config.Commands.CreateBlock.Split(','))
                AddCommand($"css_{command}", "Create block", (CCSPlayerController? player, CommandInfo command) => Command_CreateBlock(player!));

            //delete
            foreach (var command in Config.Commands.DeleteBlock.Split(','))
                AddCommand($"css_{command}", "Delete block", (CCSPlayerController? player, CommandInfo command) => Command_DeleteBlock(player!));

            //rotate
            foreach (var command in Config.Commands.RotateBlock.Split(','))
                AddCommand($"css_{command}", "Rotate block", (CCSPlayerController? player, CommandInfo command) => Command_RotateBlock(player!, command.GetArg(1)));

            //save
            foreach (var command in Config.Commands.SaveBlocks.Split(','))
                AddCommand($"css_{command}", "Save blocks", (CCSPlayerController? player, CommandInfo command) => Command_SaveBlocks(player!));

            //allow player to build
            foreach (var command in Config.Commands.ToggleBuilder.Split(','))
                AddCommand($"css_{command}", "Allow Builder", Command_ToggleBuilder!);

            //save
            foreach (var command in Config.Commands.ToggleSnapping.Split(','))
                AddCommand($"css_{command}", "Toggle Snapping", (CCSPlayerController? player, CommandInfo command) => Command_ToggleSnapping(player!));
        }
    }

    public void Command_ToggleBuilder(CCSPlayerController player, CommandInfo command)
    {
        if (!HasPermission(player))
        {
            PrintToChat(player, $"{ChatColors.Red}You don't have permission to change Builders");
            return;
        }

        if (BuildMode(player))
        {
            string input = command.ArgString;

            if (string.IsNullOrEmpty(input))
            {
                PrintToChat(player, $"{ChatColors.Red}No player specified");
                return;
            }

            bool foundPlayer = false;
            CCSPlayerController targetPlayer = null!;

            foreach (var target in Utilities.GetPlayers())
            {
                if (target.PlayerName.ToLower().Contains(input.ToLower()))
                {
                    targetPlayer = target;
                    foundPlayer = true;
                    break;
                }
                if (target.SteamID.ToString() == input)
                {
                    targetPlayer = target;
                    foundPlayer = true;
                    break;
                }
            }

            if (foundPlayer)
            {
                if (playerData[targetPlayer].Builder == true)
                {
                    playerData[targetPlayer].Builder = false;
                    PrintToChat(targetPlayer, $"{ChatColors.White}{player.PlayerName} {ChatColors.Red}Removed your access to Build");
                    PrintToChat(player, $"{ChatColors.Red}You removed {ChatColors.White}{targetPlayer.PlayerName} {ChatColors.Red}access to Build");
                    return;
                }

                playerData[targetPlayer].Builder = true;
                PrintToChat(targetPlayer, $"{ChatColors.White}{player.PlayerName} {ChatColors.Green}Gave you access to Build");
                PrintToChat(player, $"{ChatColors.Green}You gave {ChatColors.White}{targetPlayer.PlayerName} {ChatColors.Green}access to Build");
            }
            else
            {
                PrintToChat(player, $"{ChatColors.Red}Player not found");
            }
        }
    }

    public void Command_ToggleSnapping(CCSPlayerController player)
    {
        if (!BuildMode(player))
            return;

        if (!playerData[player].Snapping)
        {
            playerData[player].Snapping = true;
            PrintToChat(player, $"Block Snapping: {ChatColors.Green}Enabled");
        }
        else
        {
            playerData[player].Snapping = false;
            PrintToChat(player, $"Block Snapping: {ChatColors.Red}Disabled");
        }
    }
}