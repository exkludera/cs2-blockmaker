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
        public bool DisableShadows { get; set; } = true;

        public class Settings_Block
        {
            public float Duration { get; set; }
            public float Cooldown { get; set; }
            public float Value { get; set; }
        }

        public Settings_Block Bhop { get; set; } = new Settings_Block { Cooldown = 2.0f };
        public Settings_Block Health { get; set; } = new Settings_Block { Value = 2.0f };
        public Settings_Block Grenade { get; set; } = new Settings_Block { Cooldown = 60.0f };
        public Settings_Block Gravity { get; set; } = new Settings_Block { Duration = 4.0f, Value = 0.4f };
        public Settings_Block Frost { get; set; } = new Settings_Block { Cooldown = 60.0f };
        public Settings_Block Flash { get; set; } = new Settings_Block { Cooldown = 60.0f };
        public Settings_Block Fire { get; set; } = new Settings_Block { Duration = 5.0f, Value = 8.0f };
        public Settings_Block Delay { get; set; } = new Settings_Block { Duration = 1.0f, Cooldown = 2.0f };
        public Settings_Block Damage { get; set; } = new Settings_Block { Value = 5.0f };
        public Settings_Block Stealth { get; set; } = new Settings_Block { Duration = 10.0f, Cooldown = 60.0f };
        public Settings_Block Speed { get; set; } = new Settings_Block { Duration = 3.0f, Value = 1.5f, Cooldown = 60.0f };
        public Settings_Block Slap { get; set; } = new Settings_Block { Value = 2.0f };
        public Settings_Block Random { get; set; } = new Settings_Block { Cooldown = 60.0f };
        public Settings_Block Invincibility { get; set; } = new Settings_Block { Duration = 5.0f, Cooldown = 60.0f };
        //public Settings_Block SpeedBoost { get; set; } = new Settings_Block();
        //public Settings_Block Trampoline { get; set; } = new Settings_Block();
        //public Settings_Block Death { get; set; } = new Settings_Block();
        //public Settings_Block Deagle { get; set; } = new Settings_Block();
        //public Settings_Block AWP { get; set; } = new Settings_Block();
        //public Settings_Block NoFallDmg { get; set; } = new Settings_Block();
        //public Settings_Block Glass { get; set; } = new Settings_Block();
        //public Settings_Block TBarrier { get; set; } = new Settings_Block();
        //public Settings_Block CTBarrier { get; set; } = new Settings_Block();
        //public Settings_Block Nuke { get; set; } = new Settings_Block();
        //public Settings_Block NoSlowDown { get; set; } = new Settings_Block();
        //public Settings_Block Honey { get; set; } = new Settings_Block();
        //public Settings_Block Ice { get; set; } = new Settings_Block();

        public class Settings_BlockCamouflage : Settings_Block
        {
            public string ModelT { get; set; } = "characters/models/ctm_fbi/ctm_fbi.vmdl";
            public string ModelCT { get; set; } = "characters/models/tm_leet/tm_leet_variantb.vmdl";
        }
        public Settings_BlockCamouflage Camouflage { get; set; } = new Settings_BlockCamouflage { Duration = 10.0f, Cooldown = 60.0f};
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
        public string SelectBlockType { get; set; } = "block, blocktype";
        public string CreateBlock { get; set; } = "create,createblock,place,placeblock";
        public string DeleteBlock { get; set; } = "delete,deleteblock,remove,removeblock";
        public string RotateBlock { get; set; } = "rotate,rotateblock";
        public string SaveBlocks { get; set; } = "save,saveblocks,saveblock";
        public string Snapping { get; set; } = "snap,snapblock,blocksnap";
        public string Grid { get; set; } = "grid";
        public string Noclip { get; set; } = "nc,noclip";
        public string Godmode { get; set; } = "god,godmode";
        public string TestBlock { get; set; } = "testblock";
    }
    public Commands_Building Building { get; set; } = new Commands_Building();
}