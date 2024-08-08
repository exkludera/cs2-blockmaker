using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace BlockBuilder;

public partial class Plugin
{
    public void Command_DeleteBlock(CCSPlayerController player)
    {
        if (player == null || player.NotValid())
            return;

        if (!BuildMode(player))
            return;

        var block = player.GetBlockAimTarget();

        if (block != null)
        {
            if (UsedBlocks.ContainsKey(block))
            {
                block.Remove();
                UsedBlocks.Remove(block);

                PrintToChat(player, "Block deleted");
                PlaySound(player, Config.Sounds.Remove);
            }
        }
        else PrintToChat(player, "Could not find block to delete");
    }

    public void Command_RotateBlock(CCSPlayerController player, string rotation)
    {
        if (player == null || player.NotValid())
            return;

        if (!BuildMode(player))
            return;

        var block = player.GetBlockAimTarget();

        float selectedRotation = playerData[player].selectedRotation;

        if (block != null)
        {
            if (UsedBlocks.ContainsKey(block))
            {
                if (rotation == "reset")
                    block.Teleport(block.AbsOrigin, new QAngle(0, 0, 0));

                if (rotation == "X+")
                    block.Teleport(block.AbsOrigin, new QAngle(block.AbsRotation!.X + selectedRotation, block.AbsRotation.Y, block.AbsRotation.Z));
                if (rotation == "X-")
                    block.Teleport(block.AbsOrigin, new QAngle(block.AbsRotation!.X - selectedRotation, block.AbsRotation.Y, block.AbsRotation.Z));

                if (rotation == "Y+")
                    block.Teleport(block.AbsOrigin, new QAngle(block.AbsRotation!.X, block.AbsRotation.Y + selectedRotation, block.AbsRotation.Z));
                if (rotation == "Y-")
                    block.Teleport(block.AbsOrigin, new QAngle(block.AbsRotation!.X, block.AbsRotation.Y - selectedRotation, block.AbsRotation.Z));

                if (rotation == "Z+")
                    block.Teleport(block.AbsOrigin, new QAngle(block.AbsRotation!.X, block.AbsRotation.Y, block.AbsRotation.Z + selectedRotation));
                if (rotation == "Z-")
                    block.Teleport(block.AbsOrigin, new QAngle(block.AbsRotation!.X, block.AbsRotation.Y, block.AbsRotation.Z - selectedRotation));

                PrintToChat(player, $"Rotated Block: {ChatColors.White}{rotation} {(rotation == "reset" ? "" : $"by {selectedRotation} units")}");
                PlaySound(player, Config.Sounds.Rotate);
            }
        }
        else PrintToChat(player, "Could not find block to rotate");
    }
}
