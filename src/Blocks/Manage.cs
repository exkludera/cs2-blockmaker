using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

public partial class Plugin
{
    public void DeleteBlock(CCSPlayerController player)
    {
        var entity = player.GetBlockAimTarget();

        if (entity != null)
        {
            if (UsedBlocks.TryGetValue(entity, out var block))
            {
                block.Entity.Remove();
                UsedBlocks.Remove(block.Entity);

                if (Config.Sounds.Building.Enabled)
                    player.PlaySound(Config.Sounds.Building.Delete);

                PrintToChat(player, $"Delete Block: Deleted type: {ChatColors.White}{block.Name}{ChatColors.Grey}, size: {ChatColors.White}{block.Size}");
            }
        }
        else PrintToChat(player, "Delete Block: Could not find a block");
    }

    public void RotateBlock(CCSPlayerController player, string rotation)
    {
        var block = player.GetBlockAimTarget();

        float selectedRotation = playerData[player.Slot].RotationValue;

        if (block != null)
        {
            if (UsedBlocks.ContainsKey(block))
            {
                if (string.IsNullOrEmpty(rotation))
                {
                    PrintToChat(player, $"Rotate Block: Option cannot be empty");
                    return;
                }

                if (string.Equals(rotation, "reset", StringComparison.OrdinalIgnoreCase))
                    block.Teleport(block.AbsOrigin, new QAngle(0, 0, 0));

                else if (string.Equals(rotation, "x-", StringComparison.OrdinalIgnoreCase))
                    block.Teleport(block.AbsOrigin, new QAngle(block.AbsRotation!.X - selectedRotation, block.AbsRotation.Y, block.AbsRotation.Z));
                else if (string.Equals(rotation, "x+", StringComparison.OrdinalIgnoreCase))
                    block.Teleport(block.AbsOrigin, new QAngle(block.AbsRotation!.X + selectedRotation, block.AbsRotation.Y, block.AbsRotation.Z));

                else if (string.Equals(rotation, "y-", StringComparison.OrdinalIgnoreCase))
                    block.Teleport(block.AbsOrigin, new QAngle(block.AbsRotation!.X, block.AbsRotation.Y - selectedRotation, block.AbsRotation.Z));
                else if (string.Equals(rotation, "y+", StringComparison.OrdinalIgnoreCase))
                    block.Teleport(block.AbsOrigin, new QAngle(block.AbsRotation!.X, block.AbsRotation.Y + selectedRotation, block.AbsRotation.Z));

                else if (string.Equals(rotation, "z-", StringComparison.OrdinalIgnoreCase))
                    block.Teleport(block.AbsOrigin, new QAngle(block.AbsRotation!.X, block.AbsRotation.Y, block.AbsRotation.Z - selectedRotation));
                else if (string.Equals(rotation, "z+", StringComparison.OrdinalIgnoreCase))
                    block.Teleport(block.AbsOrigin, new QAngle(block.AbsRotation!.X, block.AbsRotation.Y, block.AbsRotation.Z + selectedRotation));

                else
                {
                    PrintToChat(player, $"Rotate Block: {ChatColors.White}'{rotation}' {ChatColors.Grey}is not a valid option");
                    return;
                }

                if (Config.Sounds.Building.Enabled)
                    player.PlaySound(Config.Sounds.Building.Rotate);

                PrintToChat(player, $"Rotate Block: {ChatColors.White}{rotation} {(string.Equals(rotation, "reset", StringComparison.OrdinalIgnoreCase) ? $"" : $"by {selectedRotation} Units")}");
            }
        }
        else PrintToChat(player, $"Rotate Block: Could not find a block");
    }
}
