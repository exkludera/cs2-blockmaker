using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using static BlockBuilder.Plugin;
using CounterStrikeSharp.API.Modules.Utils;

namespace BlockBuilder;

public static class MenuHTML
{
    public static void OpenMenu(CCSPlayerController player)
    {
        CenterHtmlMenu MainMenu = new("Block Builder", _);

        MainMenu.AddMenuOption("place", (player, menuOption) =>
        {
            _.Command_CreateBlock(player);
        });

        MainMenu.AddMenuOption("delete", (player, menuOption) =>
        {
            _.Command_DeleteBlock(player);
        });

        MainMenu.AddMenuOption("rotate", (player, menuOption) =>
        {
            string[] rotateOptions = { "reset", "X+", "X-", "Y+", "Y-", "Z+", "Z-" };

            RotateMenuOptions(player, rotateOptions);
        });

        MainMenu.AddMenuOption("select block", (player, menuOption) =>
        {
            CenterHtmlMenu BlockMenu = new("Select Block", _);

            BlockMenu.AddMenuOption("size", (player, menuOption) =>
            {
                string[] sizeValues = { "Small", "Medium", "Large", "Pole" };

                SizeMenuOptions(player, BlockMenu, sizeValues);
            });

            BlockMenu.AddMenuOption("grid", (player, menuOption) =>
            {
                float[] gridValues = { 0.0f, 16.0f, 32.0f, 64.0f, 128.0f, 256.0f, 512.0f };

                GridMenuOptions(player, BlockMenu, gridValues);
            });

            BlockMenu.AddMenuOption("type", (player, menuOption) =>
            {
                CenterHtmlMenu TypeMenu = new("Select Type", _);

                foreach (var block in _.Config.Blocks)
                {
                    string blockName = block.Key;

                    TypeMenu.AddMenuOption(blockName, (player, menuOption) =>
                    {
                        _.playerData[player].selectedBlock = blockName;

                        _.PrintToChat(player, $"Selected Block: {ChatColors.White}{blockName}");

                        MenuManager.OpenCenterHtmlMenu(_, player, MainMenu);
                    });
                }
                MenuManager.OpenCenterHtmlMenu(_, player, TypeMenu);
            });

            MenuManager.OpenCenterHtmlMenu(_, player, BlockMenu);
        });

        MainMenu.AddMenuOption("save blocks", (player, menuOption) =>
        {
            _.Command_SaveBlocks(player);
        });

        MenuManager.OpenCenterHtmlMenu(_, player, MainMenu);
    }

    private static void RotateMenuOptions(CCSPlayerController player, string[] rotateOptions)
    {
        CenterHtmlMenu RotateMenu = new("Rotate Block", _);

        RotateMenu.AddMenuOption("select rotate value", (p, option) =>
        {
            float[] rotateValues = { 10.0f, 30.0f, 45.0f, 60.0f, 90.0f, 120.0f };
            RotateValuesMenuOptions(player, RotateMenu, rotateValues);
        });

        foreach (string rotateOption in rotateOptions)
        {
            RotateMenu.AddMenuOption(rotateOption, (p, option) =>
            {
                _.Command_RotateBlock(p, rotateOption);

                MenuManager.OpenCenterHtmlMenu(_, player, RotateMenu);
            });
        }

        MenuManager.OpenCenterHtmlMenu(_, player, RotateMenu);
    }

    private static void RotateValuesMenuOptions(CCSPlayerController player, CenterHtmlMenu RotateMenu, float[] rotateValues)
    {
        CenterHtmlMenu RotateValuesMenu = new("Rotate Values", _);

        foreach (float rotateValueOption in rotateValues)
        {
            RotateValuesMenu.AddMenuOption(rotateValueOption.ToString(), (p, option) =>
            {
                _.playerData[p].selectedRotation = rotateValueOption;

                _.PrintToChat(player, $"Selected Rotation Value: {ChatColors.White}{rotateValueOption}");

                MenuManager.OpenCenterHtmlMenu(_, player, RotateMenu);
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
                _.playerData[player].selectedSize = sizeValue.ToLower();

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
            GridMenu.AddMenuOption(gridValue.ToString(), (p, option) =>
            {
                _.playerData[player].selectedGrid = gridValue;

                _.PrintToChat(p, $"Selected Grid: {ChatColors.White}{gridValue}");

                MenuManager.OpenCenterHtmlMenu(_, player, openMainMenu);
            });
        }

        MenuManager.OpenCenterHtmlMenu(_, player, GridMenu);
    }
}