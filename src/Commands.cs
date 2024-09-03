using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    public void Commands()
    {
        // toggle buildmode
        foreach (var cmd in Config.Commands.Admin.BuildMode.Split(','))
            AddCommand($"css_{cmd}", "Toggle build mode", (player, command) => Command_BuildMode(player));

        // manage builders
        foreach (var cmd in Config.Commands.Admin.ManageBuilder.Split(','))
            AddCommand($"css_{cmd}", "Manage builder", Command_ManageBuilder);

        // build menu
        foreach (var cmd in Config.Commands.Building.BuildMenu.Split(','))
            AddCommand($"css_{cmd}", "Open build menu", (player, command) => Command_BuildMenu(player));

        // select block type
        foreach (var cmd in Config.Commands.Building.SelectBlockType.Split(','))
            AddCommand($"css_{cmd}", "Select Block Type", (player, command) => Command_SelectBlockType(player, command.ArgByIndex(1)));

        //create block
        foreach (var cmd in Config.Commands.Building.CreateBlock.Split(','))
            AddCommand($"css_{cmd}", "Create block", (player, command) => Command_CreateBlock(player));

        // delete block
        foreach (var cmd in Config.Commands.Building.DeleteBlock.Split(','))
            AddCommand($"css_{cmd}", "Delete block", (player, command) => Command_DeleteBlock(player));

        // rotate block
        foreach (var cmd in Config.Commands.Building.RotateBlock.Split(','))
            AddCommand($"css_{cmd}", "Rotate block", (player, command) => Command_RotateBlock(player, command.ArgByIndex(1)));

        // save blocks
        foreach (var cmd in Config.Commands.Building.SaveBlocks.Split(','))
            AddCommand($"css_{cmd}", "Save blocks", (player, command) => Command_SaveBlocks(player));

        // toggle snap
        foreach (var cmd in Config.Commands.Building.Snapping.Split(','))
            AddCommand($"css_{cmd}", "Toggle block snapping", (player, command) => Command_Snapping(player));

        // grid
        foreach (var cmd in Config.Commands.Building.Grid.Split(','))
            AddCommand($"css_{cmd}", "Toggle block grid", (player, command) => Command_Grid(player, command.ArgByIndex(1)));

        // noclip
        foreach (var cmd in Config.Commands.Building.Noclip.Split(','))
            AddCommand($"css_{cmd}", "Toggle noclip", (player, command) => Command_Noclip(player));

        // godmode
        foreach (var cmd in Config.Commands.Building.Godmode.Split(','))
            AddCommand($"css_{cmd}", "Toggle godmode", (player, command) => Command_Godmode(player));

        // godmode
        foreach (var cmd in Config.Commands.Building.TestBlock.Split(','))
            AddCommand($"css_{cmd}", "Test block", (player, command) => Command_Testblock(player));
    }

    public void ToggleCommand(CCSPlayerController player, ref bool commandStatus, string commandName)
    {
        commandStatus = !commandStatus;

        string status = commandStatus ? "ON" : "OFF";
        char color = commandStatus ? ChatColors.Green : ChatColors.Red;

        PrintToChat(player, $"{commandName}: {color}{status}");
    }

    public void Command_BuildMode(CCSPlayerController? player)
    {
        if (player == null || player.NotValid())
            return;

        if (!HasPermission(player))
        {
            PrintToChatAll($"{ChatColors.Red}You don't have permission to change Build Mode");
            return;
        }

        if (!buildMode)
        {
            buildMode = true;
            foreach (var target in Utilities.GetPlayers().Where(p => !p.IsBot))
            {
                playerData[target.Slot] = new PlayerData();
                PlayerHolds[target] = new BuildingData();
                if (HasPermission(target))
                    playerData[target.Slot].Builder = true;
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

    public void Command_ManageBuilder(CCSPlayerController? player, CommandInfo command)
    {
        if (player == null || player.NotValid())
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

        var builderStatus = playerData[targetPlayer.Slot].Builder;
        playerData[targetPlayer.Slot].Builder = !builderStatus;

        var action = builderStatus ? "removed" : "granted";
        var color = builderStatus ? ChatColors.Red : ChatColors.Green;

        PrintToChat(targetPlayer, $"{ChatColors.LightPurple}{player.PlayerName} {color}{action} your access to Build");
        PrintToChat(player, $"{color}You {action} {ChatColors.LightPurple}{targetPlayer.PlayerName} {color}access to Build");
    }

    public void Command_BuildMenu(CCSPlayerController? player)
    {
        if (player == null || player.NotValid())
            return;

        if (!BuildMode(player))
            return;

        Menu.Command_OpenMenus(player);
    }

    public void Command_SelectBlockType(CCSPlayerController? player, string selectType)
    {
        if (player == null || player.NotValid())
            return;

        if (!BuildMode(player))
            return;

        if (string.IsNullOrEmpty(selectType))
        {
            PrintToChat(player, $"Block Type: {ChatColors.Red}no block type specified");
            return;
        }

        foreach (var property in typeof(BlockModels).GetProperties())
        {
            var block = (BlockSizes)property.GetValue(BlockModels)!;

            string blockType = block.Title;

            if (string.Equals(blockType, selectType, StringComparison.OrdinalIgnoreCase))
            {
                playerData[player.Slot].BlockType = blockType;
                PrintToChat(player, $"Block Type: {ChatColors.Green}selected {blockType} block");
                return;
            }
        }

        PrintToChat(player, $"Block Type: {ChatColors.Red}could not find a matching block");
    }


    public void Command_CreateBlock(CCSPlayerController? player)
    {
        if (player == null || player.NotValid())
            return;

        if (!BuildMode(player))
            return;

        CreateBlock(player);
    }

    public void Command_DeleteBlock(CCSPlayerController? player)
    {
        if (player == null || player.NotValid())
            return;

        if (!BuildMode(player))
            return;

        DeleteBlock(player);
    }

    public void Command_RotateBlock(CCSPlayerController? player, string rotation)
    {
        if (player == null || player.NotValid())
            return;

        if (!BuildMode(player))
            return;

        RotateBlock(player, rotation);
    }

    public void Command_SaveBlocks(CCSPlayerController? player)
    {
        if (player == null || player.NotValid())
            return;

        if (!BuildMode(player))
            return;

        SaveBlocks();
    }

    public void Command_Snapping(CCSPlayerController? player)
    {
        if (player == null || player.NotValid())
            return;

        if (!BuildMode(player))
            return;

        ToggleCommand(player, ref playerData[player.Slot].Snapping, "Block Snapping");
    }

    public void Command_Grid(CCSPlayerController? player, string grid)
    {
        if (player == null || player.NotValid())
            return;

        if (!BuildMode(player))
            return;

        if (string.IsNullOrEmpty(grid))
        {
            ToggleCommand(player, ref playerData[player.Slot].Grid, "Block Grid");
            return;
        }

        playerData[player.Slot].GridValue = float.Parse(grid);
        PrintToChat(player, $"Selected Grid: {ChatColors.White}{grid} Units");
    }

    public void Command_Noclip(CCSPlayerController? player)
    {
        if (player == null || player.NotValid())
            return;

        if (!BuildMode(player))
            return;

        ToggleCommand(player, ref playerData[player.Slot].Noclip, "Noclip");

        if (playerData[player.Slot].Noclip)
        {
            player.Pawn.Value!.MoveType = MoveType_t.MOVETYPE_NOCLIP;
            Schema.SetSchemaValue(player.Pawn.Value!.Handle, "CBaseEntity", "m_nActualMoveType", 8); // noclip
            Utilities.SetStateChanged(player.Pawn.Value!, "CBaseEntity", "m_MoveType");
        }

        else if (!playerData[player.Slot].Noclip)
        {
            player.Pawn.Value!.MoveType = MoveType_t.MOVETYPE_WALK;
            Schema.SetSchemaValue(player!.Pawn.Value!.Handle, "CBaseEntity", "m_nActualMoveType", 2); // walk
            Utilities.SetStateChanged(player!.Pawn.Value!, "CBaseEntity", "m_MoveType");
        }
    }

    public void Command_Godmode(CCSPlayerController? player)
    {
        if (player == null || player.NotValid())
            return;

        if (!BuildMode(player))
            return;

        ToggleCommand(player, ref playerData[player.Slot].Godmode, "Godmode");

        if (playerData[player.Slot].Godmode)
            player.Pawn()!.TakesDamage = false;

        else if (!playerData[player.Slot].Godmode)
            player.Pawn()!.TakesDamage = true;
    }

    public void Command_Testblock(CCSPlayerController? player)
    {
        if (player == null || player.NotValid())
            return;

        if (!BuildMode(player))
            return;

        var block = player.GetBlockAimTarget();

        if (block == null)
        {
            PrintToChat(player, $"{ChatColors.Red}could not find a block to test");
            return;
        }

        if (block.Entity == null || string.IsNullOrEmpty(block.Entity.Name))
            return;

        if (!Blocks.blocksCooldown.ContainsKey(player.Slot))
            Blocks.blocksCooldown[player.Slot] = new BlocksCooldown();

        var cooldownProperty = Blocks.blocksCooldown[player.Slot].GetType().GetField(block.Entity.Name);
        if (cooldownProperty != null && cooldownProperty.FieldType == typeof(bool))
        {
            bool cooldown = (bool)cooldownProperty.GetValue(Blocks.blocksCooldown[player.Slot])!;

            if (cooldown)
            {
                PrintToChat(player, $"{ChatColors.Red}{block.Entity.Name} block is on cooldown");
                return;
            }
        }

        PrintToChat(player, $"{ChatColors.Green}testing {block.Entity!.Name} block");
        Blocks.Actions(player, block);
    }
}