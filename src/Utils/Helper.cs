using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using System.Drawing;
using System.Text.Json;

public partial class Plugin
{
    public bool BuildMode(CCSPlayerController player)
    {
        if (buildMode && playerData[player.Slot].Builder)
            return true;
        else
        {
            PrintToChat(player, $"{ChatColors.Red}You don't have access to Build Mode");
            return false;
        }
    }

    public bool HasPermission(CCSPlayerController player)
    {
        return string.IsNullOrEmpty(Config.Commands.Admin.Permission) || AdminManager.PlayerHasPermissions(player, Config.Commands.Admin.Permission);
    }

    public void PrintToChat(CCSPlayerController player, string message)
    {
        player.PrintToChat($"{Config.Settings.Main.Prefix} {ChatColors.Grey}{message}");
    }

    public void PrintToChatAll(string message)
    {
        Server.PrintToChatAll($"{Config.Settings.Main.Prefix} {ChatColors.Grey}{message}");
    }

    public void PlaySoundAll(string sound)
    {
        foreach (var player in Utilities.GetPlayers().Where(p => !p.IsBot))
            player.PlaySound(sound);
    }

    public bool IsValidJson(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Logger.LogInformation($"JSON Check: file does not exist ({filePath})");
            return false;
        }

        string fileContent = File.ReadAllText(filePath);

        if (string.IsNullOrWhiteSpace(fileContent))
        {
            Logger.LogError($"JSON Check: file is empty ({filePath})");
            return false;
        }

        try
        {
            JsonDocument.Parse(fileContent);
            return true;
        }
        catch (JsonException)
        {
            Logger.LogError($"JSON Check: invalid content ({filePath})");
            return false;
        }
    }

    public string GetModelFromSelectedBlock(CCSPlayerController player, string size)
    {
        var blockType = playerData[player.Slot].BlockType;

        foreach (var property in typeof(BlockModels).GetProperties())
        {
            var block = (BlockSizes)property.GetValue(BlockModels)!;

            if (block.Title.Equals(blockType, StringComparison.OrdinalIgnoreCase))
            {
                return size.ToLower() switch
                {
                    "small" => block.Small,
                    "medium" => block.Medium,
                    "large" => block.Large,
                    "pole" => block.Pole,
                    _ => block.Medium,
                };
            }
        }

        return string.Empty;
    }

    public int GetPlacedBlocksCount()
    {
        return Utilities.GetAllEntities().Where(e => e.DesignerName.Contains("prop_physics_override")).Count();
    }

    public string GetMapName()
    {
        return Server.MapName.ToString();
    }

    private Color ParseColor(string colorValue)
    {
        var colorParts = colorValue.Split(',');
        if (colorParts.Length == 4 &&
            int.TryParse(colorParts[0], out var r) &&
            int.TryParse(colorParts[1], out var g) &&
            int.TryParse(colorParts[2], out var b) &&
            int.TryParse(colorParts[3], out var a))
        {
            return Color.FromArgb(a, r, g, b);
        }
        return Color.FromArgb(255, 255, 255, 255);
    }
}