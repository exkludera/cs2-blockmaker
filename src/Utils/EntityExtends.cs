using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.UserMessages;
using CounterStrikeSharp.API.Modules.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace BlockMaker;

public static class EntityExtends
{
    public const int TEAM_SPEC = 1;
    public const int TEAM_T = 2;
    public const int TEAM_CT = 3;

    public static readonly Color DEFAULT_COLOUR = Color.FromArgb(255, 255, 255, 255);

    public static void SetHp(this CCSPlayerController controller, int health = 100)
    {
        if (health <= 0 || !controller.PawnIsAlive || controller.PlayerPawn.Value == null || !controller.IsValid) return;

        controller.Health = health;
        controller.PlayerPawn.Value.Health = health;

        if (health > 100)
        {
            controller.MaxHealth = health;
            controller.PlayerPawn.Value.MaxHealth = health;
        }

        Server.NextFrame(() => Utilities.SetStateChanged(controller.PlayerPawn.Value, "CBaseEntity", "m_iHealth"));
    }

    public static bool NotValid(this CCSPlayerController? player)
    {
        return (player == null || !player.IsValid || !player.PlayerPawn.IsValid || player.Connected != PlayerConnectedState.PlayerConnected || player.IsBot || player.IsHLTV);
    }

    public static void RespawnClient(this CCSPlayerController client)
    {
        if (!client.IsValid || client.PawnIsAlive)
            return;

        var clientPawn = client.PlayerPawn.Value;

        MemoryFunctionVoid<CCSPlayerController, CCSPlayerPawn, bool, bool> CBasePlayerController_SetPawnFunc = new(GameData.GetSignature("CBasePlayerController_SetPawn"));
        CBasePlayerController_SetPawnFunc.Invoke(client, clientPawn!, true, false);
        VirtualFunction.CreateVoid<CCSPlayerController>(client.Handle, GameData.GetOffset("CCSPlayerController_Respawn"))(client);
    }

    internal static bool IsPlayer(this CCSPlayerController? player)
    {
        return player is { IsValid: true, IsHLTV: false, IsBot: false, UserId: not null, SteamID: > 0 };
    }

    public static void SetMoveType(this CCSPlayerController? player, MoveType_t type)
    {
        CCSPlayerPawn? pawn = player.Pawn();

        if (pawn != null)
            pawn.MoveType = type;
    }
    public static void Freeze(this CCSPlayerController? player)
    {
        player.SetMoveType(MoveType_t.MOVETYPE_NONE);
    }
    public static void UnFreeze(this CCSPlayerController? player)
    {
        player.SetMoveType(MoveType_t.MOVETYPE_WALK);
    }

    public static void SetGravity(this CCSPlayerController? player, float value)
    {
        CCSPlayerPawn? pawn = player.Pawn();

        if (pawn != null)
            pawn.GravityScale = value;
    }

    public static void SetVelocity(this CCSPlayerController? player, float value)
    {
        CCSPlayerPawn? pawn = player.Pawn();

        if (pawn != null)
            pawn.VelocityModifier = value;
    }

    public static void SetArmour(this CCSPlayerController? player, int hp)
    {
        CCSPlayerPawn? pawn = player.Pawn();

        if (pawn != null)
        {
            pawn.ArmorValue = hp;
        }
    }

    public static void SetModel(this CCSPlayerController? player, string model)
    {
        CCSPlayerPawn? pawn = player.Pawn();

        if (pawn != null)
        {
            pawn.SetModel(model);
        }
    }

    public static void GiveWeapon(this CCSPlayerController? player, String name)
    {
        if (player.IsAlive())
            player.GiveNamedItem("weapon_" + name);
    }

    public static void StripWeapons(this CCSPlayerController? player, bool removeKnife = false)
    {
        if (!player.IsAlive())
            return;

        player.RemoveWeapons();

        if (!removeKnife)
            player.GiveWeapon("knife");
    }

    public static void SetColour(this CCSPlayerController? player, Color colour)
    {
        CCSPlayerPawn? pawn = player.Pawn();

        if (pawn != null && player.IsAlive())
        {
            pawn.RenderMode = RenderMode_t.kRenderTransColor;
            pawn.Render = colour;
            Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");
        }
    }

    private static readonly MemoryFunctionWithReturn<nint, string, int, int> SetBodygroupFunc = new("55 48 89 E5 41 56 49 89 F6 41 55 41 89 D5 41 54 49 89 FC 48 83 EC 08");

    private static readonly Func<nint, string, int, int> SetBodygroup = SetBodygroupFunc.Invoke;

    public static void SetInvis(this CCSPlayerController? player, bool status, int strength = 255)
    {
        CCSPlayerPawn? pawn = player.Pawn();

        if (pawn != null && player.IsAlive())
        {
            var respawnpos = new Vector(pawn.AbsOrigin!.X, pawn.AbsOrigin.Y, pawn.AbsOrigin.Z);
            var respawnrotation = new QAngle(pawn.AbsRotation!.X, pawn.AbsRotation.Y, pawn.AbsRotation.Z);

            player.Respawn();

            Server.NextFrame(() =>
            {
                player.Teleport(respawnpos, respawnrotation);

                pawn!.Render = status ? Color.FromArgb(0, 0, 0, 0) : Color.FromArgb(strength, strength, strength, strength);
                Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");

                var gloves = pawn.EconGloves;
                SetBodygroup(pawn.Handle, "default_gloves", status ? 0 : 1);

                var activeWeapon = pawn!.WeaponServices?.ActiveWeapon.Value;
                if (activeWeapon != null && activeWeapon.IsValid)
                {
                    activeWeapon.Render = status ? Color.FromArgb(0, 0, 0, 0) : Color.FromArgb(strength, strength, strength, strength);
                    activeWeapon.ShadowStrength = status ? 0.0f : 1.0f;
                    Utilities.SetStateChanged(activeWeapon, "CBaseModelEntity", "m_clrRender");
                }

                var myWeapons = pawn!.WeaponServices?.MyWeapons;
                if (myWeapons != null)
                {
                    foreach (var gun in myWeapons)
                    {
                        var weapon = gun.Value;
                        if (weapon != null)
                        {
                            weapon.Render = status ? Color.FromArgb(0, 0, 0, 0) : Color.FromArgb(strength, strength, strength, strength);
                            weapon.ShadowStrength = status ? 0.0f : 1.0f;
                            Utilities.SetStateChanged(weapon, "CBaseModelEntity", "m_clrRender");

                            if (weapon.DesignerName == "weapon_c4")
                            {
                                weapon.AnimationUpdateScheduled = false;
                                Utilities.SetStateChanged(weapon, "CBaseAnimGraph", "m_bAnimationUpdateScheduled");
                            }
                        }
                    }
                }
            });
        }
    }

    public static bool IsVip(this CCSPlayerController? player)
    {
        if (!player.IsLegal())
            return false;

        return AdminManager.PlayerHasPermissions(player, new string[] { "@css/reservation" });
    }

    public static bool IsAdmin(this CCSPlayerController? player)
    {
        if (!player.IsLegal())
            return false;

        return AdminManager.PlayerHasPermissions(player, new string[] { "@css/generic" });
    }

    public static void PlaySound(this CCSPlayerController? player, string sound)
    {
        if (!player.IsLegal())
            return;

        player.ExecuteClientCommand($"play {sound}");
    }

    public static CCSPlayerPawn? Pawn(this CCSPlayerController? player)
    {
        if (!player.IsLegal() && !player.IsAlive())
            return null;

        CCSPlayerPawn? pawn = player.PlayerPawn.Value;

        return pawn;
    }

    public static bool IsLegal([NotNullWhen(true)] this CCSPlayerController? player)
    {
        return player != null && player.IsValid && player.PlayerPawn.IsValid && player.PlayerPawn.Value?.IsValid == true && !player.IsBot;
    }

    public static bool IsT([NotNullWhen(true)] this CCSPlayerController? player)
    {
        return IsLegal(player) && player.TeamNum == TEAM_T;
    }

    public static bool IsCT([NotNullWhen(true)] this CCSPlayerController? player)
    {
        return IsLegal(player) && player.TeamNum == TEAM_CT;
    }

    public static bool IsAlive([NotNullWhen(true)] this CCSPlayerController? player)
    {
        return player!.PawnIsAlive && player.PlayerPawn.Value?.LifeState == (byte)LifeState_t.LIFE_ALIVE;
    }

    static public CBasePlayerWeapon? FindWeapon(this CCSPlayerController? player, String name)
    {
        if (!player.IsAlive())
            return null;

        CCSPlayerPawn? pawn = player.Pawn();

        if (pawn == null)
            return null;

        var weapons = pawn.WeaponServices?.MyWeapons;

        if (weapons == null)
            return null;

        foreach (var weaponOpt in weapons)
        {
            CBasePlayerWeapon? weapon = weaponOpt.Value;

            if (weapon == null)
                continue;

            if (weapon.DesignerName.Contains(name))
                return weapon;
        }

        return null;
    }

    static public bool IsLegal([NotNullWhen(true)] this CBasePlayerWeapon? weapon)
    {
        return weapon != null && weapon.IsValid;
    }

    static public void SetAmmo(this CBasePlayerWeapon? weapon, int clip, int reserve)
    {
        if (!weapon.IsLegal())
            return;

        // overide reserve max so it doesn't get clipped when
        // setting "infinite ammo"
        // thanks 1Mack
        CCSWeaponBaseVData? weaponData = weapon.As<CCSWeaponBase>().VData;


        if (weaponData != null)
        {
            // TODO: this overide it for every gun the player has...
            // when not a map gun, this is not a big deal
            // for the reserve ammo it is for the clip though
            /*
                if(clip > weaponData.MaxClip1)
                {
                    weaponData.MaxClip1 = clip;
                }
            */
            if (reserve > weaponData.PrimaryReserveAmmoMax)
                weaponData.PrimaryReserveAmmoMax = reserve;
        }

        if (clip != -1)
        {
            weapon.Clip1 = clip;
            Utilities.SetStateChanged(weapon, "CBasePlayerWeapon", "m_iClip1");
        }

        if (reserve != -1)
        {
            weapon.ReserveAmmo[0] = reserve;
            Utilities.SetStateChanged(weapon, "CBasePlayerWeapon", "m_pReserveAmmo");
        }
    }

    public enum FadeFlags
    {
        FADE_IN,
        FADE_OUT,
        FADE_STAYOUT
    }
    public static void ColorScreen(this CCSPlayerController player, Color color, float hold = 0.1f, float fade = 0.2f, FadeFlags flags = FadeFlags.FADE_IN, bool withPurge = true)
    {
        var fadeMsg = UserMessage.FromPartialName("Fade");

        fadeMsg.SetInt("duration", Convert.ToInt32(fade * 512));
        fadeMsg.SetInt("hold_time", Convert.ToInt32(hold * 512));

        var flag = flags switch
        {
            FadeFlags.FADE_OUT => 0x0001,
            FadeFlags.FADE_IN => 0x0002,
            FadeFlags.FADE_STAYOUT => 0x0008,
            _ => (0x0001 | 0x0010),
        };

        if (withPurge)
            flag |= 0x0010;

        fadeMsg.SetInt("flags", flag);
        fadeMsg.SetInt("color", color.R | color.G << 8 | color.B << 16 | color.A << 24);
        fadeMsg.Send(player);
    }

    public static void Slap(this CCSPlayerController player, int damage)
    {
        if (!player.IsLegal() && !player.IsAlive())
            return;

        CCSPlayerPawn pawn = player.Pawn()!;

        /* Teleport in a random direction - thank you, Mani!*/
        /* Thank you AM & al!*/

        // ^^ & thanks to daffyyyy/CS2-SimpleAdmin

        var random = new Random();
        var vel = new Vector(pawn.AbsVelocity.X, pawn.AbsVelocity.Y, pawn.AbsVelocity.Z);

        vel.X += ((random.Next(180) + 50) * ((random.Next(2) == 1) ? -1 : 1));
        vel.Y += ((random.Next(180) + 50) * ((random.Next(2) == 1) ? -1 : 1));
        vel.Z += random.Next(200) + 100;

        pawn.AbsVelocity.X = vel.X;
        pawn.AbsVelocity.Y = vel.Y;
        pawn.AbsVelocity.Z = vel.Z;

        if (damage <= 0)
            return;

        pawn.Health -= damage;
        Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iHealth");

        if (pawn.Health <= 0)
            pawn.CommitSuicide(true, true);
    }

    public static void Health(this CCSPlayerController player, int health)
    {
        if (!player.IsLegal() && !player.IsAlive())
            return;

        CCSPlayerPawn pawn = player.Pawn()!;

        player.ColorScreen(Color.FromArgb(150, 255, 0, 0), 0.25f, 0.5f, FadeFlags.FADE_OUT);

        if (pawn.Health == pawn.MaxHealth)
            return;

        int newHealth = Math.Min(pawn.Health + health, pawn.MaxHealth);

        pawn.Health += newHealth;
        Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iHealth");

        if (pawn.Health <= 0)
            pawn.CommitSuicide(true, true);
    }
}