using CounterStrikeSharp.API.Core;

public class Settings
{
    public string Prefix { get; set; } = "{purple}[BlockMaker]{default}";
    public string MenuType { get; set; } = "html";
    public bool BuildMode { get; set; } = false;
    public bool BuildModeConfig { get; set; } = false;
    public string BlockGrabColor { get; set; } = "255,255,255,128";
    public float[] GridValues { get; set; } = { 0.0f, 8f, 16.0f, 32.0f, 64.0f, 128.0f, 256.0f };
    public bool AutoSave { get; set; } = false;
    public int SaveTime { get; set; } = 300;
}

public class Commands
{
    public bool Enabled { get; set; } = true;
    public string Permission { get; set; } = "@css/root";
    public string BuildMenu { get; set; } = "blockmenu,blocksmenu,buildmenu";
    public string BuildMode { get; set; } = "buildmode,togglebuild";
    public string CreateBlock { get; set; } = "create,block,createblock";
    public string DeleteBlock { get; set; } = "delete,deleteblock,removeblock";
    public string RotateBlock { get; set; } = "rotate,rotateblock";
    public string SaveBlocks { get; set; } = "save,saveblocks";
    public string ToggleBuilder { get; set; } = "togglebuilder,allowbuilder,allowbuild";
    public string ToggleSnapping { get; set; } = "snap,togglesnap";
}

public class Sounds
{
    public bool Enabled { get; set; } = true;
    public string Create { get; set; } = "sounds/buttons/blip1.vsnd";
    public string Delete { get; set; } = "sounds/buttons/blip2.vsnd";
    public string Place { get; set; } = "sounds/buttons/latchunlocked2.vsnd";
    public string Rotate { get; set; } = "sounds/buttons/button9.vsnd";
    public string Save { get; set; } = "sounds/buttons/bell1.vsnd";
}

public class Config : BasePluginConfig
{
    public Settings Settings { get; set; } = new Settings();
    public Commands Commands { get; set; } = new Commands();
    public Sounds Sounds { get; set; } = new Sounds();
}