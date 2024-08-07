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

    public void Command_RotateBlock(CCSPlayerController player, bool vertical)
    {
        if (player == null || player.NotValid())
            return;

        if (!BuildMode(player))
            return;

        var block = player.GetBlockAimTarget();
        string axisType;

        if (block != null)
        {
            if (UsedBlocks.ContainsKey(block))
            {
                if (vertical)
                {
                    block.Teleport(block.AbsOrigin, new QAngle(block.AbsRotation!.X + 30, block.AbsRotation.Y, block.AbsRotation.Z));
                    axisType = "Vertical";
                }

                else
                {
                    block.Teleport(block.AbsOrigin, new QAngle(block.AbsRotation!.X, block.AbsRotation.Y + 30, block.AbsRotation.Z));
                    axisType = "Horizontal";
                }

                PrintToChat(player, $"Rotated Block: {ChatColors.White}{axisType} Axis");
                PlaySound(player, Config.Sounds.Rotate);
            }
        }
        else PrintToChat(player, "Could not find block to rotate");
    }
}
