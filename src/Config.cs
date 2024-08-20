using CounterStrikeSharp.API.Core;

public class Settings
{
    public string Prefix { get; set; } = "{purple}[BlockMaker]{default}";
    public string MenuType { get; set; } = "html";
    public bool BuildMode { get; set; } = false;
    public bool BuildModeConfig { get; set; } = false;
    public string BlockGrabColor { get; set; } = "255,255,255,128";
    public float[] GridValues { get; set; } = { 16f, 32f, 64f, 128f, 256f };
    public float[] RotationValues { get; set; } = { 15f, 30f, 45f, 60f, 90f, 120f};
    public bool AutoSave { get; set; } = false;
    public int SaveTime { get; set; } = 300;
}

public class AdminCommands
{
    public string Permission { get; set; } = "@css/root";
    public string BuildMode { get; set; } = "buildmode,togglebuild";
    public string ManageBuilder { get; set; } = "builder,togglebuilder,allowbuilder";
}

public class BuildCommands
{
    public string BuildMenu { get; set; } = "buildmenu,blockmenu,blocksmenu";
    public string CreateBlock { get; set; } = "create,block,createblock";
    public string DeleteBlock { get; set; } = "delete,deleteblock,remove,removeblock";
    public string RotateBlock { get; set; } = "rotate,rotateblock";
    public string SaveBlocks { get; set; } = "save,saveblocks,saveblock";
    public string Snapping { get; set; } = "snap,togglesnap,snapping";
    public string Grid { get; set; } = "grid,togglegrid";
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
    public AdminCommands AdminCommands { get; set; } = new AdminCommands();
    public BuildCommands BuildCommands { get; set; } = new BuildCommands();
    public Sounds Sounds { get; set; } = new Sounds();
}