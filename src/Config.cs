using CounterStrikeSharp.API.Core;

public class Config : BasePluginConfig
{
    public Settings Settings { get; set; } = new Settings();
    public Commands Commands { get; set; } = new Commands();
    public Sounds Sounds { get; set; } = new Sounds();
}

public class Settings
{
    public class Settings_Main
    {
        public string Prefix { get; set; } = "{purple}[BlockMaker]{default}";
        public string MenuType { get; set; } = "html";
    }
    public Settings_Main Main { get; set; } = new Settings_Main();

    public class Settings_Building
    {
        public bool BuildMode { get; set; } = false;
        public bool BuildModeConfig { get; set; } = false;
        public string BlockGrabColor { get; set; } = "255,255,255,128";
        public float[] GridValues { get; set; } = { 16f, 32f, 64f, 128f, 256f };
        public float[] RotationValues { get; set; } = { 15f, 30f, 45f, 60f, 90f, 120f };
        public bool AutoSave { get; set; } = false;
        public int SaveTime { get; set; } = 300;
    }
    public Settings_Building Building { get; set; } = new Settings_Building();

    public class Settings_Blocks
    {
        public class Settings_DurationTime
        {
            public float Gravity { get; set; } = 4.0f;
            public float Speed { get; set; } = 3.0f;
            public float Invincibility { get; set; } = 5.0f;
            public float Camouflage { get; set; } = 5.0f;
            public float Stealth { get; set; } = 5.0f;
            public float Delay { get; set; } = 2.0f;
        }
        public Settings_DurationTime Durations { get; set; } = new Settings_DurationTime();

        public class Settings_BlockValues
        {
            public float Gravity { get; set; } = 0.4f;
            public float Speed { get; set; } = 1.5f;
            public string CamouflageT { get; set; } = "characters/models/tm_leet/tm_leet_variantb.vmdl";
            public string CamouflageCT { get; set; } = "characters/models/ctm_fbi/ctm_fbi.vmdl";
            public int Slap { get; set; } = 2;
            public int Fire { get; set; } = 5;
            public int Damage { get; set; } = 5;
        }
        public Settings_BlockValues Values { get; set; } = new Settings_BlockValues();

        public class Settings_ResetTime
        {
            public float Bhop { get; set; } = 2.0f;
            public float FrostGrenade { get; set; } = 60.0f;
            public float HEGrenade { get; set; } = 60.0f;
            public float Flashbang { get; set; } = 60.0f;
            public float Stealth { get; set; } = 60.0f;
            public float Camouflage { get; set; } = 60.0f;
            public float Invincibility { get; set; } = 60.0f;
            public float Speed { get; set; } = 60.0f;
            public float Delay { get; set; } = 2.0f;
            public float Random { get; set; } = 60.0f;
        }
        public Settings_ResetTime Cooldowns { get; set; } = new Settings_ResetTime();
    }
    public Settings_Blocks Blocks { get; set; } = new Settings_Blocks();
}

public class Sounds
{
    public class Sounds_Blocks
    {
        public string Speed { get; set; } = "sounds/bootsofspeed.vsnd";
        public string Camouflage { get; set; } = "sounds/camouflage.vsnd";
        public string Damage { get; set; } = "sounds/dmg.vsnd";
        public string Health { get; set; } = "sounds/heartbeat.vsnd";
        public string Invincibility { get; set; } = "sounds/invincibility.vsnd";
        public string Nuke { get; set; } = "sounds/nuke.vsnd";
        public string Stealth { get; set; } = "sounds/stealth.vsnd";
        public string Teleport { get; set; } = "sounds/teleport.vsnd";
    }
    public Sounds_Blocks Blocks { get; set; } = new Sounds_Blocks();

    public class Sounds_Building
    {
        public bool Enabled { get; set; } = true;
        public string Create { get; set; } = "sounds/buttons/blip1.vsnd";
        public string Delete { get; set; } = "sounds/buttons/blip2.vsnd";
        public string Place { get; set; } = "sounds/buttons/latchunlocked2.vsnd";
        public string Rotate { get; set; } = "sounds/buttons/button9.vsnd";
        public string Save { get; set; } = "sounds/buttons/bell1.vsnd";
    }
    public Sounds_Building Building { get; set; } = new Sounds_Building();
}

public class Commands
{
    public class Commands_Admin
    {
        public string Permission { get; set; } = "@css/root";
        public string BuildMode { get; set; } = "buildmode,togglebuild";
        public string ManageBuilder { get; set; } = "builder,togglebuilder,allowbuilder";
    }
    public Commands_Admin Admin { get; set; } = new Commands_Admin();

    public class Commands_Building
    {
        public string BuildMenu { get; set; } = "buildmenu,blockmenu,blocksmenu";
        public string CreateBlock { get; set; } = "create,block,createblock";
        public string DeleteBlock { get; set; } = "delete,deleteblock,remove,removeblock";
        public string RotateBlock { get; set; } = "rotate,rotateblock";
        public string SaveBlocks { get; set; } = "save,saveblocks,saveblock";
        public string Snapping { get; set; } = "snap,togglesnap,snapping";
        public string Grid { get; set; } = "grid,togglegrid";
        public string Noclip { get; set; } = "nc,noclip";
        public string Godmode { get; set; } = "god,godmode";
    }
    public Commands_Building Building { get; set; } = new Commands_Building();
}