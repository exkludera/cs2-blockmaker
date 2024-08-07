using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BlockBuilder;

public partial class Plugin
{
    public void Command_CreateBlock(CCSPlayerController player)
    {
        if (player == null || player.NotValid())
            return;

        if (!BuildMode(player))
            return;

        var hitPoint = TraceShape(new Vector(player.PlayerPawn.Value!.AbsOrigin!.X, player.PlayerPawn.Value!.AbsOrigin!.Y, player.PlayerPawn.Value!.AbsOrigin!.Z + player.PlayerPawn.Value.CameraServices!.OldPlayerViewOffsetZ), player.PlayerPawn.Value!.EyeAngles!, false, true);

        if (hitPoint == null && !hitPoint.HasValue)
        {
            PrintToChat(player, $"{ChatColors.Red}HitPoint Distance too large");
            return;
        }

        if (_.playerData.TryGetValue(player, out PlayerData playerData))
        {
            string selectedBlock = playerData.selectedBlock;

            if (string.IsNullOrEmpty(selectedBlock))
            {
                PrintToChat(player, $"{ChatColors.Red}select a block first");
                return;
            }

            string blockmodel = GetModelFromSelectedBlock(player, playerData.selectedSize);

            try
            {
                CreateBlock(selectedBlock, blockmodel, Vector3toVector(hitPoint.Value), new QAngle());
            }
            catch
            {
                PrintToChat(player, $"{ChatColors.Red}Failed to create block");
            }
            finally
            {
                PlaySound(player, Config.Sounds.Create);
                PrintToChat(player, "Created block");
            }
        }
        else PrintToChat(player, $"{ChatColors.Red}you're not found in player data :(");
    }

    public void CreateBlock(string blockType, string blockModel, Vector blockPosition, QAngle blockRotation)
    {
        var block = Utilities.CreateEntityByName<CPhysicsPropOverride>("prop_physics_override");

        if (block != null && block.IsValid)
        {
            block.SetModel(blockModel);

            block.DispatchSpawn();

            block.EnableUseOutput = true;
            block.Globalname = blockType;

            block.MoveType = MoveType_t.MOVETYPE_NONE;
            block.AcceptInput("DisableMotion", block, block);

            block.Teleport(new Vector(blockPosition.X, blockPosition.Y, blockPosition.Z), new QAngle(blockRotation.X, blockRotation.Y, blockRotation.Z));

            UsedBlocks[block] = new PropData(block, blockType, blockModel);
        }
        else Logger.LogError("(CreateBlock) failed to create block");
    }

    public void SpawnBlocks()
    {
        bool isValidJson = IsValidJson(savedBlocksPath);

        if (isValidJson)
        {
            var jsonString = File.ReadAllText(savedBlocksPath);

            var propDataList = JsonSerializer.Deserialize<List<SavePropData>>(jsonString);

            if (jsonString == null || propDataList == null)
            {
                PrintToChatAll($"{ChatColors.Red}No blocks found in the saved file");
                return;
            }

            foreach (var propData in propDataList)
                CreateBlock(propData.Name, propData.Model, new Vector(propData.Position.X, propData.Position.Y, propData.Position.Z), new QAngle(propData.Rotation.Pitch, propData.Rotation.Yaw, propData.Rotation.Roll));
        }
        else Logger.LogError("The file does not contain valid JSON data");
    }
}
