public class BlockSizes
{
    public string Title { get; set; } = "";
    public string Small { get; set; } = "";
    public string Medium { get; set; } = "";
    public string Large { get; set; } = "";
    public string Pole { get; set; } = "";
}

public class BlockModels
{
    public BlockPlatform Platform { get; set; } = new BlockPlatform();
    public BlockBhop Bhop { get; set; } = new BlockBhop();
    public BlockNoFallDmg NoFallDmg { get; set; } = new BlockNoFallDmg();
    public BlockHoney Honey { get; set; } = new BlockHoney();
    public BlockHealth Health { get; set; } = new BlockHealth();
    public BlockGrenade Grenade { get; set; } = new BlockGrenade();
    public BlockGravity Gravity { get; set; } = new BlockGravity();
    public BlockGlass Glass { get; set; } = new BlockGlass();
    public BlockFrost Frost { get; set; } = new BlockFrost();
    public BlockFlash Flash { get; set; } = new BlockFlash();
    public BlockFire Fire { get; set; } = new BlockFire();
    public BlockDelay Delay { get; set; } = new BlockDelay();
    public BlockDeath Death { get; set; } = new BlockDeath();
    public BlockDamage Damage { get; set; } = new BlockDamage();
    public BlockDeagle Deagle { get; set; } = new BlockDeagle();
    public BlockAWP AWP { get; set; } = new BlockAWP();
    public BlockTrampoline Trampoline { get; set; } = new BlockTrampoline();
    public BlockStealth Stealth { get; set; } = new BlockStealth();
    public BlockSpeedBoost SpeedBoost { get; set; } = new BlockSpeedBoost();
    public BlockSpeed Speed { get; set; } = new BlockSpeed();
    public BlockTBarrier TBarrier { get; set; } = new BlockTBarrier();
    public BlockCTBarrier CTBarrier { get; set; } = new BlockCTBarrier();
    public BlockSlap Slap { get; set; } = new BlockSlap();
    public BlockRandom Random { get; set; } = new BlockRandom();
    public BlockNuke Nuke { get; set; } = new BlockNuke();
    public BlockNoSlowDown NoSlowDown { get; set; } = new BlockNoSlowDown();
    public BlockInvincibility Invincibility { get; set; } = new BlockInvincibility();
    public BlockIce Ice { get; set; } = new BlockIce();
    public BlockCamouflage Camouflage { get; set; } = new BlockCamouflage();
}


public class BlockPlatform : BlockSizes
{
    public BlockPlatform()
    {
        Title = "Platform";
        Small = "models/blockbuilder/small_platform.vmdl";
        Medium = "models/blockbuilder/platform.vmdl";
        Large = "models/blockbuilder/large_platform.vmdl";
        Pole = "models/blockbuilder/pole_platform.vmdl";
    }
}

public class BlockBhop : BlockSizes
{
    public BlockBhop()
    {
        Title = "Bhop";
        Small = "models/blockbuilder/small_bhop.vmdl";
        Medium = "models/blockbuilder/bhop.vmdl";
        Large = "models/blockbuilder/large_bhop.vmdl";
        Pole = "models/blockbuilder/pole_bhop.vmdl";
    }
}

public class BlockNoFallDmg : BlockSizes
{
    public BlockNoFallDmg()
    {
        Title = "NoFallDmg";
        Small = "models/blockbuilder/small_nofalldmg.vmdl";
        Medium = "models/blockbuilder/nofalldmg.vmdl";
        Large = "models/blockbuilder/large_nofalldmg.vmdl";
        Pole = "models/blockbuilder/pole_nofalldmg.vmdl";
    }
}

public class BlockHoney : BlockSizes
{
    public BlockHoney()
    {
        Title = "Honey";
        Small = "models/blockbuilder/small_honey.vmdl";
        Medium = "models/blockbuilder/honey.vmdl";
        Large = "models/blockbuilder/large_honey.vmdl";
        Pole = "models/blockbuilder/pole_honey.vmdl";
    }
}

public class BlockHealth : BlockSizes
{
    public BlockHealth()
    {
        Title = "Health";
        Small = "models/blockbuilder/small_health.vmdl";
        Medium = "models/blockbuilder/health.vmdl";
        Large = "models/blockbuilder/large_health.vmdl";
        Pole = "models/blockbuilder/pole_health.vmdl";
    }
}

public class BlockGrenade : BlockSizes
{
    public BlockGrenade()
    {
        Title = "Grenade";
        Small = "models/blockbuilder/small_he.vmdl";
        Medium = "models/blockbuilder/he.vmdl";
        Large = "models/blockbuilder/large_he.vmdl";
        Pole = "models/blockbuilder/pole_he.vmdl";
    }
}

public class BlockGravity : BlockSizes
{
    public BlockGravity()
    {
        Title = "Gravity";
        Small = "models/blockbuilder/small_gravity.vmdl";
        Medium = "models/blockbuilder/gravity.vmdl";
        Large = "models/blockbuilder/large_gravity.vmdl";
        Pole = "models/blockbuilder/pole_gravity.vmdl";
    }
}

public class BlockGlass : BlockSizes
{
    public BlockGlass()
    {
        Title = "Glass";
        Small = "models/blockbuilder/small_glass.vmdl";
        Medium = "models/blockbuilder/glass.vmdl";
        Large = "models/blockbuilder/large_glass.vmdl";
        Pole = "models/blockbuilder/pole_glass.vmdl";
    }
}

public class BlockFrost : BlockSizes
{
    public BlockFrost()
    {
        Title = "Frost";
        Small = "models/blockbuilder/small_frost.vmdl";
        Medium = "models/blockbuilder/frost.vmdl";
        Large = "models/blockbuilder/large_frost.vmdl";
        Pole = "models/blockbuilder/pole_frost.vmdl";
    }
}

public class BlockFlash : BlockSizes
{
    public BlockFlash()
    {
        Title = "Flash";
        Small = "models/blockbuilder/small_flash.vmdl";
        Medium = "models/blockbuilder/flash.vmdl";
        Large = "models/blockbuilder/large_flash.vmdl";
        Pole = "models/blockbuilder/pole_flash.vmdl";
    }
}

public class BlockFire : BlockSizes
{
    public BlockFire()
    {
        Title = "Fire";
        Small = "models/blockbuilder/small_fire.vmdl";
        Medium = "models/blockbuilder/fire.vmdl";
        Large = "models/blockbuilder/large_fire.vmdl";
        Pole = "models/blockbuilder/pole_fire.vmdl";
    }
}

public class BlockDelay : BlockSizes
{
    public BlockDelay()
    {
        Title = "Delay";
        Small = "models/blockbuilder/small_delay.vmdl";
        Medium = "models/blockbuilder/delay.vmdl";
        Large = "models/blockbuilder/large_delay.vmdl";
        Pole = "models/blockbuilder/pole_delay.vmdl";
    }
}

public class BlockDeath : BlockSizes
{
    public BlockDeath()
    {
        Title = "Death";
        Small = "models/blockbuilder/small_death.vmdl";
        Medium = "models/blockbuilder/death.vmdl";
        Large = "models/blockbuilder/large_death.vmdl";
        Pole = "models/blockbuilder/pole_death.vmdl";
    }
}

public class BlockDamage : BlockSizes
{
    public BlockDamage()
    {
        Title = "Damage";
        Small = "models/blockbuilder/small_damage.vmdl";
        Medium = "models/blockbuilder/damage.vmdl";
        Large = "models/blockbuilder/large_damage.vmdl";
        Pole = "models/blockbuilder/pole_damage.vmdl";
    }
}

public class BlockDeagle : BlockSizes
{
    public BlockDeagle()
    {
        Title = "Deagle";
        Small = "models/blockbuilder/small_deagle.vmdl";
        Medium = "models/blockbuilder/deagle.vmdl";
        Large = "models/blockbuilder/large_deagle.vmdl";
        Pole = "models/blockbuilder/pole_deagle.vmdl";
    }
}

public class BlockAWP : BlockSizes
{
    public BlockAWP()
    {
        Title = "AWP";
        Small = "models/blockbuilder/small_awp.vmdl";
        Medium = "models/blockbuilder/awp.vmdl";
        Large = "models/blockbuilder/large_awp.vmdl";
        Pole = "models/blockbuilder/pole_awp.vmdl";
    }
}

public class BlockTrampoline : BlockSizes
{
    public BlockTrampoline()
    {
        Title = "Trampoline";
        Small = "models/blockbuilder/small_tramp.vmdl";
        Medium = "models/blockbuilder/tramp.vmdl";
        Large = "models/blockbuilder/large_tramp.vmdl";
        Pole = "models/blockbuilder/pole_tramp.vmdl";
    }
}

public class BlockStealth : BlockSizes
{
    public BlockStealth()
    {
        Title = "Stealth";
        Small = "models/blockbuilder/small_stealth.vmdl";
        Medium = "models/blockbuilder/stealth.vmdl";
        Large = "models/blockbuilder/large_stealth.vmdl";
        Pole = "models/blockbuilder/pole_stealth.vmdl";
    }
}

public class BlockSpeedBoost : BlockSizes
{
    public BlockSpeedBoost()
    {
        Title = "SpeedBoost";
        Small = "models/blockbuilder/small_speedboost.vmdl";
        Medium = "models/blockbuilder/speedboost.vmdl";
        Large = "models/blockbuilder/large_speedboost.vmdl";
        Pole = "models/blockbuilder/pole_speedboost.vmdl";
    }
}

public class BlockSpeed : BlockSizes
{
    public BlockSpeed()
    {
        Title = "Speed";
        Small = "models/blockbuilder/small_speed.vmdl";
        Medium = "models/blockbuilder/speed.vmdl";
        Large = "models/blockbuilder/large_speed.vmdl";
        Pole = "models/blockbuilder/pole_speed.vmdl";
    }
}

public class BlockTBarrier : BlockSizes
{
    public BlockTBarrier()
    {
        Title = "T-Barrier";
        Small = "models/blockbuilder/small_tbarrier.vmdl";
        Medium = "models/blockbuilder/tbarrier.vmdl";
        Large = "models/blockbuilder/large_tbarrier.vmdl";
        Pole = "models/blockbuilder/pole_tbarrier.vmdl";
    }
}

public class BlockCTBarrier : BlockSizes
{
    public BlockCTBarrier()
    {
        Title = "CT-Barrier";
        Small = "models/blockbuilder/small_ctbarrier.vmdl";
        Medium = "models/blockbuilder/ctbarrier.vmdl";
        Large = "models/blockbuilder/large_ctbarrier.vmdl";
        Pole = "models/blockbuilder/pole_ctbarrier.vmdl";
    }
}

public class BlockSlap : BlockSizes
{
    public BlockSlap()
    {
        Title = "Slap";
        Small = "models/blockbuilder/small_slap.vmdl";
        Medium = "models/blockbuilder/slap.vmdl";
        Large = "models/blockbuilder/large_slap.vmdl";
        Pole = "models/blockbuilder/pole_slap.vmdl";
    }
}

public class BlockRandom : BlockSizes
{
    public BlockRandom()
    {
        Title = "Random";
        Small = "models/blockbuilder/small_random.vmdl";
        Medium = "models/blockbuilder/random.vmdl";
        Large = "models/blockbuilder/large_random.vmdl";
        Pole = "models/blockbuilder/pole_random.vmdl";
    }
}

public class BlockNuke : BlockSizes
{
    public BlockNuke()
    {
        Title = "Nuke";
        Small = "models/blockbuilder/small_nuke.vmdl";
        Medium = "models/blockbuilder/nuke.vmdl";
        Large = "models/blockbuilder/large_nuke.vmdl";
        Pole = "models/blockbuilder/pole_nuke.vmdl";
    }
}

public class BlockNoSlowDown : BlockSizes
{
    public BlockNoSlowDown()
    {
        Title = "NoSlowDown";
        Small = "models/blockbuilder/small_noslowdown.vmdl";
        Medium = "models/blockbuilder/noslowdown.vmdl";
        Large = "models/blockbuilder/large_noslowdown.vmdl";
        Pole = "models/blockbuilder/pole_noslowdown.vmdl";
    }
}

public class BlockInvincibility : BlockSizes
{
    public BlockInvincibility()
    {
        Title = "Invincibility";
        Small = "models/blockbuilder/small_invincibility.vmdl";
        Medium = "models/blockbuilder/invincibility.vmdl";
        Large = "models/blockbuilder/large_invincibility.vmdl";
        Pole = "models/blockbuilder/pole_invincibility.vmdl";
    }
}

public class BlockIce : BlockSizes
{
    public BlockIce()
    {
        Title = "Ice";
        Small = "models/blockbuilder/small_ice.vmdl";
        Medium = "models/blockbuilder/ice.vmdl";
        Large = "models/blockbuilder/large_ice.vmdl";
        Pole = "models/blockbuilder/pole_ice.vmdl";
    }
}

public class BlockCamouflage : BlockSizes
{
    public BlockCamouflage()
    {
        Title = "Camouflage";
        Small = "models/blockbuilder/small_camouflage.vmdl";
        Medium = "models/blockbuilder/camouflage.vmdl";
        Large = "models/blockbuilder/large_camouflage.vmdl";
        Pole = "models/blockbuilder/pole_camouflage.vmdl";
    }
}