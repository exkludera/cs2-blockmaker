using CounterStrikeSharp.API.Core;

public class Menu
{
    public string Type { get; set; } = "html";
    public string Command { get; set; } = "css_blockmenu,css_blocksmenu,css_buildmenu";
}

public class BuildMode
{
    public bool Enabled { get; set; } = false;
    public string Permission { get; set; } = "@css/root";
    public string Command { get; set; } = "css_buildmode,css_togglebuild";
}

public class Sounds
{
    public bool Enabled { get; set; } = true;
    public string Rotate { get; set; } = "sounds/buttons/button9.vsnd";
    public string Create { get; set; } = "sounds/buttons/blip1.vsnd";
    public string Remove { get; set; } = "sounds/buttons/blip2.vsnd";
    public string Place { get; set; } = "sounds/buttons/latchunlocked2.vsnd";
    public string Save { get; set; } = "sounds/buttons/bell1.vsnd";
}

public class AutoSave
{
    public bool Enabled { get; set; } = false;
    public int Time { get; set; } = 120;
}

public class BlockInfo
{
    public string Default { get; set; } = string.Empty;
    public string Small { get; set; } = string.Empty;
    public string Large { get; set; } = string.Empty;
    public string Pole { get; set; } = string.Empty;
}

public class Config : BasePluginConfig
{
    public string Prefix { get; set; } = "{purple}[BlockBuilder]{default}";
    public Menu Menu { get; set; } = new Menu();
    public BuildMode BuildMode { get; set; } = new BuildMode();
    public Sounds Sounds { get; set; } = new Sounds();
    public AutoSave AutoSave { get; set; } = new AutoSave();
    public Dictionary<string, BlockInfo> Blocks { get; set; } = new()
    {
        { "PLATFORM", new BlockInfo {
            Default = "models/blockbuilder/platform.vmdl",
            Small = "models/blockbuilder/small_platform.vmdl",
            Large = "models/blockbuilder/large_platform.vmdl",
            Pole = "models/blockbuilder/pole_platform.vmdl",
        } },
        { "BHOP", new BlockInfo {
            Default = "models/blockbuilder/bhop.vmdl",
            Small = "models/blockbuilder/small_bhop.vmdl",
            Large = "models/blockbuilder/large_bhop.vmdl",
            Pole = "models/blockbuilder/pole_bhop.vmdl",
        } },
        { "NOFALLDMG", new BlockInfo {
            Default = "models/blockbuilder/nofalldmg.vmdl",
            Small = "models/blockbuilder/small_nofalldmg.vmdl",
            Large = "models/blockbuilder/large_nofalldmg.vmdl",
            Pole = "models/blockbuilder/pole_nofalldmg.vmdl",
        } },
        { "HONEY", new BlockInfo {
            Default = "models/blockbuilder/honey.vmdl",
            Small = "models/blockbuilder/small_honey.vmdl",
            Large = "models/blockbuilder/large_honey.vmdl",
            Pole = "models/blockbuilder/pole_honey.vmdl",
        } },
        { "HEALTH", new BlockInfo {
            Default = "models/blockbuilder/health.vmdl",
            Small = "models/blockbuilder/small_health.vmdl",
            Large = "models/blockbuilder/large_health.vmdl",
            Pole = "models/blockbuilder/pole_health.vmdl",
        } },
        { "GRENADE", new BlockInfo {
            Default = "models/blockbuilder/he.vmdl",
            Small = "models/blockbuilder/small_he.vmdl",
            Large = "models/blockbuilder/large_he.vmdl",
            Pole = "models/blockbuilder/pole_he.vmdl",
        } },
        { "GRAVITY", new BlockInfo {
            Default = "models/blockbuilder/gravity.vmdl",
            Small = "models/blockbuilder/small_gravity.vmdl",
            Large = "models/blockbuilder/large_gravity.vmdl",
            Pole = "models/blockbuilder/pole_gravity.vmdl",
        } },
        { "GLASS", new BlockInfo {
            Default = "models/blockbuilder/glass.vmdl",
            Small = "models/blockbuilder/small_glass.vmdl",
            Large = "models/blockbuilder/large_glass.vmdl",
            Pole = "models/blockbuilder/pole_glass.vmdl",
        } },
        { "FROST", new BlockInfo {
            Default = "models/blockbuilder/frost.vmdl",
            Small = "models/blockbuilder/small_frost.vmdl",
            Large = "models/blockbuilder/large_frost.vmdl",
            Pole = "models/blockbuilder/pole_frost.vmdl",
        } },
        { "FLASH", new BlockInfo {
            Default = "models/blockbuilder/flash.vmdl",
            Small = "models/blockbuilder/small_flash.vmdl",
            Large = "models/blockbuilder/large_flash.vmdl",
            Pole = "models/blockbuilder/pole_flash.vmdl",
        } },
        { "FIRE", new BlockInfo {
            Default = "models/blockbuilder/fire.vmdl",
            Small = "models/blockbuilder/small_fire.vmdl",
            Large = "models/blockbuilder/large_fire.vmdl",
            Pole = "models/blockbuilder/pole_fire.vmdl",
        } },
        { "DELAY", new BlockInfo {
            Default = "models/blockbuilder/delay.vmdl",
            Small = "models/blockbuilder/small_delay.vmdl",
            Large = "models/blockbuilder/large_delay.vmdl",
            Pole = "models/blockbuilder/pole_delay.vmdl",
        } },
        { "DEATH", new BlockInfo {
            Default = "models/blockbuilder/death.vmdl",
            Small = "models/blockbuilder/small_death.vmdl",
            Large = "models/blockbuilder/large_death.vmdl",
            Pole = "models/blockbuilder/pole_death.vmdl",
        } },
        { "DAMAGE", new BlockInfo {
            Default = "models/blockbuilder/damage.vmdl",
            Small = "models/blockbuilder/small_damage.vmdl",
            Large = "models/blockbuilder/large_damage.vmdl",
            Pole = "models/blockbuilder/pole_damage.vmdl",
        } },
        { "DEAGLE", new BlockInfo {
            Default = "models/blockbuilder/deagle.vmdl",
            Small = "models/blockbuilder/small_deagle.vmdl",
            Large = "models/blockbuilder/large_deagle.vmdl",
            Pole = "models/blockbuilder/pole_deagle.vmdl",
        } },
        { "AWP", new BlockInfo {
            Default = "models/blockbuilder/awp.vmdl",
            Small = "models/blockbuilder/small_awp.vmdl",
            Large = "models/blockbuilder/large_awp.vmdl",
            Pole = "models/blockbuilder/pole_awp.vmdl",
        } },
        { "TRAMPOLINE", new BlockInfo {
            Default = "models/blockbuilder/tramp.vmdl",
            Small = "models/blockbuilder/small_tramp.vmdl",
            Large = "models/blockbuilder/large_tramp.vmdl",
            Pole = "models/blockbuilder/pole_tramp.vmdl",
        } },
        { "STEALTH", new BlockInfo {
            Default = "models/blockbuilder/stealth.vmdl",
            Small = "models/blockbuilder/small_stealth.vmdl",
            Large = "models/blockbuilder/large_stealth.vmdl",
            Pole = "models/blockbuilder/pole_stealth.vmdl",
        } },
        { "SPEEDBOOST", new BlockInfo {
            Default = "models/blockbuilder/speedboost.vmdl",
            Small = "models/blockbuilder/small_speedboost.vmdl",
            Large = "models/blockbuilder/large_speedboost.vmdl",
            Pole = "models/blockbuilder/pole_speedboost.vmdl",
        } },
        { "SPEED", new BlockInfo {
            Default = "models/blockbuilder/speed.vmdl",
            Small = "models/blockbuilder/small_speed.vmdl",
            Large = "models/blockbuilder/large_speed.vmdl",
            Pole = "models/blockbuilder/pole_speed.vmdl",
        } },
        { "T-BARRIER", new BlockInfo {
            Default = "models/blockbuilder/tbarrier.vmdl",
            Small = "models/blockbuilder/small_tbarrier.vmdl",
            Large = "models/blockbuilder/large_tbarrier.vmdl",
            Pole = "models/blockbuilder/pole_tbarrier.vmdl",
        } },
        { "CT-BARRIER", new BlockInfo {
            Default = "models/blockbuilder/ctbarrier.vmdl",
            Small = "models/blockbuilder/small_ctbarrier.vmdl",
            Large = "models/blockbuilder/large_ctbarrier.vmdl",
            Pole = "models/blockbuilder/pole_ctbarrier.vmdl",
        } },
        { "SLAP", new BlockInfo {
            Default = "models/blockbuilder/slap.vmdl",
            Small = "models/blockbuilder/small_slap.vmdl",
            Large = "models/blockbuilder/large_slap.vmdl",
            Pole = "models/blockbuilder/pole_slap.vmdl",
        } },
        { "RANDOM", new BlockInfo {
            Default = "models/blockbuilder/random.vmdl",
            Small = "models/blockbuilder/small_random.vmdl",
            Large = "models/blockbuilder/large_random.vmdl",
            Pole = "models/blockbuilder/pole_random.vmdl",
        } },
        { "NUKE", new BlockInfo {
            Default = "models/blockbuilder/nuke.vmdl",
            Small = "models/blockbuilder/small_nuke.vmdl",
            Large = "models/blockbuilder/large_nuke.vmdl",
            Pole = "models/blockbuilder/pole_nuke.vmdl",
        } },
        { "NOSLOWDOWN", new BlockInfo {
            Default = "models/blockbuilder/noslowdown.vmdl",
            Small = "models/blockbuilder/small_noslowdown.vmdl",
            Large = "models/blockbuilder/large_noslowdown.vmdl",
            Pole = "models/blockbuilder/pole_noslowdown.vmdl",
        } },
        { "INVINCIBILITY", new BlockInfo {
            Default = "models/blockbuilder/invincibility.vmdl",
            Small = "models/blockbuilder/small_invincibility.vmdl",
            Large = "models/blockbuilder/large_invincibility.vmdl",
            Pole = "models/blockbuilder/pole_invincibility.vmdl",
        } },
        { "ICE", new BlockInfo {
            Default = "models/blockbuilder/ice.vmdl",
            Small = "models/blockbuilder/small_ice.vmdl",
            Large = "models/blockbuilder/large_ice.vmdl",
            Pole = "models/blockbuilder/pole_ice.vmdl",
        } },
        { "CAMOUFLAGE", new BlockInfo {
            Default = "models/blockbuilder/camouflage.vmdl",
            Small = "models/blockbuilder/small_camouflage.vmdl",
            Large = "models/blockbuilder/large_camouflage.vmdl",
            Pole = "models/blockbuilder/pole_camouflage.vmdl",
        } },
    };

}