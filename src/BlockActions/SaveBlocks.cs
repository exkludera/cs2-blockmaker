using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using static BlockMaker.VectorUtils;

namespace BlockMaker;

public partial class Plugin
{
    public void Command_SaveBlocks(CCSPlayerController player)
    {
        if (player == null || player.NotValid())
            return;

        if (!BuildMode(player))
            return;

        SaveBlocks();
    }

    public void SaveBlocks()
    {
        if (!File.Exists(savedBlocksPath))
        {
            using (FileStream fs = File.Create(savedBlocksPath))
            {
                Logger.LogInformation($"File does not exist, creating one ({savedBlocksPath})");
                fs.Close();
            }
        }

        try
        {
            var blockDataList = new List<SaveBlockData>();

            foreach (var entry in UsedBlocks)
            {
                var prop = entry.Key;
                var data = entry.Value;

                if (prop != null && prop.IsValid)
                {
                    blockDataList.Add(new SaveBlockData
                    {
                        Name = data.Name,
                        Model = data.Model,
                        Size = data.Size,
                        Position = new VectorDTO(prop.AbsOrigin!),
                        Rotation = new QAngleDTO(prop.AbsRotation!)
                    });
                }
            }

            if (blockDataList.Count() == 0 || GetPlacedBlocksCount() == 0)
            {
                PrintToChatAll($"{ChatColors.Red}No blocks to save");
                return;
            }

            var jsonString = JsonSerializer.Serialize(blockDataList, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(savedBlocksPath, jsonString);

            PlaySoundAll(Config.Sounds.Save);
            PrintToChatAll($"Saved {ChatColors.White}{GetPlacedBlocksCount()} {ChatColors.Grey}Block{(GetPlacedBlocksCount() == 1 ? "" : "s")} on {ChatColors.White}{GetMapName()}");
            Logger.LogInformation($"Saved {GetPlacedBlocksCount()} Block{(GetPlacedBlocksCount() == 1 ? "" : "s")} on {GetMapName()}");
        }
        catch
        {
            Logger.LogError("Failed to save blocks :(");
            return;
        }
    }
}
