using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using static BlockMaker.Plugin;

namespace BlockMaker;

public static class MenuHTML
{
    public static void OpenMenu(CCSPlayerController player)
    {
        CenterHtmlMenu MainMenu = new("Block Builder", _);

        MainMenu.AddMenuOption("Create Block", (player, menuOption) =>
        {
            _.Command_CreateBlock(player);
        });

        MainMenu.AddMenuOption("Delete Block", (player, menuOption) =>
        {
            _.Command_DeleteBlock(player);
        });

        MainMenu.AddMenuOption("Rotate Block", (player, menuOption) =>
        {
            string[] rotateOptions = { "Reset", "X-", "X+", "Y-", "Y+", "Z-", "Z+" };

            RotateMenuOptions(player, rotateOptions);
        });

        MainMenu.AddMenuOption($"Block Settings", (player, menuOption) =>
        {
            CenterHtmlMenu BlockMenu = new("Block Settings", _);

            BlockMenu.AddMenuOption($"Size: {_.playerData[player].Size}", (player, menuOption) =>
            {
                string[] sizeValues = { "Small", "Medium", "Large", "Pole" };

                SizeMenuOptions(player, MainMenu, sizeValues);
            });

            BlockMenu.AddMenuOption($"Grid: {_.playerData[player].Grid} Units", (player, menuOption) =>
            {
                float[] gridValues = _.Config.Settings.GridValues;

                GridMenuOptions(player, MainMenu, gridValues);
            });

            BlockMenu.AddMenuOption($"Type: {_.playerData[player].Block}", (player, menuOption) =>
            {
                CenterHtmlMenu TypeMenu = new("Select Type", _);

                foreach (var block in _.BlockModels)
                {
                    string blockName = block.Key;

                    TypeMenu.AddMenuOption(blockName, (player, menuOption) =>
                    {
                        _.playerData[player].Block = blockName;

                        _.PrintToChat(player, $"Selected Block: {ChatColors.White}{blockName}");

                        MenuManager.OpenCenterHtmlMenu(_, player, MainMenu);
                    });
                }
                MenuManager.OpenCenterHtmlMenu(_, player, TypeMenu);
            });

            MenuManager.OpenCenterHtmlMenu(_, player, BlockMenu);
        });

        MainMenu.AddMenuOption("Global Settings", (player, menuOption) =>
        {
            CenterHtmlMenu SettingsMenu = new("Global Settings", _);

            SettingsMenu.AddMenuOption("Toggle BuildMode", (player, menuOption) =>
            {
                _.Command_ToggleBuildMode(player);

                MenuManager.OpenCenterHtmlMenu(_, player, MainMenu);
            });

            SettingsMenu.AddMenuOption("Save Blocks", (player, menuOption) =>
            {
                _.Command_SaveBlocks(player);

                MenuManager.OpenCenterHtmlMenu(_, player, MainMenu);
            });

            MenuManager.OpenCenterHtmlMenu(_, player, SettingsMenu);
        });

        MenuManager.OpenCenterHtmlMenu(_, player, MainMenu);
    }

    private static void RotateMenuOptions(CCSPlayerController player, string[] rotateOptions)
    {
        CenterHtmlMenu RotateMenu = new("Rotate Block", _);

        RotateMenu.AddMenuOption($"Value: {_.playerData[player].Rotation} Units", (p, option) =>
        {
            float[] rotateValues = { 10.0f, 30.0f, 45.0f, 60.0f, 90.0f, 120.0f };
            RotateValuesMenuOptions(player, RotateMenu, rotateValues);
        });

        foreach (string rotateOption in rotateOptions)
        {
            RotateMenu.AddMenuOption(rotateOption, (p, option) =>
            {
                _.Command_RotateBlock(p, rotateOption);
            });
        }

        MenuManager.OpenCenterHtmlMenu(_, player, RotateMenu);
    }

    private static void RotateValuesMenuOptions(CCSPlayerController player, CenterHtmlMenu RotateMenu, float[] rotateValues)
    {
        CenterHtmlMenu RotateValuesMenu = new("Rotate Values", _);

        foreach (float rotateValueOption in rotateValues)
        {
            RotateValuesMenu.AddMenuOption(rotateValueOption.ToString() + " Units", (p, option) =>
            {
                _.playerData[p].Rotation = rotateValueOption;

                _.PrintToChat(player, $"Selected Rotation Value: {ChatColors.White}{rotateValueOption} Units");
            });
        }

        MenuManager.OpenCenterHtmlMenu(_, player, RotateValuesMenu);
    }

    private static void SizeMenuOptions(CCSPlayerController player, CenterHtmlMenu openMainMenu, string[] sizeValues)
    {
        CenterHtmlMenu SizeMenu = new("Select Size", _);

        foreach (string sizeValue in sizeValues)
        {
            SizeMenu.AddMenuOption(sizeValue, (p, option) =>
            {
                _.playerData[player].Size = sizeValue;

                _.PrintToChat(p, $"Selected Size: {ChatColors.White}{sizeValue}");

                MenuManager.OpenCenterHtmlMenu(_, player, openMainMenu);
            });
        }

        MenuManager.OpenCenterHtmlMenu(_, player, SizeMenu);
    }

    private static void GridMenuOptions(CCSPlayerController player, CenterHtmlMenu openMainMenu, float[] gridValues)
    {
        CenterHtmlMenu GridMenu = new("Select Grid", _);

        foreach (float gridValue in gridValues)
        {
            GridMenu.AddMenuOption(gridValue.ToString() + " Units", (p, option) =>
            {
                _.playerData[player].Grid = gridValue;

                _.PrintToChat(p, $"Selected Grid: {ChatColors.White}{gridValue} Units");

                MenuManager.OpenCenterHtmlMenu(_, player, openMainMenu);
            });
        }

        MenuManager.OpenCenterHtmlMenu(_, player, GridMenu);
    }
}