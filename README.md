# cs2-blockbuilder

**a plugin to create and save blocks, mostly for hns**

> block managing can be done within the building menu /buildmenu
> 
> USE button to hold block, look around to move, left and right click to change distance
> 
> RELOAD button to hold block, left click to rotate horizontal axis, right click to rotate vertical axis

> [!CAUTION]
> **this project is not finished! any help would be very appreciated.**

<br>

**showcase:**<br>
[![showcase](https://img.youtube.com/vi/AEAEKhCErsw/hqdefault.jpg)](https://youtube.com/watch?v=AEAEKhCErsw)

<br>

## information:

### requirements

- [MetaMod](https://cs2.poggu.me/metamod/installation)
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)
- [MultiAddonManager](https://github.com/Source2ZE/MultiAddonManager)
- [BlockBuilder Assets](https://steamcommunity.com/sharedfiles/filedetails/?id=3299954847)

<br>

> [!NOTE]
> thanks to [UgurhanK/BaseBuilder](https://github.com/UgurhanK/BaseBuilder) for the code base
>
> inspired by [BlockBuilder by x3ro](https://forums.alliedmods.net/showthread.php?t=258329) (block models are from them also)

<img src="https://media.discordapp.net/attachments/1051988905320255509/1146537451750432778/ezgif.com-video-to-gif_2.gif?ex=66a359f6&is=66a20876&hm=768e346857f44792cf5b2917fe55b525522029ecccac95bb765b881baa6660d7&" width="250">

<br>

<a href='https://ko-fi.com/G2G2Y3Z9R' target='_blank'><img style='border:0px; height:75px;' src='https://storage.ko-fi.com/cdn/brandasset/kofi_s_tag_dark.png?_gl=1*6vhavf*_gcl_au*MTIwNjcwMzM4OC4xNzE1NzA0NjM5*_ga*NjE5MjYyMjkzLjE3MTU3MDQ2MTM.*_ga_M13FZ7VQ2C*MTcyMjIwMDA2NS4xNy4xLjE3MjIyMDA0MDUuNjAuMC4w' border='0' alt='Buy Me a Coffee at ko-fi.com' /></a> <br>

## example config

```json
{
  "Prefix": "{purple}[BlockBuilder]{default}",
  "Menu": {
    "Type": "html",
    "Command": "css_blockmenu,css_blocksmenu,css_buildmenu"
  },
  "BuildMode": {
    "Enabled": true,
    "Permission": "@css/root",
    "Command": "css_buildmode,css_togglebuild"
  },
  "Sounds": {
    "Enabled": true,
    "Rotate": "sounds/buttons/button9.vsnd",
    "Create": "sounds/buttons/blip1.vsnd",
    "Remove": "sounds/buttons/blip2.vsnd",
    "Place": "sounds/buttons/latchunlocked2.vsnd",
    "Save": "sounds/buttons/bell1.vsnd"
  },
  "AutoSave": {
    "Enabled": false,
    "Time": 120
  },
  "Blocks": {
    "PLATFORM": {
      "Default": "models/blockbuilder/platform.vmdl",
      "Small": "models/blockbuilder/small_platform.vmdl",
      "Large": "models/blockbuilder/large_platform.vmdl",
      "Pole": "models/blockbuilder/pole_platform.vmdl"
    },
    "BHOP": {
      "Default": "models/blockbuilder/bhop.vmdl",
      "Small": "models/blockbuilder/small_bhop.vmdl",
      "Large": "models/blockbuilder/large_bhop.vmdl",
      "Pole": "models/blockbuilder/pole_bhop.vmdl"
    },
    "NOFALLDMG": {
      "Default": "models/blockbuilder/nofalldmg.vmdl",
      "Small": "models/blockbuilder/small_nofalldmg.vmdl",
      "Large": "models/blockbuilder/large_nofalldmg.vmdl",
      "Pole": "models/blockbuilder/pole_nofalldmg.vmdl"
    },
    "HONEY": {
      "Default": "models/blockbuilder/honey.vmdl",
      "Small": "models/blockbuilder/small_honey.vmdl",
      "Large": "models/blockbuilder/large_honey.vmdl",
      "Pole": "models/blockbuilder/pole_honey.vmdl"
    },
    "HEALTH": {
      "Default": "models/blockbuilder/health.vmdl",
      "Small": "models/blockbuilder/small_health.vmdl",
      "Large": "models/blockbuilder/large_health.vmdl",
      "Pole": "models/blockbuilder/pole_health.vmdl"
    },
    "GRENADE": {
      "Default": "models/blockbuilder/he.vmdl",
      "Small": "models/blockbuilder/small_he.vmdl",
      "Large": "models/blockbuilder/large_he.vmdl",
      "Pole": "models/blockbuilder/pole_he.vmdl"
    },
    "GRAVITY": {
      "Default": "models/blockbuilder/gravity.vmdl",
      "Small": "models/blockbuilder/small_gravity.vmdl",
      "Large": "models/blockbuilder/large_gravity.vmdl",
      "Pole": "models/blockbuilder/pole_gravity.vmdl"
    },
    "GLASS": {
      "Default": "models/blockbuilder/glass.vmdl",
      "Small": "models/blockbuilder/small_glass.vmdl",
      "Large": "models/blockbuilder/large_glass.vmdl",
      "Pole": "models/blockbuilder/pole_glass.vmdl"
    },
    "FROST": {
      "Default": "models/blockbuilder/frost.vmdl",
      "Small": "models/blockbuilder/small_frost.vmdl",
      "Large": "models/blockbuilder/large_frost.vmdl",
      "Pole": "models/blockbuilder/pole_frost.vmdl"
    },
    "FLASH": {
      "Default": "models/blockbuilder/flash.vmdl",
      "Small": "models/blockbuilder/small_flash.vmdl",
      "Large": "models/blockbuilder/large_flash.vmdl",
      "Pole": "models/blockbuilder/pole_flash.vmdl"
    },
    "FIRE": {
      "Default": "models/blockbuilder/fire.vmdl",
      "Small": "models/blockbuilder/small_fire.vmdl",
      "Large": "models/blockbuilder/large_fire.vmdl",
      "Pole": "models/blockbuilder/pole_fire.vmdl"
    },
    "DELAY": {
      "Default": "models/blockbuilder/delay.vmdl",
      "Small": "models/blockbuilder/small_delay.vmdl",
      "Large": "models/blockbuilder/large_delay.vmdl",
      "Pole": "models/blockbuilder/pole_delay.vmdl"
    },
    "DEATH": {
      "Default": "models/blockbuilder/death.vmdl",
      "Small": "models/blockbuilder/small_death.vmdl",
      "Large": "models/blockbuilder/large_death.vmdl",
      "Pole": "models/blockbuilder/pole_death.vmdl"
    },
    "DAMAGE": {
      "Default": "models/blockbuilder/damage.vmdl",
      "Small": "models/blockbuilder/small_damage.vmdl",
      "Large": "models/blockbuilder/large_damage.vmdl",
      "Pole": "models/blockbuilder/pole_damage.vmdl"
    },
    "DEAGLE": {
      "Default": "models/blockbuilder/deagle.vmdl",
      "Small": "models/blockbuilder/small_deagle.vmdl",
      "Large": "models/blockbuilder/large_deagle.vmdl",
      "Pole": "models/blockbuilder/pole_deagle.vmdl"
    },
    "AWP": {
      "Default": "models/blockbuilder/awp.vmdl",
      "Small": "models/blockbuilder/small_awp.vmdl",
      "Large": "models/blockbuilder/large_awp.vmdl",
      "Pole": "models/blockbuilder/pole_awp.vmdl"
    },
    "TRAMPOLINE": {
      "Default": "models/blockbuilder/tramp.vmdl",
      "Small": "models/blockbuilder/small_tramp.vmdl",
      "Large": "models/blockbuilder/large_tramp.vmdl",
      "Pole": "models/blockbuilder/pole_tramp.vmdl"
    },
    "STEALTH": {
      "Default": "models/blockbuilder/stealth.vmdl",
      "Small": "models/blockbuilder/small_stealth.vmdl",
      "Large": "models/blockbuilder/large_stealth.vmdl",
      "Pole": "models/blockbuilder/pole_stealth.vmdl"
    },
    "SPEEDBOOST": {
      "Default": "models/blockbuilder/speedboost.vmdl",
      "Small": "models/blockbuilder/small_speedboost.vmdl",
      "Large": "models/blockbuilder/large_speedboost.vmdl",
      "Pole": "models/blockbuilder/pole_speedboost.vmdl"
    },
    "SPEED": {
      "Default": "models/blockbuilder/speed.vmdl",
      "Small": "models/blockbuilder/small_speed.vmdl",
      "Large": "models/blockbuilder/large_speed.vmdl",
      "Pole": "models/blockbuilder/pole_speed.vmdl"
    },
    "T-BARRIER": {
      "Default": "models/blockbuilder/tbarrier.vmdl",
      "Small": "models/blockbuilder/small_tbarrier.vmdl",
      "Large": "models/blockbuilder/large_tbarrier.vmdl",
      "Pole": "models/blockbuilder/pole_tbarrier.vmdl"
    },
    "CT-BARRIER": {
      "Default": "models/blockbuilder/ctbarrier.vmdl",
      "Small": "models/blockbuilder/small_ctbarrier.vmdl",
      "Large": "models/blockbuilder/large_ctbarrier.vmdl",
      "Pole": "models/blockbuilder/pole_ctbarrier.vmdl"
    },
    "SLAP": {
      "Default": "models/blockbuilder/slap.vmdl",
      "Small": "models/blockbuilder/small_slap.vmdl",
      "Large": "models/blockbuilder/large_slap.vmdl",
      "Pole": "models/blockbuilder/pole_slap.vmdl"
    },
    "RANDOM": {
      "Default": "models/blockbuilder/random.vmdl",
      "Small": "models/blockbuilder/small_random.vmdl",
      "Large": "models/blockbuilder/large_random.vmdl",
      "Pole": "models/blockbuilder/pole_random.vmdl"
    },
    "NUKE": {
      "Default": "models/blockbuilder/nuke.vmdl",
      "Small": "models/blockbuilder/small_nuke.vmdl",
      "Large": "models/blockbuilder/large_nuke.vmdl",
      "Pole": "models/blockbuilder/pole_nuke.vmdl"
    },
    "NOSLOWDOWN": {
      "Default": "models/blockbuilder/noslowdown.vmdl",
      "Small": "models/blockbuilder/small_noslowdown.vmdl",
      "Large": "models/blockbuilder/large_noslowdown.vmdl",
      "Pole": "models/blockbuilder/pole_noslowdown.vmdl"
    },
    "INVINCIBILITY": {
      "Default": "models/blockbuilder/invincibility.vmdl",
      "Small": "models/blockbuilder/small_invincibility.vmdl",
      "Large": "models/blockbuilder/large_invincibility.vmdl",
      "Pole": "models/blockbuilder/pole_invincibility.vmdl"
    },
    "ICE": {
      "Default": "models/blockbuilder/ice.vmdl",
      "Small": "models/blockbuilder/small_ice.vmdl",
      "Large": "models/blockbuilder/large_ice.vmdl",
      "Pole": "models/blockbuilder/pole_ice.vmdl"
    },
    "CAMOUFLAGE": {
      "Default": "models/blockbuilder/camouflage.vmdl",
      "Small": "models/blockbuilder/small_camouflage.vmdl",
      "Large": "models/blockbuilder/large_camouflage.vmdl",
      "Pole": "models/blockbuilder/pole_camouflage.vmdl"
    }
  }
}
```