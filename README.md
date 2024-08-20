# cs2-blockmaker

**BlockMaker plugin to create and save blocks, mostly for HNS**

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
- [ipsvn/ChaseMod](https://github.com/ipsvn/ChaseMod) (optional for gameplay)

<br>

> [!NOTE]
> thanks to [UgurhanK/BaseBuilder](https://github.com/UgurhanK/BaseBuilder) for the code base
>
> inspired by [BlockBuilder by x3ro](https://forums.alliedmods.net/showthread.php?t=258329) (block models are from them also)

<img src="https://github.com/user-attachments/assets/53e486cc-8da4-45ab-bc6e-eb38145aba36" height="200px"> <br>

<br>

## example config

```json
{
  "Settings": {
    "Prefix": "{purple}[BlockMaker]{default}",
    "MenuType": "html",
    "BuildMode": true,
    "BuildModeConfig": true,
    "BlockGrabColor": "255,255,255,128",
    "GridValues": [ 16, 32, 64, 128, 256 ],
    "RotationValues": [ 15, 30, 45, 60, 90, 120 ],
    "AutoSave": false,
    "SaveTime": 300
  },
  "AdminCommands": {
    "Permission": "@css/root",
    "BuildMode": "buildmode,togglebuild",
    "ManageBuilder": "builder,togglebuilder,allowbuilder"
  },
  "BuildCommands": {
    "BuildMenu": "buildmenu,blockmenu,blocksmenu",
    "CreateBlock": "create,block,createblock",
    "DeleteBlock": "delete,deleteblock,remove,removeblock",
    "RotateBlock": "rotate,rotateblock",
    "SaveBlocks": "save,saveblocks,saveblock",
    "Snapping": "snap,togglesnap,snapping",
    "Grid": "grid,togglegrid"
  },
  "Sounds": {
    "Enabled": true,
    "Create": "sounds/buttons/blip1.vsnd",
    "Delete": "sounds/buttons/blip2.vsnd",
    "Place": "sounds/buttons/latchunlocked2.vsnd",
    "Rotate": "sounds/buttons/button9.vsnd",
    "Save": "sounds/buttons/bell1.vsnd"
  }
}
```

<br> <a href="https://ko-fi.com/exkludera" target="blank"><img src="https://cdn.ko-fi.com/cdn/kofi5.png" height="48px" alt="Buy Me a Coffee at ko-fi.com"></a>