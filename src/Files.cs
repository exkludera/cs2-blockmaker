using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace BlockMaker;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    private string blocksFolder = string.Empty;
    private string savedBlocksPath = string.Empty;
    private string modelsPath = string.Empty;

    public Dictionary<string, BlockSizes> BlockModels { get; set; } = new();

    public void Files()
    {
        // saved map blocks
        blocksFolder = Path.Combine(ModuleDirectory, "blocks");
        Directory.CreateDirectory(blocksFolder);

        // block models
        modelsPath = Path.Combine(ModuleDirectory, "models.json");

        if (!string.IsNullOrEmpty(modelsPath))
        {
            if (!File.Exists(modelsPath))
            {
                using (FileStream fs = File.Create(modelsPath))
                    fs.Close();

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                string jsonContent = JsonSerializer.Serialize(Blocks, options);

                File.WriteAllText(modelsPath, jsonContent);
            }
        }

        LoadBlocksModels();
    }

    public void LoadBlocksModels()
    {
        if (!string.IsNullOrEmpty(modelsPath) && File.Exists(modelsPath))
        {
            string jsonContent = File.ReadAllText(modelsPath);
            BlockModels = JsonSerializer.Deserialize<Dictionary<string, BlockSizes>>(jsonContent) ?? new Dictionary<string, BlockSizes>();
        }
    }

    public Dictionary<string, BlockSizes> Blocks { get; set; } = new()
    {
        { "Platform", new BlockSizes {
            Small = "models/blockbuilder/small_platform.vmdl",
            Medium = "models/blockbuilder/platform.vmdl",
            Large = "models/blockbuilder/large_platform.vmdl",
            Pole = "models/blockbuilder/pole_platform.vmdl",
        } },
        { "Bhop", new BlockSizes {
            Small = "models/blockbuilder/small_bhop.vmdl",
            Medium = "models/blockbuilder/bhop.vmdl",
            Large = "models/blockbuilder/large_bhop.vmdl",
            Pole = "models/blockbuilder/pole_bhop.vmdl",
        } },
        { "NoFallDmg", new BlockSizes {
            Small = "models/blockbuilder/small_nofalldmg.vmdl",
            Medium = "models/blockbuilder/nofalldmg.vmdl",
            Large = "models/blockbuilder/large_nofalldmg.vmdl",
            Pole = "models/blockbuilder/pole_nofalldmg.vmdl",
        } },
        { "Honey", new BlockSizes {
            Small = "models/blockbuilder/small_honey.vmdl",
            Medium = "models/blockbuilder/honey.vmdl",
            Large = "models/blockbuilder/large_honey.vmdl",
            Pole = "models/blockbuilder/pole_honey.vmdl",
        } },
        { "Health", new BlockSizes {
            Small = "models/blockbuilder/small_health.vmdl",
            Medium = "models/blockbuilder/health.vmdl",
            Large = "models/blockbuilder/large_health.vmdl",
            Pole = "models/blockbuilder/pole_health.vmdl",
        } },
        { "Grenade", new BlockSizes {
            Small = "models/blockbuilder/small_he.vmdl",
            Medium = "models/blockbuilder/he.vmdl",
            Large = "models/blockbuilder/large_he.vmdl",
            Pole = "models/blockbuilder/pole_he.vmdl",
        } },
        { "Gravity", new BlockSizes {
            Small = "models/blockbuilder/small_gravity.vmdl",
            Medium = "models/blockbuilder/gravity.vmdl",
            Large = "models/blockbuilder/large_gravity.vmdl",
            Pole = "models/blockbuilder/pole_gravity.vmdl",
        } },
        { "Glass", new BlockSizes {
            Small = "models/blockbuilder/small_glass.vmdl",
            Medium = "models/blockbuilder/glass.vmdl",
            Large = "models/blockbuilder/large_glass.vmdl",
            Pole = "models/blockbuilder/pole_glass.vmdl",
        } },
        { "Frost", new BlockSizes {
            Small = "models/blockbuilder/small_frost.vmdl",
            Medium = "models/blockbuilder/frost.vmdl",
            Large = "models/blockbuilder/large_frost.vmdl",
            Pole = "models/blockbuilder/pole_frost.vmdl",
        } },
        { "Flash", new BlockSizes {
            Small = "models/blockbuilder/small_flash.vmdl",
            Medium = "models/blockbuilder/flash.vmdl",
            Large = "models/blockbuilder/large_flash.vmdl",
            Pole = "models/blockbuilder/pole_flash.vmdl",
        } },
        { "Fire", new BlockSizes {
            Small = "models/blockbuilder/small_fire.vmdl",
            Medium = "models/blockbuilder/fire.vmdl",
            Large = "models/blockbuilder/large_fire.vmdl",
            Pole = "models/blockbuilder/pole_fire.vmdl",
        } },
        { "Delay", new BlockSizes {
            Small = "models/blockbuilder/small_delay.vmdl",
            Medium = "models/blockbuilder/delay.vmdl",
            Large = "models/blockbuilder/large_delay.vmdl",
            Pole = "models/blockbuilder/pole_delay.vmdl",
        } },
        { "Death", new BlockSizes {
            Small = "models/blockbuilder/small_death.vmdl",
            Medium = "models/blockbuilder/death.vmdl",
            Large = "models/blockbuilder/large_death.vmdl",
            Pole = "models/blockbuilder/pole_death.vmdl",
        } },
        { "Damage", new BlockSizes {
            Small = "models/blockbuilder/small_damage.vmdl",
            Medium = "models/blockbuilder/damage.vmdl",
            Large = "models/blockbuilder/large_damage.vmdl",
            Pole = "models/blockbuilder/pole_damage.vmdl",
        } },
        { "Deagle", new BlockSizes {
            Small = "models/blockbuilder/small_deagle.vmdl",
            Medium = "models/blockbuilder/deagle.vmdl",
            Large = "models/blockbuilder/large_deagle.vmdl",
            Pole = "models/blockbuilder/pole_deagle.vmdl",
        } },
        { "AWP", new BlockSizes {
            Small = "models/blockbuilder/small_awp.vmdl",
            Medium = "models/blockbuilder/awp.vmdl",
            Large = "models/blockbuilder/large_awp.vmdl",
            Pole = "models/blockbuilder/pole_awp.vmdl",
        } },
        { "Trampoline", new BlockSizes {
            Small = "models/blockbuilder/small_tramp.vmdl",
            Medium = "models/blockbuilder/tramp.vmdl",
            Large = "models/blockbuilder/large_tramp.vmdl",
            Pole = "models/blockbuilder/pole_tramp.vmdl",
        } },
        { "Stealth", new BlockSizes {
            Small = "models/blockbuilder/small_stealth.vmdl",
            Medium = "models/blockbuilder/stealth.vmdl",
            Large = "models/blockbuilder/large_stealth.vmdl",
            Pole = "models/blockbuilder/pole_stealth.vmdl",
        } },
        { "SpeedBoost", new BlockSizes {
            Small = "models/blockbuilder/small_speedboost.vmdl",
            Medium = "models/blockbuilder/speedboost.vmdl",
            Large = "models/blockbuilder/large_speedboost.vmdl",
            Pole = "models/blockbuilder/pole_speedboost.vmdl",
        } },
        { "Speed", new BlockSizes {
            Small = "models/blockbuilder/small_speed.vmdl",
            Medium = "models/blockbuilder/speed.vmdl",
            Large = "models/blockbuilder/large_speed.vmdl",
            Pole = "models/blockbuilder/pole_speed.vmdl",
        } },
        { "T-Barrier", new BlockSizes {
            Small = "models/blockbuilder/small_tbarrier.vmdl",
            Medium = "models/blockbuilder/tbarrier.vmdl",
            Large = "models/blockbuilder/large_tbarrier.vmdl",
            Pole = "models/blockbuilder/pole_tbarrier.vmdl",
        } },
        { "CT-Barrier", new BlockSizes {
            Small = "models/blockbuilder/small_ctbarrier.vmdl",
            Medium = "models/blockbuilder/ctbarrier.vmdl",
            Large = "models/blockbuilder/large_ctbarrier.vmdl",
            Pole = "models/blockbuilder/pole_ctbarrier.vmdl",
        } },
        { "Slap", new BlockSizes {
            Small = "models/blockbuilder/small_slap.vmdl",
            Medium = "models/blockbuilder/slap.vmdl",
            Large = "models/blockbuilder/large_slap.vmdl",
            Pole = "models/blockbuilder/pole_slap.vmdl",
        } },
        { "Random", new BlockSizes {
            Small = "models/blockbuilder/small_random.vmdl",
            Medium = "models/blockbuilder/random.vmdl",
            Large = "models/blockbuilder/large_random.vmdl",
            Pole = "models/blockbuilder/pole_random.vmdl",
        } },
        { "Nuke", new BlockSizes {
            Small = "models/blockbuilder/small_nuke.vmdl",
            Medium = "models/blockbuilder/nuke.vmdl",
            Large = "models/blockbuilder/large_nuke.vmdl",
            Pole = "models/blockbuilder/pole_nuke.vmdl",
        } },
        { "NoSlowDown", new BlockSizes {
            Small = "models/blockbuilder/small_noslowdown.vmdl",
            Medium = "models/blockbuilder/noslowdown.vmdl",
            Large = "models/blockbuilder/large_noslowdown.vmdl",
            Pole = "models/blockbuilder/pole_noslowdown.vmdl",
        } },
        { "Invincibility", new BlockSizes {
            Small = "models/blockbuilder/small_invincibility.vmdl",
            Medium = "models/blockbuilder/invincibility.vmdl",
            Large = "models/blockbuilder/large_invincibility.vmdl",
            Pole = "models/blockbuilder/pole_invincibility.vmdl",
        } },
        { "Ice", new BlockSizes {
            Small = "models/blockbuilder/small_ice.vmdl",
            Medium = "models/blockbuilder/ice.vmdl",
            Large = "models/blockbuilder/large_ice.vmdl",
            Pole = "models/blockbuilder/pole_ice.vmdl",
        } },
        { "Camouflage", new BlockSizes {
            Small = "models/blockbuilder/small_camouflage.vmdl",
            Medium = "models/blockbuilder/camouflage.vmdl",
            Large = "models/blockbuilder/large_camouflage.vmdl",
            Pole = "models/blockbuilder/pole_camouflage.vmdl",
        } },
    };
}