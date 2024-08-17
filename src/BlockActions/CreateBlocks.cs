using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BlockMaker;

public partial class Plugin
{
    public void Command_CreateBlock(CCSPlayerController player)
    {
        if (player == null || player.NotValid())
            return;

        if (!BuildMode(player))
            return;

        var hitPoint = RayTrace.TraceShape(new Vector(player.PlayerPawn.Value!.AbsOrigin!.X, player.PlayerPawn.Value!.AbsOrigin!.Y, player.PlayerPawn.Value!.AbsOrigin!.Z + player.PlayerPawn.Value.CameraServices!.OldPlayerViewOffsetZ), player.PlayerPawn.Value!.EyeAngles!, false, true);

        if (hitPoint == null && !hitPoint.HasValue)
        {
            PrintToChat(player, $"Create Block: {ChatColors.Red}Distance too large between block and aim location");
            return;
        }

        string selectedBlock = playerData[player].Block;

        if (string.IsNullOrEmpty(selectedBlock))
        {
            PrintToChat(player, $"Create Block: Select a Block first");
            return;
        }

        string blockmodel = GetModelFromSelectedBlock(player, playerData[player].Size);

        try
        {
            CreateBlock(selectedBlock, blockmodel, playerData[player].Size, RayTrace.Vector3toVector(hitPoint.Value), new QAngle());

            PrintToChat(player, $"Create Block: Created type: {ChatColors.White}{playerData[player].Block}{ChatColors.Grey}, size: {ChatColors.White}{playerData[player].Size}");
            PlaySound(player, Config.Sounds.Create);
        }
        catch
        {
            PrintToChat(player, $"Create Block: Failed to create block");
            return;
        }
    }

    public void CreateBlock(string blockType, string blockModel, string blockSize, Vector blockPosition, QAngle blockRotation)
    {
        var block = Utilities.CreateEntityByName<CPhysicsPropOverride>("prop_physics_override");
        if (block != null && block.IsValid)
        {
            block.DispatchSpawn();
            block.SetModel(blockModel);

            block.Entity!.Name = blockType;
            block.EnableUseOutput = true;
            block.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags = (uint)(block.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags & ~(1 << 2));

            block.AcceptInput("DisableMotion", block, block);
            block.Teleport(new Vector(blockPosition.X, blockPosition.Y, blockPosition.Z), new QAngle(blockRotation.X, blockRotation.Y, blockRotation.Z));

            UsedBlocks[block] = new BlockData(block, blockType, blockModel, blockSize);
        }
        else Logger.LogError("(CreateBlock) failed to create block");
    }

    public void SpawnBlocks()
    {
        bool isValidJson = IsValidJson(savedBlocksPath);

        if (isValidJson)
        {
            var jsonString = File.ReadAllText(savedBlocksPath);

            var blockDataList = JsonSerializer.Deserialize<List<SaveBlockData>>(jsonString);

            if (jsonString == null || blockDataList == null || jsonString.ToString() == "[]")
            {
                PrintToChatAll($"{ChatColors.Red}{noSpawnBlocksMessage()}");
                return;
            }

            foreach (var blockData in blockDataList)
                CreateBlock(blockData.Name, blockData.Model, blockData.Size, new Vector(blockData.Position.X, blockData.Position.Y, blockData.Position.Z), new QAngle(blockData.Rotation.Pitch, blockData.Rotation.Yaw, blockData.Rotation.Roll));
        }
        else
        {
            PrintToChatAll($"{ChatColors.Red}{noSpawnBlocksMessage()}");
            Logger.LogError(noSpawnBlocksMessage());
        }
    }
}
