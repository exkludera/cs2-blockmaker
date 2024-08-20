using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using static BlockMaker.Plugin;

namespace BlockMaker;

public static class MenuWASD
{
    private static string? BlockType;
    private static string? BlockSize;
    private static bool Grid;
    private static float GridValue;
    private static float RotationValue;

    public static void OpenMenu(CCSPlayerController player)
    {
        IWasdMenu MainMenu = WasdManager.CreateMenu("Block Builder");

        BlockType = _.playerData[player].BlockType;
        BlockSize = _.playerData[player].BlockSize;
        Grid = _.playerData[player].Grid;
        GridValue = _.playerData[player].GridValue;
        RotationValue = _.playerData[player].RotationValue;

        MainMenu.Add("Create Block", (player, menuOption) =>
        {
            _.Command_CreateBlock(player, false);
        });

        MainMenu.Add("Delete Block", (player, menuOption) =>
        {
            _.Command_DeleteBlock(player, false);
        });

        MainMenu.Add("Rotate Block", (player, menuOption) =>
        {
            string[] rotateOptions = { "Reset", "X-", "X+", "Y-", "Y+", "Z-", "Z+" };

            RotateMenuOptions(player, rotateOptions);
        });

        MainMenu.Add($"Block Settings", (player, menuOption) =>
        {
            IWasdMenu BlockMenu = WasdManager.CreateMenu("Block Settings");

            BlockMenu.Add($"Size: " + BlockSize, (player, menuOption) =>
            {
                string[] sizeValues = { "Small", "Medium", "Large", "Pole" };

                SizeMenuOptions(player, MainMenu, sizeValues);
            });

            BlockMenu.Add($"Grid: {GridValue} Units", (player, menuOption) =>
            {
                float[] gridValues = _.Config.Settings.GridValues;

                GridMenuOptions(player, gridValues);
            });

            BlockMenu.Add($"Type: " + BlockType, (player, menuOption) =>
            {
                IWasdMenu TypeMenu = WasdManager.CreateMenu("Select Type");

                foreach (var block in _.BlockModels)
                {
                    string blockName = block.Key;

                    TypeMenu.Add(blockName, (player, menuOption) =>
                    {
                        _.playerData[player].BlockType = blockName;

                        _.PrintToChat(player, $"Selected Block: {ChatColors.White}{blockName}");

                        WasdManager.OpenMainMenu(player, MainMenu);
                    });
                }
                WasdManager.OpenMainMenu(player, TypeMenu);
            });

            WasdManager.OpenMainMenu(player, BlockMenu);
        });

        MainMenu.Add("Global Settings", (player, menuOption) =>
        {
            IWasdMenu SettingsMenu = WasdManager.CreateMenu("Global Settings");

            SettingsMenu.Add("Toggle BuildMode", (player, menuOption) =>
            {
                _.Command_BuildMode(player, false);

                WasdManager.OpenMainMenu(player, MainMenu);
            });

            SettingsMenu.Add("Save Blocks", (player, menuOption) =>
            {
                _.Command_SaveBlocks(player, false);

                WasdManager.OpenMainMenu(player, MainMenu);
            });

            WasdManager.OpenMainMenu(player, SettingsMenu);
        });

        WasdManager.OpenMainMenu(player, MainMenu);
    }

    private static void RotateMenuOptions(CCSPlayerController player, string[] rotateOptions)
    {
        IWasdMenu RotateMenu = WasdManager.CreateMenu("Rotate Block");

        RotateMenu.Add($"Value: {RotationValue} Units", (p, option) =>
        {
            float[] rotateValues = _.Config.Settings.RotationValues;
            RotateValuesMenuOptions(player, RotateMenu, rotateValues);
        });

        foreach (string rotateOption in rotateOptions)
        {
            RotateMenu.Add(rotateOption, (p, option) =>
            {
                _.Command_RotateBlock(p, false, rotateOption);
            });
        }

        WasdManager.OpenMainMenu(player, RotateMenu);
    }

    private static void RotateValuesMenuOptions(CCSPlayerController player, IWasdMenu RotateMenu, float[] rotateValues)
    {
        IWasdMenu RotateValuesMenu = WasdManager.CreateMenu("Rotate Values");

        foreach (float rotateValueOption in rotateValues)
        {
            RotateValuesMenu.Add(rotateValueOption.ToString() + " Units", (p, option) =>
            {
                _.playerData[p].RotationValue = rotateValueOption;

                _.PrintToChat(player, $"Selected Rotation Value: {ChatColors.White}{rotateValueOption} Units");
            });
        }

        WasdManager.OpenMainMenu(player, RotateValuesMenu);
    }

    private static void SizeMenuOptions(CCSPlayerController player, IWasdMenu openMainMenu, string[] sizeValues)
    {
        IWasdMenu SizeMenu = WasdManager.CreateMenu("Select Size");

        foreach (string sizeValue in sizeValues)
        {
            SizeMenu.Add(sizeValue, (p, option) =>
            {
                _.playerData[player].BlockSize = sizeValue;

                _.PrintToChat(p, $"Selected Size: {ChatColors.White}{sizeValue}");

                WasdManager.OpenMainMenu(player, openMainMenu);
            });
        }

        WasdManager.OpenMainMenu(player, SizeMenu);
    }

    private static void GridMenuOptions(CCSPlayerController player, float[] gridValues)
    {
        IWasdMenu GridMenu = WasdManager.CreateMenu("Select Grid");

        GridMenu.Add($"Grid: {(Grid ? "Enabled" : "Disabled")}", (p, option) =>
        {
            _.Command_Grid(player, false);
        });

        foreach (float gridValue in gridValues)
        {
            GridMenu.Add(gridValue.ToString() + " Units", (p, option) =>
            {
                _.playerData[player].GridValue = gridValue;

                _.PrintToChat(p, $"Selected Grid: {ChatColors.White}{gridValue} Units");
            });
        }

        WasdManager.OpenMainMenu(player, GridMenu);
    }
}