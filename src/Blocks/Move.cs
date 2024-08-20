using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using System.Data;
using System.Drawing;
using static BlockMaker.VectorUtils;

namespace BlockMaker;

public partial class Plugin
{
    public Dictionary<CCSPlayerController, Builder> PlayerHolds = new Dictionary<CCSPlayerController, Builder>();

    public void OnTick()
    {
        if (!buildMode)
            return;

        foreach (var player in Utilities.GetPlayers().Where(p => p != null && p.PawnIsAlive && !p.IsBot))
        {
            if (player.Buttons.HasFlag(PlayerButtons.Reload))
            {
                if (PlayerHolds.ContainsKey(player))
                    RotateRepeat(player, PlayerHolds[player].block!);

                else GrabBlock(player);
            }
            else if (player.Buttons.HasFlag(PlayerButtons.Use))
            {
                if (PlayerHolds.ContainsKey(player))
                    DistanceRepeat(player, PlayerHolds[player].block!);

                else GrabBlock(player);
            }
            else
            {
                if (PlayerHolds.ContainsKey(player))
                {
                    PlayerHolds[player].block.Render = Color.White;
                    Utilities.SetStateChanged(PlayerHolds[player].block, "CBaseModelEntity", "m_clrRender");

                    PlayerHolds.Remove(player);
                    PlaySound(player, Config.Sounds.Place);
                }
            }
        }
    }

    public void GrabBlock(CCSPlayerController player)
    {
        var block = player.GetBlockAimTarget();
        if (block != null)
        {
            if (!UsedBlocks.ContainsKey(block))
            {
                PrintToChat(player, $"{ChatColors.Red}Block not found in UsedBlocks");
                return;
            }
            GrabBlockAdd(player, block);
        }
    }

    public void GrabBlockAdd(CCSPlayerController player, CBaseProp block)
    {
        var hitPoint = RayTrace.TraceShape(new Vector(player.PlayerPawn.Value!.AbsOrigin!.X, player.PlayerPawn.Value!.AbsOrigin!.Y, player.PlayerPawn.Value!.AbsOrigin!.Z + player.PlayerPawn.Value.CameraServices!.OldPlayerViewOffsetZ), player.PlayerPawn.Value!.EyeAngles!, false, true);

        if (block != null && block.IsValid && hitPoint != null && hitPoint.HasValue)
        {
            if (CalculateDistance(block.AbsOrigin!, RayTrace.Vector3toVector(hitPoint.Value)) > 150)
            {
                PrintToChat(player, $"{ChatColors.Red}Distance too large between block and aim location");
                return;
            }

            int distance = (int)CalculateDistance(block.AbsOrigin!, player.PlayerPawn.Value!.AbsOrigin!);

            block.Render = ParseColor(Config.Settings.BlockGrabColor);
            Utilities.SetStateChanged(block, "CBaseModelEntity", "m_clrRender");

            PlayerHolds.Add(player, new Builder() { block = block, distance = distance });
        }
    }

    public void DistanceRepeat(CCSPlayerController player, CBaseProp block)
    {
        var (position, rotation) = GetEndXYZ(player, block, PlayerHolds[player].distance, playerData[player].Grid, playerData[player].GridValue, playerData[player].Snapping);
        
        block.Teleport(position, rotation);

        if (player.Buttons.HasFlag(PlayerButtons.Attack))
        {
            if (PlayerHolds[player].distance > 350) PlayerHolds[player].distance += 7;
            PlayerHolds[player].distance += 3;
        }
        else if (player.Buttons.HasFlag(PlayerButtons.Attack2) && PlayerHolds[player].distance > 3)
        {
            if (PlayerHolds[player].distance > 350) PlayerHolds[player].distance -= 7;
            PlayerHolds[player].distance -= 3;
        }
    }

    public void RotateRepeat(CCSPlayerController player, CBaseProp block)
    {
        var (position, rotation) = GetEndXYZ(player, block, PlayerHolds[player].distance, playerData[player].Grid, playerData[player].GridValue, playerData[player].Snapping);

        block.Teleport(position, rotation);

        if (player.Buttons.HasFlag(PlayerButtons.Attack))
        {
            PlayerHolds[player].block.Teleport(null, new QAngle(PlayerHolds[player].block.AbsRotation!.X, PlayerHolds[player].block.AbsRotation!.Y + 3, PlayerHolds[player].block.AbsRotation!.Z));
        }
        else if (player.Buttons.HasFlag(PlayerButtons.Attack2))
        {
            PlayerHolds[player].block.Teleport(null, new QAngle(PlayerHolds[player].block.AbsRotation!.X, PlayerHolds[player].block.AbsRotation!.Y, PlayerHolds[player].block.AbsRotation!.Z + 3));
        }
    }
}