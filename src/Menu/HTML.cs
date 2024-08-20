using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using static BlockMaker.Plugin;

namespace BlockMaker;

public static class MenuHTML
{
    private static string? BlockType;
    private static string? BlockSize;
    private static bool Grid;
    private static float GridValue;
    private static float RotationValue;

    public static void OpenMenu(CCSPlayerController player)
    {
        CenterHtmlMenu MainMenu = new("Block Builder", _);

        BlockType = _.playerData[player].BlockType;
        BlockSize = _.playerData[player].BlockSize;
        Grid = _.playerData[player].Grid;
        GridValue = _.playerData[player].GridValue;
        RotationValue = _.playerData[player].RotationValue;

        MainMenu.AddMenuOption("Create Block", (player, menuOption) =>
        {
            _.Command_CreateBlock(player, false);
        });

        MainMenu.AddMenuOption("Delete Block", (player, menuOption) =>
        {
            _.Command_DeleteBlock(player, false);
        });

        MainMenu.AddMenuOption("Rotate Block", (player, menuOption) =>
        {
            float[] rotateValues = _.Config.Settings.RotationValues;
            string[] rotateOptions = { "Reset", "X-", "X+", "Y-", "Y+", "Z-", "Z+" };

            RotateMenuOptions(player, rotateOptions, rotateValues);
        });

        MainMenu.AddMenuOption($"Block Settings", (player, menuOption) =>
        {
            CenterHtmlMenu BlockMenu = new("Block Settings", _);

            BlockMenu.AddMenuOption($"Size: " + BlockSize, (player, menuOption) =>
            {
                string[] sizeValues = { "Small", "Medium", "Large", "Pole" };

                SizeMenuOptions(player, MainMenu, sizeValues);
            });

            BlockMenu.AddMenuOption($"Grid: {GridValue} Units", (player, menuOption) =>
            {
                float[] gridValues = _.Config.Settings.GridValues;

                GridMenuOptions(player, gridValues);
            });

            BlockMenu.AddMenuOption($"Type: {BlockType}", (player, menuOption) =>
            {
                TypeMenuOptions(player);
            });

            MenuManager.OpenCenterHtmlMenu(_, player, BlockMenu);
        });

        MainMenu.AddMenuOption("Global Settings", (player, menuOption) =>
        {
            CenterHtmlMenu SettingsMenu = new("Global Settings", _);

            SettingsMenu.AddMenuOption("Toggle BuildMode", (player, menuOption) =>
            {
                _.Command_BuildMode(player, false);

                MenuManager.OpenCenterHtmlMenu(_, player, MainMenu);
            });

            SettingsMenu.AddMenuOption("Save Blocks", (player, menuOption) =>
            {
                _.Command_SaveBlocks(player, false);

                MenuManager.OpenCenterHtmlMenu(_, player, MainMenu);
            });

            MenuManager.OpenCenterHtmlMenu(_, player, SettingsMenu);
        });

        MenuManager.OpenCenterHtmlMenu(_, player, MainMenu);
    }

    private static void RotateMenuOptions(CCSPlayerController player, string[] rotateOptions, float[] rotateValues)
    {
        RotationValue = _.playerData[player].RotationValue;

        CenterHtmlMenu RotateMenu = new($"Rotate Block ({RotationValue} Units)", _);

        RotateMenu.AddMenuOption($"Select Units", (p, option) =>
        {
            RotateValuesMenuOptions(player, rotateOptions, rotateValues);
        });

        foreach (string rotateOption in rotateOptions)
        {
            RotateMenu.AddMenuOption(rotateOption, (p, option) =>
            {
                _.Command_RotateBlock(p, false, rotateOption);
            });
        }

        MenuManager.OpenCenterHtmlMenu(_, player, RotateMenu);
    }

    private static void RotateValuesMenuOptions(CCSPlayerController player, string[] rotateOptions, float[] rotateValues)
    {
        CenterHtmlMenu RotateValuesMenu = new($"Rotate Values", _);

        foreach (float rotateValueOption in rotateValues)
        {
            RotateValuesMenu.AddMenuOption(rotateValueOption.ToString() + " Units", (p, option) =>
            {
                _.playerData[p].RotationValue = rotateValueOption;

                _.PrintToChat(player, $"Selected Rotation Value: {ChatColors.White}{rotateValueOption} Units");

                MenuManager.CloseActiveMenu(player);
                RotateMenuOptions(player, rotateOptions, rotateValues);
            });
        }

        MenuManager.OpenCenterHtmlMenu(_, player, RotateValuesMenu);
    }

    private static void SizeMenuOptions(CCSPlayerController player, CenterHtmlMenu openMainMenu, string[] sizeValues)
    {
        BlockSize = _.playerData[player].BlockSize;

        CenterHtmlMenu SizeMenu = new($"Select Size ({BlockSize})", _);

        foreach (string sizeValue in sizeValues)
        {
            SizeMenu.AddMenuOption(sizeValue, (p, option) =>
            {
                _.playerData[player].BlockSize = sizeValue;

                _.PrintToChat(p, $"Selected Size: {ChatColors.White}{sizeValue}");

                MenuManager.CloseActiveMenu(player);
                SizeMenuOptions(player, openMainMenu, sizeValues);
            });
        }

        MenuManager.OpenCenterHtmlMenu(_, player, SizeMenu);
    }

    private static void GridMenuOptions(CCSPlayerController player, float[] gridValues)
    {
        Grid = _.playerData[player].Grid;
        GridValue = _.playerData[player].GridValue;

        CenterHtmlMenu GridMenu = new($"Select Grid ({(Grid ? "Enabled" : "Disabled")} - {GridValue})", _);

        GridMenu.AddMenuOption($"Toggle Grid", (p, option) =>
        {
            _.Command_Grid(player, false);

            MenuManager.CloseActiveMenu(player);
            GridMenuOptions(player, gridValues);
        });

        foreach (float gridValue in gridValues)
        {
            GridMenu.AddMenuOption(gridValue.ToString() + " Units", (p, option) =>
            {
                _.playerData[player].GridValue = gridValue;

                _.PrintToChat(p, $"Selected Grid: {ChatColors.White}{gridValue} Units");

                MenuManager.CloseActiveMenu(player);
                GridMenuOptions(player, gridValues);
            });
        }

        MenuManager.OpenCenterHtmlMenu(_, player, GridMenu);
    }

    private static void TypeMenuOptions(CCSPlayerController player)
    {
        BlockType = _.playerData[player].BlockType;
        CenterHtmlMenu TypeMenu = new($"Select Type ({BlockType})", _);

        foreach (var block in _.BlockModels)
        {
            string blockName = block.Key;

            TypeMenu.AddMenuOption(blockName, (player, menuOption) =>
            {
                _.playerData[player].BlockType = blockName;

                _.PrintToChat(player, $"Selected Block: {ChatColors.White}{blockName}");

                MenuManager.CloseActiveMenu(player);
                TypeMenuOptions(player);
            });
        }
        MenuManager.OpenCenterHtmlMenu(_, player, TypeMenu);
    }
}