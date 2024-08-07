using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BlockBuilder;

public class PlayerData
{
    public int currentTeam;
    public bool allowedBuilder;
    public string selectedBlock = "PLATFORM";
    public string selectedSize = "";
}

public partial class Plugin
{
    public void Reset()
    {
        UsedBlocks.Clear();
        PlayerHolds.Clear();
    }

    private void PrecacheResource(ResourceManifest manifest, string file)
    {
        if (!string.IsNullOrEmpty(file))
            manifest.AddResource(file);
    }

    public bool BuildMode(CCSPlayerController player)
    {
        if (buildMode && HasPermission(player))
            return true;
        else
        {
            PrintToChat(player, $"{ChatColors.Red}You don't have access to Build Mode");
            return false;
        }
    }

    public bool HasPermission(CCSPlayerController player)
    {
        return string.IsNullOrEmpty(Config.BuildMode.Permission) || AdminManager.PlayerHasPermissions(player, Config.BuildMode.Permission);
    }

    public void PrintToChat(CCSPlayerController player, string message)
    {
        player.PrintToChat($"{Config.Prefix} {ChatColors.Grey}{message}");
    }

    public void PrintToChatAll(string message)
    {
        Server.PrintToChatAll($"{Config.Prefix} {ChatColors.Grey}{message}");
    }

    public void PlaySound(CCSPlayerController player, string sound)
    {
        if (!Config.Sounds.Enabled || !player.IsValid || player.IsBot)
            return;

        player.ExecuteClientCommand($"play {sound}");
    }
    public void PlaySoundAll(string sound)
    {
        if (!Config.Sounds.Enabled)
            return;

        foreach (var player in Utilities.GetPlayers().Where(p => !p.IsBot))
            player.ExecuteClientCommand($"play {sound}");
    }

    public bool IsValidJson(string filePath)
    {
        if (!File.Exists(filePath))
        {
            File.Create(savedBlocksPath);
            Logger.LogInformation($"JSON Check: file does not exist, creating one ({filePath})");
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
        if (playerData.TryGetValue(player, out PlayerData data) && !string.IsNullOrEmpty(data.selectedBlock))
        {
            if (Config.Blocks.TryGetValue(data.selectedBlock, out BlockInfo blockInfo))
            {
                switch (size.ToLower())
                {
                    case "small":
                        return blockInfo.Small;
                    case "large":
                        return blockInfo.Large;
                    case "pole":
                        return blockInfo.Pole;
                    default:
                        return blockInfo.Default;
                }
            }
        }
        return string.Empty;
    }
}