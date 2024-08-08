using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using System.Data;
using static BlockBuilder.VectorUtils;

namespace BlockBuilder;

public partial class Plugin
{
    public Dictionary<CBaseProp, PropData> UsedBlocks = new Dictionary<CBaseProp, PropData>();
    public Dictionary<CCSPlayerController, Builder> PlayerHolds = new Dictionary<CCSPlayerController, Builder>();

    public void Command_ToggleBuildMode(CCSPlayerController player, CommandInfo command)
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
            }

            buildMode = true;
            PrintToChatAll($"Build Mode: {ChatColors.Green}Enabled");
        }
        else
        {
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
                    RotateRepeat(player, PlayerHolds[player].mainProp!);

                else FirstPressCheck(player);
            }
            else if (player.Buttons.HasFlag(PlayerButtons.Use))
            {
                if (PlayerHolds.ContainsKey(player))
                    PressRepeat(player, PlayerHolds[player].mainProp!);

                else FirstPressCheck(player);
            }
            else
            {
                if (PlayerHolds.ContainsKey(player))
                {
                    PlayerHolds[player].mainProp.MoveType = MoveType_t.MOVETYPE_NONE;
                    PlayerHolds[player].mainProp.AcceptInput("DisableMotion", PlayerHolds[player].mainProp, PlayerHolds[player].mainProp);

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


    public void FirstPress(CCSPlayerController player, CBaseProp prop)
    {
        var hitPoint = TraceShape(new Vector(player.PlayerPawn.Value!.AbsOrigin!.X, player.PlayerPawn.Value!.AbsOrigin!.Y, player.PlayerPawn.Value!.AbsOrigin!.Z + player.PlayerPawn.Value.CameraServices!.OldPlayerViewOffsetZ), player.PlayerPawn.Value!.EyeAngles!, false, true);

        if (prop != null && prop.IsValid && hitPoint != null && hitPoint.HasValue)
        {
            if (CalculateDistance(prop.AbsOrigin!, Vector3toVector(hitPoint.Value)) > 150)
            {
                PrintToChat(player, $"{ChatColors.Red}Distance too large between prop and hitPoint");
                return;
            }

            int distance = (int)CalculateDistance(prop.AbsOrigin!, player.PlayerPawn.Value!.AbsOrigin!);

            prop.MoveType = MoveType_t.MOVETYPE_VPHYSICS;
            prop.AcceptInput("EnableMotion", prop, prop);

            PlayerHolds.Add(player, new Builder() { mainProp = prop, distance = distance });
        }
    }

    public void PressRepeat(CCSPlayerController player, CBaseProp block)
    {
        block.Teleport(GetEndXYZ(player, PlayerHolds[player].distance, playerData[player].selectedGrid), null, player.PlayerPawn.Value!.AbsVelocity!);

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
        block.Teleport(GetEndXYZ(player, PlayerHolds[player].distance, playerData[player].selectedGrid), null, player.PlayerPawn.Value!.AbsVelocity!);

        if (player.Buttons.HasFlag(PlayerButtons.Attack))
        {
            PlayerHolds[player].mainProp.Teleport(null, new QAngle(PlayerHolds[player].mainProp.AbsRotation!.X, PlayerHolds[player].mainProp.AbsRotation!.Y + 5, PlayerHolds[player].mainProp.AbsRotation!.Z));
        }
        else if (player.Buttons.HasFlag(PlayerButtons.Attack2))
        {
            PlayerHolds[player].mainProp.Teleport(null, new QAngle(PlayerHolds[player].mainProp.AbsRotation!.X, PlayerHolds[player].mainProp.AbsRotation!.Y, PlayerHolds[player].mainProp.AbsRotation!.Z + 5));
        }
    }
}

public class Builder
{
    public CBaseProp mainProp = null!;
    public Vector offset = null!;
    public int distance;
}

public class PropData
{
    public PropData(CBaseProp prop, string blockName, string modelPath)
    {
        Entity = prop;
        Name = blockName;
        Model = modelPath;
    }

    public CBaseProp Entity;
    public string Name { get; private set; }
    public string Model { get; private set; }
}
public class SavePropData
{
    public string Name { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public VectorDTO Position { get; set; } = new VectorDTO(Vector.Zero);
    public QAngleDTO Rotation { get; set; } = new QAngleDTO(QAngle.Zero);
}