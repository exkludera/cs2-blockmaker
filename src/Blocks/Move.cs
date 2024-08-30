using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using System.Data;
using System.Drawing;
using static BlockMaker.VectorUtils;

namespace BlockMaker;

public partial class Plugin
{
    public Dictionary<CCSPlayerController, BuildingData> PlayerHolds = new Dictionary<CCSPlayerController, BuildingData>();

    public void OnTick()
    {
        if (!buildMode)
            return;

        foreach (var player in Utilities.GetPlayers().Where(p => p.IsLegal() && p.IsAlive() && playerData.ContainsKey(p.Slot) && playerData[p.Slot].Builder))
        {
            if (!PlayerHolds.ContainsKey(player))
            {
                if (player.Buttons.HasFlag(PlayerButtons.Reload) || player.Buttons.HasFlag(PlayerButtons.Use))
                    GrabBlock(player);
            }
            else
            {
                var blockData = PlayerHolds[player];

                if (blockData.block == null || !blockData.block.IsValid)
                {
                    PlayerHolds.Remove(player);
                    continue;
                }

                if (player.Buttons.HasFlag(PlayerButtons.Reload))
                    RotateRepeat(player, blockData.block);

                else if (player.Buttons.HasFlag(PlayerButtons.Use))
                    DistanceRepeat(player, blockData.block);

                else
                {
                    blockData.block.Render = Color.White;
                    Utilities.SetStateChanged(blockData.block, "CBaseModelEntity", "m_clrRender");

                    PlayerHolds.Remove(player);

                    if (Config.Sounds.Building.Enabled)
                        player.PlaySound(Config.Sounds.Building.Place);
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

            block.Render = ParseColor(Config.Settings.Building.BlockGrabColor);
            Utilities.SetStateChanged(block, "CBaseModelEntity", "m_clrRender");

            PlayerHolds.Add(player, new BuildingData() { block = block, distance = distance });
        }
    }

    public void DistanceRepeat(CCSPlayerController player, CBaseProp block)
    {
        var (position, rotation) = GetEndXYZ(player, block, PlayerHolds[player].distance, playerData[player.Slot].Grid, playerData[player.Slot].GridValue, playerData[player.Slot].Snapping);
        
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
        var (position, rotation) = GetEndXYZ(player, block, PlayerHolds[player].distance, playerData[player.Slot].Grid, playerData[player.Slot].GridValue, playerData[player.Slot].Snapping);

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