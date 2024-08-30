using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using static BlockMaker.Plugin;

namespace BlockMaker;

public static class MenuWASD
{
    public static void OpenMenu(CCSPlayerController player)
    {
        IWasdMenu MainMenu = WasdManager.CreateMenu("Block Builder");

        MainMenu.Add("Create Block", (player, menuOption) =>
        {
            Instance.Command_CreateBlock(player);
        });

        MainMenu.Add("Delete Block", (player, menuOption) =>
        {
            Instance.Command_DeleteBlock(player);
        });

        MainMenu.Add("Rotate Block", (player, menuOption) =>
        {
            float[] rotateValues = Instance.Config.Settings.Building.RotationValues;
            string[] rotateOptions = { "Reset", "X-", "X+", "Y-", "Y+", "Z-", "Z+" };

            RotateMenuOptions(player, rotateOptions, rotateValues);
        });

        MainMenu.Add($"Block Settings", (player, menuOption) =>
        {
            IWasdMenu BlockMenu = WasdManager.CreateMenu("Block Settings");

            BlockMenu.Add($"Size: " + Instance.playerData[player.Slot].BlockSize, (player, menuOption) =>
            {
                string[] sizeValues = { "Small", "Medium", "Large", "Pole" };

                SizeMenuOptions(player, MainMenu, sizeValues);
            });

            BlockMenu.Add($"Grid: {Instance.playerData[player.Slot].GridValue} Units", (player, menuOption) =>
            {
                float[] gridValues = Instance.Config.Settings.Building.GridValues;

                GridMenuOptions(player, gridValues);
            });

            BlockMenu.Add($"Type: {Instance.playerData[player.Slot].BlockType}", (player, menuOption) =>
            {
                TypeMenuOptions(player);
            });

            WasdManager.OpenMainMenu(player, BlockMenu);
        });

        MainMenu.Add("Settings", (player, menuOption) =>
        {
            SettingsOptions(player);
        });

        WasdManager.OpenMainMenu(player, MainMenu);
    }

    private static void RotateMenuOptions(CCSPlayerController player, string[] rotateOptions, float[] rotateValues)
    {
        IWasdMenu RotateMenu = WasdManager.CreateMenu($"Rotate Block ({Instance.playerData[player.Slot].RotationValue} Units)");

        RotateMenu.Add($"Select Units", (p, option) =>
        {
            RotateValuesMenuOptions(player, rotateOptions, rotateValues);
        });

        foreach (string rotateOption in rotateOptions)
        {
            RotateMenu.Add(rotateOption, (p, option) =>
            {
                Instance.Command_RotateBlock(p, rotateOption);
            });
        }

        WasdManager.OpenMainMenu(player, RotateMenu);
    }

    private static void RotateValuesMenuOptions(CCSPlayerController player, string[] rotateOptions, float[] rotateValues)
    {
        IWasdMenu RotateValuesMenu = WasdManager.CreateMenu($"Rotate Values");

        foreach (float rotateValueOption in rotateValues)
        {
            RotateValuesMenu.Add(rotateValueOption.ToString() + " Units", (p, option) =>
            {
                Instance.playerData[p.Slot].RotationValue = rotateValueOption;

                Instance.PrintToChat(player, $"Selected Rotation Value: {ChatColors.White}{rotateValueOption} Units");

                RotateMenuOptions(player, rotateOptions, rotateValues);
            });
        }

        WasdManager.OpenMainMenu(player, RotateValuesMenu);
    }

    private static void SizeMenuOptions(CCSPlayerController player, IWasdMenu openMainMenu, string[] sizeValues)
    {
        IWasdMenu SizeMenu = WasdManager.CreateMenu($"Select Size ({Instance.playerData[player.Slot].BlockSize})");

        foreach (string sizeValue in sizeValues)
        {
            SizeMenu.Add(sizeValue, (p, option) =>
            {
                Instance.playerData[player.Slot].BlockSize = sizeValue;

                Instance.PrintToChat(p, $"Selected Size: {ChatColors.White}{sizeValue}");

                SizeMenuOptions(player, openMainMenu, sizeValues);
            });
        }

        WasdManager.OpenMainMenu(player, SizeMenu);
    }

    private static void GridMenuOptions(CCSPlayerController player, float[] gridValues)
    {
        IWasdMenu GridMenu = WasdManager.CreateMenu($"Select Grid ({(Instance.playerData[player.Slot].Grid ? "ON" : "OFF")} - {Instance.playerData[player.Slot].GridValue})");

        GridMenu.Add($"Toggle Grid", (p, option) =>
        {
            Instance.Command_Grid(player);

            GridMenuOptions(player, gridValues);
        });

        foreach (float gridValue in gridValues)
        {
            GridMenu.Add(gridValue.ToString() + " Units", (p, option) =>
            {
                Instance.playerData[player.Slot].GridValue = gridValue;

                Instance.PrintToChat(p, $"Selected Grid: {ChatColors.White}{gridValue} Units");

                GridMenuOptions(player, gridValues);
            });
        }

        WasdManager.OpenMainMenu(player, GridMenu);
    }

    private static void TypeMenuOptions(CCSPlayerController player)
    {
        IWasdMenu TypeMenu = WasdManager.CreateMenu($"Select Type ({Instance.playerData[player.Slot].BlockType})");

        foreach (var block in Instance.BlockModels)
        {
            string blockName = block.Key;

            TypeMenu.Add(blockName, (player, menuOption) =>
            {
                Instance.playerData[player.Slot].BlockType = blockName;

                Instance.PrintToChat(player, $"Selected Block: {ChatColors.White}{blockName}");

                TypeMenuOptions(player);
            });
        }
        WasdManager.OpenMainMenu(player, TypeMenu);
    }

    private static void SettingsOptions(CCSPlayerController player)
    {
        IWasdMenu SettingsMenu = WasdManager.CreateMenu("Settings");

        SettingsMenu.Add("Build Mode: " + (Instance.buildMode ? "ON" : "OFF"), (player, menuOption) =>
        {
            Instance.Command_BuildMode(player);

            SettingsOptions(player);
        });

        SettingsMenu.Add("Godmode: " + (Instance.playerData[player.Slot].Godmode ? "ON" : "OFF"), (player, menuOption) =>
        {
            Instance.Command_Godmode(player);

            SettingsOptions(player);
        });

        SettingsMenu.Add("Noclip: " + (Instance.playerData[player.Slot].Noclip ? "ON" : "OFF"), (player, menuOption) =>
        {
            Instance.Command_Noclip(player);

            SettingsOptions(player);
        });

        SettingsMenu.Add("Save Blocks", (player, menuOption) =>
        {
            Instance.Command_SaveBlocks(player);

            SettingsOptions(player);
        });

        WasdManager.OpenMainMenu(player, SettingsMenu);
    }
}