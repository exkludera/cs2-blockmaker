using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

public class PlayerData
{
    public bool Builder = false;
    public string BlockType = "Platform";
    public string BlockSize = "Medium";
    public bool Grid = false;
    public float GridValue = 32f;
    public float RotationValue = 30f;
    public bool Snapping = false;
    public bool Noclip = false;
    public bool Godmode = false;
}

public class BlocksCooldown
{
    public bool Invincibility = false;
    public bool Deagle = false;
    public bool AWP = false;
    public bool Camouflage = false;
    public bool Grenade = false;
    public bool Frost = false;
    public bool Stealth = false;
    public bool Speed = false;
    public bool Flash = false;
    public bool Random = false;
    public bool Fire = false;
}

public class BuildingData
{
    public CBaseProp block = null!;
    public Vector offset = new();
    public int distance;
}