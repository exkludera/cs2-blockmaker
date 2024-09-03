using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using System.Text.Json;

public partial class Plugin
{
    public Dictionary<CBaseProp, BlockData> UsedBlocks = new Dictionary<CBaseProp, BlockData>();

    public void CreateBlock(CCSPlayerController player)
    {
        var hitPoint = RayTrace.TraceShape(new Vector(player.PlayerPawn.Value!.AbsOrigin!.X, player.PlayerPawn.Value!.AbsOrigin!.Y, player.PlayerPawn.Value!.AbsOrigin!.Z + player.PlayerPawn.Value.CameraServices!.OldPlayerViewOffsetZ), player.PlayerPawn.Value!.EyeAngles!, false, true);

        if (hitPoint == null && !hitPoint.HasValue)
        {
            PrintToChat(player, $"Create Block: {ChatColors.Red}Distance too large between block and aim location");
            return;
        }

        string selectedBlock = playerData[player.Slot].BlockType;

        if (string.IsNullOrEmpty(selectedBlock))
        {
            PrintToChat(player, $"Create Block: Select a Block first");
            return;
        }

        string blockmodel = GetModelFromSelectedBlock(player, playerData[player.Slot].BlockSize);

        try
        {
            CreateBlockEntity(selectedBlock, blockmodel, playerData[player.Slot].BlockSize, RayTrace.Vector3toVector(hitPoint.Value), new QAngle());

            if (Config.Sounds.Building.Enabled)
                player.PlaySound(Config.Sounds.Building.Create);

            PrintToChat(player, $"Create Block: Created type: {ChatColors.White}{playerData[player.Slot].BlockType}{ChatColors.Grey}, size: {ChatColors.White}{playerData[player.Slot].BlockSize}");
        }
        catch
        {
            PrintToChat(player, $"Create Block: Failed to create block");
            return;
        }
    }

    public void CreateBlockEntity(string blockType, string blockModel, string blockSize, Vector blockPosition, QAngle blockRotation)
    {
        var block = Utilities.CreateEntityByName<CPhysicsPropOverride>("prop_physics_override")!;

        if (block != null && block.IsValid)
        {
            block.DispatchSpawn();
            block.SetModel(blockModel);

            block.Entity!.Name = blockType;
            block.EnableUseOutput = true;
            block.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags = (uint)(block.CBodyComponent!.SceneNode!.Owner!.Entity!.Flags & ~(1 << 2));

            block.AcceptInput("DisableMotion", block, block);
            block.Teleport(new Vector(blockPosition.X, blockPosition.Y, blockPosition.Z), new QAngle(blockRotation.X, blockRotation.Y, blockRotation.Z));

            block.ShadowStrength = Config.Settings.Blocks.DisableShadows ? 0.0f : 1.0f;

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
                CreateBlockEntity(blockData.Name, blockData.Model, blockData.Size, new Vector(blockData.Position.X, blockData.Position.Y, blockData.Position.Z), new QAngle(blockData.Rotation.Pitch, blockData.Rotation.Yaw, blockData.Rotation.Roll));
        }
        else
        {
            PrintToChatAll($"{ChatColors.Red}{noSpawnBlocksMessage()}");
            Logger.LogError(noSpawnBlocksMessage());
        }
    }

    private string noSpawnBlocksMessage()
    {
        return $"Failed to spawn Blocks. File for {GetMapName()} is empty or invalid";
    }
}
