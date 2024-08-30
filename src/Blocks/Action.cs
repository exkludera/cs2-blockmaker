using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using System.Drawing;
using System.Reflection;

namespace BlockMaker;

public partial class Plugin
{
    public Dictionary<int, BlocksCooldown> blocksCooldown = new Dictionary<int, BlocksCooldown>();

    public void BlockCooldownTimer(CCSPlayerController player, string block, float timer)
    {
        if (timer == 0)
            return;

        AddTimer(timer, () =>
        {
            PropertyInfo blockProperty = typeof(BlocksCooldown).GetProperty(block)!;

            if (blockProperty != null && blockProperty.PropertyType == typeof(bool))
            {
                bool currentValue = (bool)blockProperty.GetValue(blocksCooldown[player.Slot])!;

                if (currentValue)
                {
                    blockProperty.SetValue(blocksCooldown[player.Slot], false);
                    PrintToChat(player, $"{ChatColors.White}{block} {ChatColors.Grey}block cooldown has reset");
                }
            }
        });
    }

    public void BlockActions(CCSPlayerController player, CPhysicsPropOverride block)
    {
        if (block == null || player.NotValid())
            return;

        if (block.Entity == null || string.IsNullOrEmpty(block.Entity.Name))
            return;

        if (!blocksCooldown.ContainsKey(player.Slot))
            blocksCooldown[player.Slot] = new BlocksCooldown();


        if (block.Entity.Name == "Random")
        {
            if (blocksCooldown[player.Slot].Random)
                return;

            string[] randomBlocks =
                {
                    "Gravity", "Grenade", "Frost", "Flash", "AWP",
                    "Deagle", "Camouflage", "Invincibility", "Stealth",
                    "Speed", "Death", "SpeedBoost",
                };

            Random random = new Random();
            int randomIndex = random.Next(0, randomBlocks.Length);
            string randomBlock = randomBlocks[randomIndex];

            PrintToChat(player, $"You got {ChatColors.White}{randomBlock} {ChatColors.Grey}from the {ChatColors.White}Random {ChatColors.Grey}block");

            block.Entity.Name = randomBlock;

            blocksCooldown[player.Slot].Random = true;

            BlockCooldownTimer(player, "Random", Config.Settings.Blocks.Cooldowns.Random);
        }

        if (block.Entity.Name == "Bhop")
        {
            var render = block.Render;

            AddTimer(0.1f, () =>
            {
                //disable?
                block.Render = Color.FromArgb(100, 100, 100, 100);
                Utilities.SetStateChanged(block, "CBaseModelEntity", "m_clrRender");
            });

            AddTimer(Config.Settings.Blocks.Cooldowns.Bhop, () =>
            {
                //enable?
                block.Render = render;
                Utilities.SetStateChanged(block, "CBaseModelEntity", "m_clrRender");
            });

            return;
        }

        if (block.Entity.Name == "NoFallDmg")
        {
            player.PlayerPawn.Value!.TakesDamage = false;
            AddTimer(0.01f, () => { player.PlayerPawn.Value.TakesDamage = true; });
            return;
        }

        if (block.Entity.Name == "Gravity")
        {
            var gravity = player.GravityScale;

            player.SetGravity(Config.Settings.Blocks.Values.Gravity);

            AddTimer(Config.Settings.Blocks.Durations.Gravity, () =>
            {
                player.SetGravity(gravity);
                PrintToChat(player, "Gravity has worn off");
            });

            return;
        }

        if (block.Entity.Name == "Health")
        {
            var playerPawn = player.Pawn();

            if (playerPawn == null)
                return;

            player.Health(+2);

            player.PlaySound(Config.Sounds.Blocks.Health);

            player.ColorScreen(Color.FromArgb(150, 255, 0, 0), 0.25f, 0.5f, EntityExtends.FadeFlags.FADE_OUT);

            return;
        }

        if (block.Entity.Name == "Grenade")
        {
            if (blocksCooldown[player.Slot].Grenade)
                return;

            player.GiveWeapon("weapon_hegrenade");

            blocksCooldown[player.Slot].Grenade = true;

            BlockCooldownTimer(player, "Grenade", Config.Settings.Blocks.Cooldowns.HEGrenade);

            return;
        }

        if (block.Entity.Name == "Frost")
        {
            if (blocksCooldown[player.Slot].Frost)
                return;

            player.GiveWeapon("weapon_smokegrenade");

            blocksCooldown[player.Slot].Frost = true;

            BlockCooldownTimer(player, "Frost", Config.Settings.Blocks.Cooldowns.FrostGrenade);

            return;
        }

        if (block.Entity.Name == "Flash")
        {
            if (blocksCooldown[player.Slot].Flash)
                return;

            player.GiveWeapon("weapon_flashbang");

            blocksCooldown[player.Slot].Flash = true;

            BlockCooldownTimer(player, "Flash", Config.Settings.Blocks.Cooldowns.Flashbang);

            return;
        }

        if (block.Entity.Name == "Fire")
        {
            var fire = Utilities.CreateEntityByName<CParticleSystem>("info_particle_system")!;

            fire.EffectName = "particles/inferno_fx/molotov_fire01.vpcf";
            fire.DispatchSpawn();
            fire.AcceptInput("Start");

            player.Health(-Config.Settings.Blocks.Values.Fire);

            player.PlaySound(Config.Sounds.Blocks.Damage);

            return;
        }

        if (block.Entity.Name == "Delay")
        {
            var render = block.Render;
            
            AddTimer(Config.Settings.Blocks.Durations.Delay, () =>
            {
                //disable?
                block.Render = Color.FromArgb(100, 100, 100, 100);
                Utilities.SetStateChanged(block, "CBaseModelEntity", "m_clrRender");
            });

            AddTimer(Config.Settings.Blocks.Cooldowns.Delay, () =>
            {
                //enable?
                block.Render = render;
                Utilities.SetStateChanged(block, "CBaseModelEntity", "m_clrRender");
            });

            return;
        }

        if (block.Entity.Name == "Death")
        {
            player.CommitSuicide(false, true);
            return;
        }

        if (block.Entity.Name == "Damage")
        {
            var playerPawn = player.Pawn();

            if (playerPawn == null)
                return;

            player.Health(-Config.Settings.Blocks.Values.Damage);

            player.PlaySound(Config.Sounds.Blocks.Damage);

            return;
        }

        if (block.Entity.Name == "Deagle")
        {
            if (blocksCooldown[player.Slot].Deagle)
                return;

            player.GiveWeapon("weapon_deagle");
            player.FindWeapon("weapon_deagle").SetAmmo(1,0);
            PrintToChatAll($"{ChatColors.LightPurple}{player.PlayerName} {ChatColors.Grey}has got a Deagle");

            blocksCooldown[player.Slot].Deagle = true;

            return;
        }

        if (block.Entity.Name == "AWP")
        {
            if (blocksCooldown[player.Slot].AWP)
                return;

            player.GiveWeapon("weapon_awp");
            player.FindWeapon("weapon_awp").SetAmmo(1, 0);
            PrintToChatAll($"{ChatColors.LightPurple}{player.PlayerName} {ChatColors.Grey}has got an AWP");

            blocksCooldown[player.Slot].AWP = true;

            return;
        }

        if (block.Entity.Name == "Trampoline")
        {
            return;
        }

        if (block.Entity.Name == "SpeedBoost")
        {
            return;
        }

        if (block.Entity.Name == "Speed")
        {
            if (blocksCooldown[player.Slot].Speed)
                return;

            var speed = player.Speed;

            player.Speed = Config.Settings.Blocks.Values.Speed;

            player.PlaySound(Config.Sounds.Blocks.Speed);

            AddTimer(Config.Settings.Blocks.Durations.Speed, () =>
            {
                player.Speed = speed;
                PrintToChat(player, "Speed has worn off");
            });

            blocksCooldown[player.Slot].Speed = true;

            BlockCooldownTimer(player, "Speed", Config.Settings.Blocks.Cooldowns.Speed);

            return;
        }

        if (block.Entity.Name == "Slap")
        {
            player.Slap(Config.Settings.Blocks.Values.Slap);
            return;
        }

        if (block.Entity.Name == "Nuke")
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

            PrintToChatAll($"{ChatColors.LightPurple}{player.PlayerName} {ChatColors.Grey}has nuked the {teamName} team");

            PlaySoundAll(Config.Sounds.Blocks.Nuke);

            return;
        }

        if (block.Entity.Name == "Stealth")
        {
            if (blocksCooldown[player.Slot].Stealth)
                return;

            player.SetInvis(true);

            player.PlaySound(Config.Sounds.Blocks.Stealth);

            player.ColorScreen(Color.FromArgb(150, 100, 100, 100), 2.5f, 5.0f, EntityExtends.FadeFlags.FADE_OUT);

            AddTimer(Config.Settings.Blocks.Durations.Stealth, () =>
            {
                player.SetInvis(false);
                PrintToChat(player, "Stealth has worn off");
            });

            blocksCooldown[player.Slot].Stealth = true;

            BlockCooldownTimer(player, "Stealth", Config.Settings.Blocks.Cooldowns.Stealth);

            return;
        }

        if (block.Entity.Name == "Invincibility")
        {
            if (blocksCooldown[player.Slot].Invincibility)
                return;

            player.TakesDamage = false;

            player.PlaySound(Config.Sounds.Blocks.Invincibility);

            player.ColorScreen(Color.FromArgb(150, 100, 0, 100), 2.5f, 5.0f, EntityExtends.FadeFlags.FADE_OUT);

            AddTimer(Config.Settings.Blocks.Durations.Invincibility, () =>
            {
                player.TakesDamage = true;
                PrintToChat(player, "Invincibility has worn off");
            });

            blocksCooldown[player.Slot].Invincibility = true;

            BlockCooldownTimer(player, "Invincibility", Config.Settings.Blocks.Cooldowns.Invincibility);

            return;
        }

        if (block.Entity.Name == "Camouflage")
        {
            if (blocksCooldown[player.Slot].Camouflage)
                return;

            var model = player.CBodyComponent!.SceneNode!.GetSkeletonInstance().ModelState.ModelName;

             if (player.IsT())
                player.SetModel(Config.Settings.Blocks.Values.CamouflageT);

            else if (player.IsCT())
                player.SetModel(Config.Settings.Blocks.Values.CamouflageT);

            player.PlaySound(Config.Sounds.Blocks.Camouflage);

            AddTimer(Config.Settings.Blocks.Durations.Camouflage, () =>
            {
                player.SetModel(model);
                PrintToChat(player, "Camouflage has worn off");
            });

            blocksCooldown[player.Slot].Camouflage = true;

            BlockCooldownTimer(player, "Camouflage", Config.Settings.Blocks.Cooldowns.Camouflage);

            return;
        }

        if (block.Entity.Name == "Platform")
        {
            return;
        }

        if (block.Entity.Name == "Honey")
        {
            return;
        }

        if (block.Entity.Name == "Glass")
        {
            return;
        }

        if (block.Entity.Name == "T-Barrier")
        {
            return;
        }

        if (block.Entity.Name == "CT-Barrier")
        {
            return;
        }

        if (block.Entity.Name == "Ice")
        {
            return;
        }

        if (block.Entity.Name == "NoSlowDown")
        {
            return;
        }
    }
}