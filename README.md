<div align="center">
  <img width="50" height="50" alt="cssharp" src="https://github.com/user-attachments/assets/3393573f-29be-46e1-bc30-fafaec573456" />
	<h3><strong>Block Maker</strong></h3>
	<h4>a plugin to create and save blocks, mostly for HNS</h4>
	<h2>
		<img src="https://img.shields.io/github/downloads/exkludera-cssharp/BlockMaker/total" alt="Downloads">
		<img src="https://img.shields.io/github/stars/exkludera-cssharp/BlockMaker?style=flat&logo=github" alt="Stars">
		<img src="https://img.shields.io/github/forks/exkludera-cssharp/BlockMaker?style=flat&logo=github" alt="Forks">
		<img src="https://img.shields.io/github/license/exkludera-cssharp/BlockMaker" alt="License">
	</h2>
	<!--<a href="https://discord.gg" target="_blank"><img src="https://img.shields.io/badge/Discord%20Server-7289da?style=for-the-badge&logo=discord&logoColor=white" /></a> <br>-->
	<a href="https://ko-fi.com/exkludera" target="_blank"><img src="https://img.shields.io/badge/KoFi-af00bf?style=for-the-badge&logo=kofi&logoColor=white" alt="Buy Me a Coffee at ko-fi.com" /></a>
	<a href="https://paypal.com/donate/?hosted_button_id=6AWPNVF5TLUC8" target="_blank"><img src="https://img.shields.io/badge/PayPal-0095ff?style=for-the-badge&logo=paypal&logoColor=white" alt="PayPal"  /></a>
	<a href="https://github.com/sponsors/exkludera" target="_blank"><img src="https://img.shields.io/badge/Sponsor-696969?style=for-the-badge&logo=github&logoColor=white" alt="GitHub Sponsor" /></a>
</div>

> [!NOTE]
> thanks to [UgurhanK/BaseBuilder](https://github.com/UgurhanK/BaseBuilder) for the code base
>
> inspired by [BlockBuilder by x3ro](https://forums.alliedmods.net/showthread.php?t=258329)

> block managing can be done within the building menu /bm or commands
>
> hold USE to grab a block and look around to move it; JUMP brings it closer and DUCK moves it farther away
>
> while holding a block, MOUSE1 duplicates it and transfers the hold to the duplicate; MOUSE2 deletes it
>
> hold RELOAD button and move your mouse to rotate the block

## Changes in this fork (0.2.7)

### Latest live snapshot

- Added an iGz-inspired Duck block using native CS2 crouch state; it currently reuses the Platform models.
- Added separate three-orientation rotation cycles for regular blocks and pole blocks.
- Changed Gravity to remain active through the next landing at `0.25` gravity.
- Preserved the existing Money block and once-per-round cooldown behavior.

- Added a numeric building menu and improved block grabbing, duplication, deletion, rotation, and property editing.
- Improved ray-traced block targeting and face-based snapping, including rotated blocks.
- Added dedicated Small, Normal, Large, and X-Large models so visual size and collision bounds stay aligned.
- Improved teleport placement, exit orientation, and momentum preservation.
- Improved Honey and Trampoline behavior, cooldown handling, and movement-state cleanup.
- Improved layout persistence at round end and builder-state handling after hot reloads.
- Health blocks now heal without a red screen flash or heartbeat sound.
- Updated defaults: Health `1`, Fire `1`, Damage `5`, Death `OnTop` enabled, and Honey `0.2`.
- Added a Money block that defaults to Terrorists, awards `$3,000`, and exposes the award through the editable `Value` property.
- Added `Cooldown = -1` for once-per-round block activation. The lock survives player death and resets when the next round starts.
- Money, grenade, firearm, and temporary-perk blocks now default to once-per-round activation.
- Gravity now matches iGz-style behavior: `0.25` gravity from contact through the next landing, without a timed cooldown.
- Existing generated configuration files are extended and migrated when new built-in defaults are introduced.

## Requirements

- [MetaMod](https://github.com/alliedmodders/metamod-source)
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)
- [MultiAddonManager](https://github.com/Source2ZE/MultiAddonManager)
- [Block Maker Addon](https://steamcommunity.com/sharedfiles/filedetails/?id=3430295154)
- [CS2MenuManager](https://github.com/schwarper/CS2MenuManager)
- [Ray-Trace](https://github.com/FUNPLAY-pro-CS2/Ray-Trace)
- [ChaseMod](https://github.com/ipsvn/ChaseMod) (optional for gameplay)

## Showcase
<details>
	<summary>content</summary>

| YouTube Video                                                                                               |
| ------------------------------------------------------------------------------------------------------------ |
| [![showcase](https://img.youtube.com/vi/IEcDrD1sUSc/hqdefault.jpg)](https://youtube.com/watch?v=IEcDrD1sUSc) |
</details>

## Config

### Block cooldowns

Cooldowns are tracked per player and per block.

| Value | Behavior |
| --- | --- |
| `> 0` | The block can be used again after this many seconds. |
| `0` | No configured cooldown. The block may still have an internal safety delay. |
| `-1` | The block can be activated once per round. Dying does not reset it. |

The once-per-round default applies to Money, Grenade, Frost, Flash, Stealth,
Speed, Camouflage, Random, Invincibility, and every firearm category. Gravity
does not use the standard timed cooldown: it remains active while airborne and
becomes available again after landing. Administrators can enter `-1` through
the block property editor for any action that uses the standard cooldown system.

Existing `default_properties.json` files are migrated only when a value still
matches the previous built-in default, preserving separately customized values.

<details>
<summary>BlockMaker.json</summary>
  
```json
{
  "Settings": {
    "Prefix": "{purple}BlockMaker {grey}|",
    "MenuType": "CenterHtmlMenu",
    "Building": {
      "BuildMode": {
        "Enable": true,
        "Config": false
      },
      "AutoSave": {
        "Enable": true,
        "Timer": 300
      },
      "Grab": {
        "Render": true,
        "RenderColor": "255,255,255,128",
        "Beams": true,
        "BeamsColor": "255,255,255,255"
      }
    },
    "Blocks": {
      "DisableShadows": true,
      "CamouflageT": "characters/models/ctm_fbi/ctm_fbi.vmdl",
      "CamouflageCT": "characters/models/tm_leet/tm_leet_variantb.vmdl",
      "FireParticle": "particles/burning_fx/env_fire_medium.vpcf",
      "Sizes": [
        { "Title": "Small", "Size": 0.25 },
        { "Title": "Normal", "Size": 1 },
        { "Title": "Large", "Size": 2 },
        { "Title": "X-Large", "Size": 3 }
      ],
      "Effects": [
        { "Title": "Fire", "Particle": "particles/burning_fx/env_fire_small.vpcf" },
        { "Title": "Smoke", "Particle": "particles/burning_fx/smoke_gib_01.vpcf" },
        { "Title": "Money", "Particle": "particles/money_fx/moneybag_trail.vpcf" }
      ]
    },
    "Teleports": {
      "ForceAngles": false,
      "AllowEntities": true,
      "Velocity": 300,
      "Entry": {
        "Model": "models/blockmaker/teleport/model.vmdl",
        "Color": "0,255,0,255"
      },
      "Exit": {
        "Model": "models/blockmaker/teleport/model.vmdl",
        "Color": "255,0,0,255"
      }
    },
    "Lights": {
      "Model": "models/generic/interior_lamp_kit_01/ilk01_lamp_01_bulb.vmdl",
      "HideModel": true
    }
  },
  "Commands": {
    "Admin": {
      "Permission": ["@css/root"],
      "BuildMode": ["buildmode"],
      "ManageBuilder": ["builder", "builders"],
      "ResetProperties": ["resetproperties"]
    },
    "Building": {
      "BuildMenu": ["bm", "buildmenu"],
      "CreateBlock": ["create"],
      "DeleteBlock": ["delete"],
      "RotateBlock": ["rotate"],
      "PositionBlock": ["position"],
      "BlockType": ["type"],
      "BlockColor": ["color"],
      "CopyBlock": ["copy"],
      "ConvertBlock": ["convert"],
      "LockBlock": ["lock"],
      "LockAll": ["lockall"],
      "SaveBlocks": ["save"],
      "Snapping": ["snap"],
      "Grid": ["grid"],
      "Noclip": ["nc"],
      "Godmode": ["god"],
      "TestBlock": ["testblock"]
    }
  },
  "Sounds": {
    "SoundEvents": "soundevents/blockmaker.vsndevts",
    "Blocks": {
      "Speed": "bm_speed",
      "Camouflage": "bm_camouflage",
      "Damage": "bm_damage",
      "Fire": "bm_fire",
      "Health": "bm_health",
      "Invincibility": "bm_invincibility",
      "Nuke": "bm_nuke",
      "Stealth": "bm_stealth",
      "Teleport": "bm_teleport"
    },
    "Building": {
      "Enabled": true,
      "Create": "bm_create",
      "Delete": "bm_delete",
      "Place": "bm_place",
      "Rotate": "bm_rotate",
      "Save": "bm_save"
    }
  }
}
```
</details>

## Compiled block sizes

Block sizes now use separate compiled models with matching render and physics
meshes. The built-in model naming convention is:

The Small, Normal, Large, and Pole dimensions follow iGz's BlockMaker for
Counter-Strike 1.6. X-Large remains an additional CS2-only size.

| Menu size | Model suffix | Dimensions |
| --- | --- | --- |
| Small | `block_small.vmdl` | 16 × 16 × 8 |
| Normal | `block.vmdl` | 64 × 64 × 8 |
| Large | `block_large.vmdl` | 128 × 128 × 8 |
| X-Large | `block_xlarge.vmdl` | 192 × 192 × 8 |
| Pole | `pole.vmdl` | 64 × 8 × 8 |

The plugin does not call `SetScale`; the dimensions are baked into each
compiled model. The uncompiled source pack and generator are in
`tools/blockmaker-original-models` at the workspace root.

Built-in entries derive the size-specific paths from `Block`. For a custom
entry, `UseSizeModels` defaults to `false`. Custom models can provide explicit
paths with a `SizeModels` object:

```json
{
  "Title": "Example",
  "Block": "models/custom/example/block.vmdl",
  "Pole": "models/custom/example/pole.vmdl",
  "UseSizeModels": false,
  "SizeModels": {
    "Small": "models/custom/example/my_small.vmdl",
    "Large": "models/custom/example/my_large.vmdl",
    "X-Large": "models/custom/example/my_xlarge.vmdl"
  }
}
```

<details> <summary>models.json</summary>
  
```json
{
  "Platform": {
    "Title": "Platform",
    "Block": "models/blockmaker/platform/block.vmdl",
    "Pole": "models/blockmaker/platform/pole.vmdl"
  },
  "Bhop": {
    "Title": "Bhop",
    "Block": "models/blockmaker/bhop/block.vmdl",
    "Pole": "models/blockmaker/bhop/pole.vmdl"
  },
  "Health": {
    "Title": "Health",
    "Block": "models/blockmaker/health/block.vmdl",
    "Pole": "models/blockmaker/health/pole.vmdl"
  },
  "Money": {
    "Title": "Money",
    "Block": "models/blockmaker/money/block.vmdl",
    "Pole": "models/blockmaker/money/pole.vmdl"
  },
  "Grenade": {
    "Title": "Grenade",
    "Block": "models/blockmaker/grenade/block.vmdl",
    "Pole": "models/blockmaker/grenade/pole.vmdl"
  },
  "Gravity": {
    "Title": "Gravity",
    "Block": "models/blockmaker/gravity/block.vmdl",
    "Pole": "models/blockmaker/gravity/pole.vmdl"
  },
  "Glass": {
    "Title": "Glass",
    "Block": "models/blockmaker/glass/block.vmdl",
    "Pole": "models/blockmaker/glass/pole.vmdl"
  },
  "Frost": {
    "Title": "Frost",
    "Block": "models/blockmaker/frost/block.vmdl",
    "Pole": "models/blockmaker/frost/pole.vmdl"
  },
  "Flash": {
    "Title": "Flash",
    "Block": "models/blockmaker/flash/block.vmdl",
    "Pole": "models/blockmaker/flash/pole.vmdl"
  },
  "Fire": {
    "Title": "Fire",
    "Block": "models/blockmaker/fire/block.vmdl",
    "Pole": "models/blockmaker/fire/pole.vmdl"
  },
  "Delay": {
    "Title": "Delay",
    "Block": "models/blockmaker/delay/block.vmdl",
    "Pole": "models/blockmaker/delay/pole.vmdl"
  },
  "Death": {
    "Title": "Death",
    "Block": "models/blockmaker/death/block.vmdl",
    "Pole": "models/blockmaker/death/pole.vmdl"
  },
  "Damage": {
    "Title": "Damage",
    "Block": "models/blockmaker/damage/block.vmdl",
    "Pole": "models/blockmaker/damage/pole.vmdl"
  },
  "Pistol": {
    "Title": "Pistol",
    "Block": "models/blockmaker/pistol/block.vmdl",
    "Pole": "models/blockmaker/pistol/pole.vmdl"
  },
  "Rifle": {
    "Title": "Rifle",
    "Block": "models/blockmaker/rifle/block.vmdl",
    "Pole": "models/blockmaker/rifle/pole.vmdl"
  },
  "Sniper": {
    "Title": "Sniper",
    "Block": "models/blockmaker/sniper/block.vmdl",
    "Pole": "models/blockmaker/sniper/pole.vmdl"
  },
  "SMG": {
    "Title": "SMG",
    "Block": "models/blockmaker/smg/block.vmdl",
    "Pole": "models/blockmaker/smg/pole.vmdl"
  },
  "ShotgunHeavy": {
    "Title": "Shotgun/Heavy",
    "Block": "models/blockmaker/heavy/block.vmdl",
    "Pole": "models/blockmaker/heavy/pole.vmdl"
  },
  "Stealth": {
    "Title": "Stealth",
    "Block": "models/blockmaker/stealth/block.vmdl",
    "Pole": "models/blockmaker/stealth/pole.vmdl"
  },
  "Speed": {
    "Title": "Speed",
    "Block": "models/blockmaker/speed/block.vmdl",
    "Pole": "models/blockmaker/speed/pole.vmdl"
  },
  "SpeedBoost": {
    "Title": "SpeedBoost",
    "Block": "models/blockmaker/speedboost/block.vmdl",
    "Pole": "models/blockmaker/speedboost/pole.vmdl"
  },
  "Slap": {
    "Title": "Slap",
    "Block": "models/blockmaker/slap/block.vmdl",
    "Pole": "models/blockmaker/slap/pole.vmdl"
  },
  "Random": {
    "Title": "Random",
    "Block": "models/blockmaker/random/block.vmdl",
    "Pole": "models/blockmaker/random/pole.vmdl"
  },
  "Nuke": {
    "Title": "Nuke",
    "Block": "models/blockmaker/nuke/block.vmdl",
    "Pole": "models/blockmaker/nuke/pole.vmdl"
  },
  "Invincibility": {
    "Title": "Invincibility",
    "Block": "models/blockmaker/invincibility/block.vmdl",
    "Pole": "models/blockmaker/invincibility/pole.vmdl"
  },
  "Ice": {
    "Title": "Ice",
    "Block": "models/blockmaker/ice/block.vmdl",
    "Pole": "models/blockmaker/ice/pole.vmdl"
  },
  "Camouflage": {
    "Title": "Camouflage",
    "Block": "models/blockmaker/camouflage/block.vmdl",
    "Pole": "models/blockmaker/camouflage/pole.vmdl"
  },
  "Trampoline": {
    "Title": "Trampoline",
    "Block": "models/blockmaker/trampoline/block.vmdl",
    "Pole": "models/blockmaker/trampoline/pole.vmdl"
  },
  "NoFallDmg": {
    "Title": "NoFallDmg",
    "Block": "models/blockmaker/nofall/block.vmdl",
    "Pole": "models/blockmaker/nofall/pole.vmdl"
  },
  "Honey": {
    "Title": "Honey",
    "Block": "models/blockmaker/honey/block.vmdl",
    "Pole": "models/blockmaker/honey/pole.vmdl"
  },
  "Barrier": {
    "Title": "Barrier",
    "Block": "models/blockmaker/barrier/block.vmdl",
    "Pole": "models/blockmaker/barrier/pole.vmdl"
  },
  "CustomBlocks": []
}
```
</details>
<details> <summary>default_properties.json</summary>
  
```json
{
  "Bhop": {
    "Cooldown": 1,
    "Value": 0,
    "Duration": 0.25,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Health": {
    "Cooldown": 0.75,
    "Value": 1,
    "Duration": 0,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Money": {
    "Cooldown": -1,
    "Value": 3000,
    "Duration": 0,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Grenade": {
    "Cooldown": -1,
    "Value": 0,
    "Duration": 0,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Gravity": {
    "Cooldown": 0,
    "Value": 0.25,
    "Duration": 0,
    "OnTop": false,
    "Locked": false,
    "Builder": ""
  },
  "Frost": {
    "Cooldown": -1,
    "Value": 0,
    "Duration": 0,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Flash": {
    "Cooldown": -1,
    "Value": 0,
    "Duration": 0,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Fire": {
    "Cooldown": 5,
    "Value": 1,
    "Duration": 5,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Delay": {
    "Cooldown": 1.5,
    "Value": 0,
    "Duration": 1,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Damage": {
    "Cooldown": 0.75,
    "Value": 5,
    "Duration": 0,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Stealth": {
    "Cooldown": -1,
    "Value": 0,
    "Duration": 7.5,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Speed": {
    "Cooldown": -1,
    "Value": 2,
    "Duration": 3,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "SpeedBoost": {
    "Cooldown": 0,
    "Value": 650,
    "Duration": 0,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Camouflage": {
    "Cooldown": -1,
    "Value": 0,
    "Duration": 10,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Slap": {
    "Cooldown": 0,
    "Value": 2,
    "Duration": 0,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Random": {
    "Cooldown": -1,
    "Value": 0,
    "Duration": 0,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Invincibility": {
    "Cooldown": -1,
    "Value": 0,
    "Duration": 5,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Trampoline": {
    "Cooldown": 0,
    "Value": 500,
    "Duration": 0,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Death": {
    "Cooldown": 0,
    "Value": 0,
    "Duration": 0,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Honey": {
    "Cooldown": 0,
    "Value": 0.2,
    "Duration": 0,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Platform": {
    "Cooldown": 0,
    "Value": 0,
    "Duration": 0,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "NoFallDmg": {
    "Cooldown": 0,
    "Value": 0,
    "Duration": 0,
    "OnTop": false,
    "Locked": false,
    "Builder": ""
  },
  "Ice": {
    "Cooldown": 0,
    "Value": 0,
    "Duration": 0,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Nuke": {
    "Cooldown": 0,
    "Value": 0,
    "Duration": 0,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Glass": {
    "Cooldown": 0,
    "Value": 0,
    "Duration": 0,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Pistol": {
    "Cooldown": -1,
    "Value": 1,
    "Duration": 0,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Rifle": {
    "Cooldown": -1,
    "Value": 1,
    "Duration": 0,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Sniper": {
    "Cooldown": -1,
    "Value": 1,
    "Duration": 0,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Shotgun/Heavy": {
    "Cooldown": -1,
    "Value": 1,
    "Duration": 0,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "SMG": {
    "Cooldown": -1,
    "Value": 1,
    "Duration": 0,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  },
  "Barrier": {
    "Cooldown": 2,
    "Value": 0,
    "Duration": 0.01,
    "OnTop": true,
    "Locked": false,
    "Builder": ""
  }
}
```
</details>
