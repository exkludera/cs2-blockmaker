using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using System.Drawing;

public class Blocks
{
    private static Dictionary<string, Action<CCSPlayerController, CBaseProp>> blockActions = null!;
    private static Settings.Settings_Blocks settings = Plugin.Instance.Config.Settings.Blocks;
    private static Sounds.Sounds_Blocks sounds = Plugin.Instance.Config.Sounds.Blocks;
    public static void Load()
    {
        var block = Plugin.BlockModels;
        blockActions = new Dictionary<string, Action<CCSPlayerController, CBaseProp>>
        {
            { block.Random.Title, Action_Random },
            { block.Bhop.Title, Action_Bhop },
            { block.NoFallDmg.Title, Action_NoFallDmg },
            { block.Gravity.Title, Action_Gravity },
            { block.Health.Title, Action_Health },
            { block.Grenade.Title, Action_Grenade },
            { block.Frost.Title, Action_Frost },
            { block.Flash.Title, Action_Flash },
            { block.Fire.Title, Action_Fire },
            { block.Delay.Title, Action_Delay },
            { block.Death.Title, Action_Death },
            { block.Damage.Title, Action_Damage },
            { block.Deagle.Title, Action_Deagle },
            { block.AWP.Title, Action_AWP },
            { block.Trampoline.Title, Action_Trampoline },
            { block.SpeedBoost.Title, Action_SpeedBoost },
            { block.Speed.Title, Action_Speed },
            { block.Slap.Title, Action_Slap },
            { block.Nuke.Title, Action_Nuke },
            { block.Stealth.Title, Action_Stealth },
            { block.Invincibility.Title, Action_Invincibility },
            { block.Camouflage.Title, Action_Camouflage },
            { block.Platform.Title, Action_Platform },
            { block.Honey.Title, Action_Honey },
            { block.Glass.Title, Action_Glass },
            { block.TBarrier.Title, Action_TBarrier },
            { block.CTBarrier.Title, Action_CTBarrier },
            { block.Ice.Title, Action_Ice },
            { block.NoSlowDown.Title, Action_NoSlowDown }
        };
    }

    public static Dictionary<int, BlocksCooldown> blocksCooldown = new Dictionary<int, BlocksCooldown>();
    public static void BlockCooldownTimer(CCSPlayerController player, string block, float timer)
    {
        if (timer <= 0)
            return;

        Plugin.Instance.AddTimer(timer, () =>
        {
            var cooldownProperty = blocksCooldown[player.Slot].GetType().GetField(block);

            if (cooldownProperty != null && cooldownProperty.FieldType == typeof(bool))
            {
                bool cooldown = (bool)cooldownProperty.GetValue(blocksCooldown[player.Slot])!;

                if (cooldown)
                {
                    cooldownProperty.SetValue(blocksCooldown[player.Slot], false);
                    Plugin.Instance.PrintToChat(player, $"{ChatColors.White}{block} {ChatColors.Grey}block is no longer on cooldown");
                }
            }

            else Plugin.Instance.PrintToChat(player, $"{ChatColors.Red}Error: could not reset cooldown for {block} block");
        });
    }

    public static void Actions(CCSPlayerController player, CBaseProp block)
    {
        if (block == null || player.NotValid())
            return;

        if (block.Entity == null)
            return;

        if (!blocksCooldown.ContainsKey(player.Slot))
            blocksCooldown[player.Slot] = new BlocksCooldown();

        string blockName = block.Entity.Name;

        if (string.IsNullOrEmpty(blockName))
            return;

        if (blockActions.TryGetValue(blockName, out var action))
            action(player, block);

        else Plugin.Instance.PrintToChat(player, $"{ChatColors.Red}Error: No action found for {blockName} block");
    }

    private static void Action_Random(CCSPlayerController player, CBaseProp block)
    {
        if (blocksCooldown[player.Slot].Random)
            return;

        var availableActions = blockActions.Where(kvp => kvp.Key != Plugin.BlockModels.Random.Title).ToList();

        var randomAction = availableActions[new Random().Next(availableActions.Count)];

        randomAction.Value(player, block);

        Plugin.Instance.PrintToChat(player, $"You got {ChatColors.White}{randomAction.Key} {ChatColors.Grey}from the {ChatColors.White}{block.Entity!.Name} {ChatColors.Grey}block");

        blocksCooldown[player.Slot].Random = true;

        BlockCooldownTimer(player, "Random", settings.Random.Cooldown);
    }

    private static void Action_Bhop(CCSPlayerController player, CBaseProp block)
    {
        var movetype = block.MoveType;
        var solidflags = block.Collision.SolidFlags;
        var solidftype = block.Collision.SolidType;
        var collisiongroup = block.Collision.CollisionGroup;

        var render = block.Render;

        Plugin.Instance.AddTimer(0.1f, () =>
        {
            block.MoveType = MoveType_t.MOVETYPE_VPHYSICS;
            block.Collision.SolidFlags = 0x0008;
            block.Collision.SolidType = SolidType_t.SOLID_VPHYSICS;
            block.Collision.CollisionGroup = (byte)CollisionGroup.COLLISION_GROUP_NEVER;

            block.Render = Color.FromArgb(125, 125, 125, 125);
            Utilities.SetStateChanged(block, "CBaseModelEntity", "m_clrRender");
        });

        Plugin.Instance.AddTimer(settings.Bhop.Cooldown, () =>
        {
            block.MoveType = movetype;
            block.Collision.SolidFlags = solidflags;
            block.Collision.SolidType = solidftype;
            block.Collision.CollisionGroup = collisiongroup;

            block.Render = render;
            Utilities.SetStateChanged(block, "CBaseModelEntity", "m_clrRender");
        });
    }

    private static void Action_NoFallDmg(CCSPlayerController player, CBaseProp block)
    {
        player.PlayerPawn.Value!.TakesDamage = false;
        Plugin.Instance.AddTimer(0.01f, () => { player.PlayerPawn.Value.TakesDamage = true; });
    }

    private static void Action_Gravity(CCSPlayerController player, CBaseProp block)
    {
        var gravity = player.GravityScale;

        player.SetGravity(settings.Gravity.Value);

        Plugin.Instance.AddTimer(settings.Gravity.Duration, () =>
        {
            player.SetGravity(gravity);
            Plugin.Instance.PrintToChat(player, $"{block.Entity!.Name} has worn off");
        });
    }

    private static void Action_Health(CCSPlayerController player, CBaseProp block)
    {
        if (player.Pawn()!.Health >= player.Pawn()!.MaxHealth)
            return;

        player.Health(+2);
        player.PlaySound(sounds.Health);
    }

    private static void Action_Grenade(CCSPlayerController player, CBaseProp block)
    {
        if (blocksCooldown[player.Slot].Grenade)
            return;

        player.GiveWeapon("weapon_hegrenade");

        blocksCooldown[player.Slot].Grenade = true;
        BlockCooldownTimer(player, "Grenade", settings.Grenade.Cooldown);
    }

    private static void Action_Frost(CCSPlayerController player, CBaseProp block)
    {
        if (blocksCooldown[player.Slot].Frost)
            return;

        player.GiveWeapon("weapon_smokegrenade");

        blocksCooldown[player.Slot].Frost = true;
        BlockCooldownTimer(player, "Smoke", settings.Frost.Cooldown);
    }

    private static void Action_Flash(CCSPlayerController player, CBaseProp block)
    {
        if (blocksCooldown[player.Slot].Flash)
            return;

        player.GiveWeapon("weapon_flashbang");

        blocksCooldown[player.Slot].Flash = true;
        BlockCooldownTimer(player, "Flash", settings.Flash.Cooldown);
    }

    private static void Action_Fire(CCSPlayerController player, CBaseProp block)
    {
        if (blocksCooldown[player.Slot].Fire)
            return;

        var fire = Utilities.CreateEntityByName<CParticleSystem>("info_particle_system")!;

        fire.EffectName = "particles/burning_fx/env_fire_medium.vpcf";

        fire.DispatchSpawn();
        fire.AcceptInput("Start");
        fire.AcceptInput("FollowEntity", player.Pawn(), player.Pawn(), "!activator");

        player.Health((int)- settings.Fire.Value);

        var firetimer = Plugin.Instance.AddTimer(1.0f, () =>
        {
            player.Health((int)- settings.Fire.Value);
        }, TimerFlags.REPEAT);

        Plugin.Instance.AddTimer(settings.Fire.Duration, () =>
        {
            firetimer.Kill();
            fire.AcceptInput("Stop");
            fire.Remove();
        });

        blocksCooldown[player.Slot].Fire = true;
    }

    private static void Action_Delay(CCSPlayerController player, CBaseProp block)
    {
        var movetype = block.MoveType;
        var solidflags = block.Collision.SolidFlags;
        var solidftype = block.Collision.SolidType;
        var collisiongroup = block.Collision.CollisionGroup;

        var render = block.Render;

        Plugin.Instance.AddTimer(settings.Delay.Duration, () =>
        {
            block.MoveType = MoveType_t.MOVETYPE_VPHYSICS;
            block.Collision.SolidFlags = 0x0008;
            block.Collision.SolidType = SolidType_t.SOLID_VPHYSICS;
            block.Collision.CollisionGroup = (byte)CollisionGroup.COLLISION_GROUP_NEVER;

            block.Render = Color.FromArgb(125, 125, 125, 125);
            Utilities.SetStateChanged(block, "CBaseModelEntity", "m_clrRender");
        });

        Plugin.Instance.AddTimer(settings.Delay.Cooldown, () =>
        {
            block.MoveType = movetype;
            block.Collision.SolidFlags = solidflags;
            block.Collision.SolidType = solidftype;
            block.Collision.CollisionGroup = collisiongroup;

            block.Render = render;
            Utilities.SetStateChanged(block, "CBaseModelEntity", "m_clrRender");
        });
    }

    private static void Action_Death(CCSPlayerController player, CBaseProp block)
    {
        if (player.Pawn()!.TakesDamage == false)
        {
            Plugin.Instance.PrintToChat(player, "looks like you avoided death");
            return;
        }

        player.CommitSuicide(false, true);
    }

    private static void Action_Damage(CCSPlayerController player, CBaseProp block)
    {
        var playerPawn = player.Pawn();

        if (playerPawn == null)
            return;

        player.Health((int)- settings.Damage.Value);
        player.PlaySound(sounds.Damage);
    }

    private static void Action_Deagle(CCSPlayerController player, CBaseProp block)
    {
        if (blocksCooldown[player.Slot].Deagle)
            return;

        player.GiveWeapon("weapon_deagle");
        player.FindWeapon("weapon_deagle").SetAmmo(1, 0);

        Plugin.Instance.PrintToChatAll($"{ChatColors.LightPurple}{player.PlayerName} {ChatColors.Grey}has got a Deagle");

        blocksCooldown[player.Slot].Deagle = true;
    }

    private static void Action_AWP(CCSPlayerController player, CBaseProp block)
    {
        if (blocksCooldown[player.Slot].AWP)
            return;

        player.GiveWeapon("weapon_awp");
        player.FindWeapon("weapon_awp").SetAmmo(1, 0);

        Plugin.Instance.PrintToChatAll($"{ChatColors.LightPurple}{player.PlayerName} {ChatColors.Grey}has got an AWP");

        blocksCooldown[player.Slot].AWP = true;
    }

    private static void Action_Trampoline(CCSPlayerController player, CBaseProp block)
    {

    }

    private static void Action_SpeedBoost(CCSPlayerController player, CBaseProp block)
    {

    }

    private static void Action_Speed(CCSPlayerController player, CBaseProp block)
    {
        if (blocksCooldown[player.Slot].Speed)
            return;

        var speed = player.Speed;

        player.Speed = settings.Speed.Value;
        player.PlaySound(sounds.Speed);

        Plugin.Instance.AddTimer(settings.Speed.Duration, () =>
        {
            player.Speed = speed;
            Plugin.Instance.PrintToChat(player, $"{block.Entity!.Name} has worn off");
        });

        blocksCooldown[player.Slot].Speed = true;
        BlockCooldownTimer(player, "Speed", settings.Speed.Cooldown);
    }

    private static void Action_Slap(CCSPlayerController player, CBaseProp block)
    {
        player.Slap((int)settings.Slap.Value);
    }

    private static void Action_Nuke(CCSPlayerController player, CBaseProp block)
    {
        CsTeam teamToNuke = 0;
        string teamName = "";

        if (player.IsT())
        {
            teamToNuke = CsTeam.CounterTerrorist;
            teamName = "Counter-Terrorist";
        }
        else if (player.IsCT())
        {
            teamToNuke = CsTeam.Terrorist;
            teamName = "Terrorist";
        }

        var playersToNuke = Utilities.GetPlayers().Where(p => p.Team == teamToNuke);

        foreach (var playerToNuke in playersToNuke)
            playerToNuke.CommitSuicide(false, true);

        Plugin.Instance.PrintToChatAll($"{ChatColors.LightPurple}{player.PlayerName} {ChatColors.Grey}has nuked the {teamName} team");

        Plugin.Instance.PlaySoundAll(sounds.Nuke);
    }

    private static void Action_Stealth(CCSPlayerController player, CBaseProp block)
    {
        if (blocksCooldown[player.Slot].Stealth)
            return;

        player.SetInvis(true);
        player.PlaySound(sounds.Stealth);
        player.ColorScreen(Color.FromArgb(150, 100, 100, 100), 2.5f, 5.0f, EntityExtends.FadeFlags.FADE_OUT);

        Plugin.Instance.AddTimer(settings.Stealth.Duration, () =>
        {
            player.SetInvis(false);
            Plugin.Instance.PrintToChat(player, $"{block.Entity!.Name} has worn off");
        });

        blocksCooldown[player.Slot].Stealth = true;
        BlockCooldownTimer(player, "Stealth", settings.Stealth.Cooldown);
    }

    private static void Action_Invincibility(CCSPlayerController player, CBaseProp block)
    {
        if (blocksCooldown[player.Slot].Invincibility)
            return;

        player.Pawn()!.TakesDamage = false;
        player.PlaySound(sounds.Invincibility);
        player.ColorScreen(Color.FromArgb(100, 100, 0, 100), 2.5f, 5.0f, EntityExtends.FadeFlags.FADE_OUT);

        Plugin.Instance.AddTimer(settings.Invincibility.Duration, () =>
        {
            player.Pawn()!.TakesDamage = true;
            Plugin.Instance.PrintToChat(player, $"{block.Entity!.Name} has worn off");
        });

        blocksCooldown[player.Slot].Invincibility = true;
        BlockCooldownTimer(player, "Invincibility", settings.Invincibility.Cooldown);
    }

    private static void Action_Camouflage(CCSPlayerController player, CBaseProp block)
    {
        if (blocksCooldown[player.Slot].Camouflage)
            return;

        var model = player.Pawn()!.CBodyComponent!.SceneNode!.GetSkeletonInstance().ModelState.ModelName;

        if (player.IsT())
            player.SetModel(settings.Camouflage.ModelT);
        else if (player.IsCT())
            player.SetModel(settings.Camouflage.ModelCT);

        player.PlaySound(sounds.Camouflage);

        Plugin.Instance.AddTimer(settings.Camouflage.Duration, () =>
        {
            player.SetModel(model);
            Plugin.Instance.PrintToChat(player, $"{block.Entity!.Name} has worn off");
        });

        blocksCooldown[player.Slot].Camouflage = true;
        BlockCooldownTimer(player, "Camouflage", settings.Camouflage.Cooldown);
    }

    private static void Action_Platform(CCSPlayerController player, CBaseProp block) { }
    private static void Action_Honey(CCSPlayerController player, CBaseProp block) { }
    private static void Action_Glass(CCSPlayerController player, CBaseProp block) { }
    private static void Action_TBarrier(CCSPlayerController player, CBaseProp block) { }
    private static void Action_CTBarrier(CCSPlayerController player, CBaseProp block) { }
    private static void Action_Ice(CCSPlayerController player, CBaseProp block) { }
    private static void Action_NoSlowDown(CCSPlayerController player, CBaseProp block) { }
}