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

<img src="https://media.discordapp.net/attachments/1051988905320255509/1146537451750432778/ezgif.com-video-to-gif_2.gif?ex=66a359f6&is=66a20876&hm=768e346857f44792cf5b2917fe55b525522029ecccac95bb765b881baa6660d7&" width="250">

<br>

<a href='https://ko-fi.com/G2G2Y3Z9R' target='_blank'><img style='border:0px; height:75px;' src='https://storage.ko-fi.com/cdn/brandasset/kofi_s_tag_dark.png?_gl=1*6vhavf*_gcl_au*MTIwNjcwMzM4OC4xNzE1NzA0NjM5*_ga*NjE5MjYyMjkzLjE3MTU3MDQ2MTM.*_ga_M13FZ7VQ2C*MTcyMjIwMDA2NS4xNy4xLjE3MjIyMDA0MDUuNjAuMC4w' border='0' alt='Buy Me a Coffee at ko-fi.com' /></a> <br>

## example config

```json
{
  "Settings": {
    "Prefix": "{purple}[BlockMaker]{default}",
    "MenuType": "html",
    "BuildMode": false,
    "BuildModeConfig": false,
    "BlockGrabColor": "255,255,255,128",
    "GridValues": [ 0,8,16,32,64,128,256 ],
    "AutoSave": false,
    "SaveTime": 300
  },
  "Commands": {
    "Enabled": true,
    "Permission": "@css/root",
    "BuildMenu": "blockmenu,blocksmenu,buildmenu",
    "BuildMode": "buildmode,togglebuild",
    "CreateBlock": "create,block,createblock",
    "DeleteBlock": "delete,deleteblock,removeblock",
    "RotateBlock": "rotate,rotateblock",
    "SaveBlocks": "save,saveblocks",
    "ToggleBuilder": "togglebuilder,allowbuilder,allowbuild",
    "ToggleSnapping": "snap,togglesnap"
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