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
            _.Command_CreateBlock(player);
        });

        MainMenu.Add("Delete Block", (player, menuOption) =>
        {
            _.Command_DeleteBlock(player);
        });

        MainMenu.Add("Rotate Block", (player, menuOption) =>
        {
            string[] rotateOptions = { "Reset", "X-", "X+", "Y-", "Y+", "Z-", "Z+" };

            RotateMenuOptions(player, rotateOptions);
        });

        MainMenu.Add($"Block Settings", (player, menuOption) =>
        {
            IWasdMenu BlockMenu = WasdManager.CreateMenu("Block Settings");

            BlockMenu.Add($"Size: {_.playerData[player].Size}", (player, menuOption) =>
            {
                string[] sizeValues = { "Small", "Medium", "Large", "Pole" };

                SizeMenuOptions(player, MainMenu, sizeValues);
            });

            BlockMenu.Add($"Grid: {_.playerData[player].Grid} Units", (player, menuOption) =>
            {
                float[] gridValues = _.Config.Settings.GridValues;

                GridMenuOptions(player, MainMenu, gridValues);
            });

            BlockMenu.Add($"Type: {_.playerData[player].Block}", (player, menuOption) =>
            {
                IWasdMenu TypeMenu = WasdManager.CreateMenu("Select Type");

                foreach (var block in _.BlockModels)
                {
                    string blockName = block.Key;

                    TypeMenu.Add(blockName, (player, menuOption) =>
                    {
                        _.playerData[player].Block = blockName;

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
                _.Command_ToggleBuildMode(player);

                WasdManager.OpenMainMenu(player, MainMenu);
            });

            SettingsMenu.Add("Save Blocks", (player, menuOption) =>
            {
                _.Command_SaveBlocks(player);

                WasdManager.OpenMainMenu(player, MainMenu);
            });

            WasdManager.OpenMainMenu(player, SettingsMenu);
        });

        WasdManager.OpenMainMenu(player, MainMenu);
    }

    private static void RotateMenuOptions(CCSPlayerController player, string[] rotateOptions)
    {
        IWasdMenu RotateMenu = WasdManager.CreateMenu("Rotate Block");

        RotateMenu.Add($"Value: {_.playerData[player].Rotation} Units", (p, option) =>
        {
            float[] rotateValues = { 10.0f, 30.0f, 45.0f, 60.0f, 90.0f, 120.0f };
            RotateValuesMenuOptions(player, RotateMenu, rotateValues);
        });

        foreach (string rotateOption in rotateOptions)
        {
            RotateMenu.Add(rotateOption, (p, option) =>
            {
                _.Command_RotateBlock(p, rotateOption);
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
                _.playerData[p].Rotation = rotateValueOption;

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
                _.playerData[player].Size = sizeValue;

                _.PrintToChat(p, $"Selected Size: {ChatColors.White}{sizeValue}");

                WasdManager.OpenMainMenu(player, openMainMenu);
            });
        }

        WasdManager.OpenMainMenu(player, SizeMenu);
    }

    private static void GridMenuOptions(CCSPlayerController player, IWasdMenu openMainMenu, float[] gridValues)
    {
        IWasdMenu GridMenu = WasdManager.CreateMenu("Select Grid");

        foreach (float gridValue in gridValues)
        {
            GridMenu.Add(gridValue.ToString() + " Units", (p, option) =>
            {
                _.playerData[player].Grid = gridValue;

                _.PrintToChat(p, $"Selected Grid: {ChatColors.White}{gridValue} Units");

                WasdManager.OpenMainMenu(player, openMainMenu);
            });
        }

        WasdManager.OpenMainMenu(player, GridMenu);
    }
}