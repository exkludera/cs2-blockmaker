using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.Logging;
using static BlockBuilder.VectorUtils;
using System.Text.Json;

namespace BlockBuilder;

public partial class Plugin
{
    public void Command_SaveBlocks(CCSPlayerController player)
    {
        if (player == null || player.NotValid())
            return;

        if (!BuildMode(player))
            return;

        try
        {
            SaveBlocks();
        }
        catch
        {
            Logger.LogError("Failed to save blocks");
        }
        finally
        {
            PrintToChatAll("Saved Blocks");
            PlaySoundAll(Config.Sounds.Save);
        }
    }

    public void SaveBlocks()
    {
        if (!File.Exists(savedBlocksPath))
        {
            File.Create(savedBlocksPath);
            Logger.LogInformation($"JSON Check: file does not exist, creating one ({savedBlocksPath})");
        }

        var propDataList = new List<SavePropData>();

        if (propDataList == null)
        {
            PrintToChatAll("[AutoSave] No blocks to save");
            return;
        }

        foreach (var entry in UsedBlocks)
        {
            var prop = entry.Key;
            var data = entry.Value;

            if (prop != null && prop.IsValid)
            {
                propDataList.Add(new SavePropData
                {
                    Name = data.Name,
                    Model = data.Model,
                    Position = new VectorDTO(prop.AbsOrigin!),
                    Rotation = new QAngleDTO(prop.AbsRotation!)
                });
            }
        }

        var jsonString = JsonSerializer.Serialize(propDataList, new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(savedBlocksPath, jsonString);

        Logger.LogInformation("[AutoSave] Saved blocks");
    }
}
