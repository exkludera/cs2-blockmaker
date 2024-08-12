using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using System.Data;
using System.Drawing;
using static BlockMaker.VectorUtils;

namespace BlockMaker;

public partial class Plugin
{
    public Dictionary<CBaseProp, BlockData> UsedBlocks = new Dictionary<CBaseProp, BlockData>();
    public Dictionary<CCSPlayerController, Builder> PlayerHolds = new Dictionary<CCSPlayerController, Builder>();

    public void Command_ToggleBuildMode(CCSPlayerController player)
    {
        if (!HasPermission(player))
        {
            PrintToChatAll($"{ChatColors.Red}You don't have permission to change Build Mode");
            return;
        }

        if (!buildMode)
        {
            foreach (var target in Utilities.GetPlayers().Where(p => !p.IsBot && !playerData.ContainsKey(p) && !PlayerHolds.ContainsKey(p)))
            {
                playerData[target] = new();
                PlayerHolds[target] = new();
                if (HasPermission(target))
                    playerData[target].Builder = true;
            }

            buildMode = true;
            PrintToChatAll($"Build Mode: {ChatColors.Green}Enabled");
        }
        else
        {
            playerData.Clear();
            PlayerHolds.Clear();

            buildMode = false;
            PrintToChatAll($"Build Mode: {ChatColors.Red}Disabled");
        }
    }

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

                else FirstPressCheck(player);
            }
            else if (player.Buttons.HasFlag(PlayerButtons.Use))
            {
                if (PlayerHolds.ContainsKey(player))
                    PressRepeat(player, PlayerHolds[player].block!);

                else FirstPressCheck(player);
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

    public void FirstPressCheck(CCSPlayerController player)
    {
        var block = player.GetBlockAimTarget();
        if (block != null)
        {
            if (!UsedBlocks.ContainsKey(block))
            {
                PrintToChat(player, $"{ChatColors.Red}Block not found in UsedBlocks");
                return;
            }
            FirstPress(player, block);
        }
    }

    public void FirstPress(CCSPlayerController player, CBaseProp block)
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

    public void PressRepeat(CCSPlayerController player, CBaseProp block)
    {
        var (position, rotation) = GetEndXYZ(player, block, PlayerHolds[player].distance, playerData[player].Grid, playerData[player].Snapping);
        
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
        var (position, rotation) = GetEndXYZ(player, block, PlayerHolds[player].distance, playerData[player].Grid, playerData[player].Snapping);

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