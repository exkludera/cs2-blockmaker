using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using static BlockBuilder.Plugin;
using CounterStrikeSharp.API.Modules.Utils;

namespace BlockBuilder;

public static class MenuWASD
{
    public static void OpenMenu(CCSPlayerController player)
    {
        IWasdMenu MainMenu = WasdManager.CreateMenu("Block Builder");

        MainMenu.Add("place", (player, menuOption) =>
        {
            _.Command_CreateBlock(player);
        });

        MainMenu.Add("delete", (player, menuOption) =>
        {
            _.Command_DeleteBlock(player);
        });

        MainMenu.Add("rotate", (player, menuOption) =>
        {
            string[] rotateOptions = { "reset", "X+", "X-", "Y+", "Y-", "Z+", "Z-" };

            RotateMenuOptions(player, rotateOptions);
        });

        MainMenu.Add("select block", (player, menuOption) =>
        {
            IWasdMenu BlockMenu = WasdManager.CreateMenu("Select Block");

            BlockMenu.Add("size", (player, menuOption) =>
            {
                string[] sizeValues = { "Small", "Medium", "Large", "Pole" };

                SizeMenuOptions(player, BlockMenu, sizeValues);
            });

            BlockMenu.Add("grid", (player, menuOption) =>
            {
                float[] gridValues = { 0.0f, 16.0f, 32.0f, 64.0f, 128.0f, 256.0f, 512.0f };

                GridMenuOptions(player, BlockMenu, gridValues);
            });

            BlockMenu.Add("type", (player, menuOption) =>
            {
                IWasdMenu TypeMenu = WasdManager.CreateMenu("Select Type");

                foreach (var block in _.Config.Blocks)
                {
                    string blockName = block.Key;

                    TypeMenu.Add(blockName, (player, menuOption) =>
                    {
                        _.playerData[player].selectedBlock = blockName;

                        _.PrintToChat(player, $"Selected Block: {ChatColors.White}{blockName}");

                        WasdManager.OpenMainMenu(player, MainMenu);
                    });
                }
                WasdManager.OpenMainMenu(player, TypeMenu);
            });

            WasdManager.OpenMainMenu(player, BlockMenu);
        });

        MainMenu.Add("save blocks", (player, menuOption) =>
        {
            _.Command_SaveBlocks(player);
        });

        WasdManager.OpenMainMenu(player, MainMenu);
    }

    private static void RotateMenuOptions(CCSPlayerController player, string[] rotateOptions)
    {
        IWasdMenu RotateMenu = WasdManager.CreateMenu("Rotate Block");

        RotateMenu.Add("select rotate value", (p, option) =>
        {
            float[] rotateValues = { 10.0f, 30.0f, 45.0f, 60.0f, 90.0f, 120.0f };
            RotateValuesMenuOptions(player, RotateMenu, rotateValues);
        });

        foreach (string rotateOption in rotateOptions)
        {
            RotateMenu.Add(rotateOption, (p, option) =>
            {
                _.Command_RotateBlock(p, rotateOption);

                WasdManager.OpenMainMenu(player, RotateMenu);
            });
        }

        WasdManager.OpenMainMenu(player, RotateMenu);
    }

    private static void RotateValuesMenuOptions(CCSPlayerController player, IWasdMenu RotateMenu, float[] rotateValues)
    {
        IWasdMenu RotateValuesMenu = WasdManager.CreateMenu("Rotate Values");

        foreach (float rotateValueOption in rotateValues)
        {
            RotateValuesMenu.Add(rotateValueOption.ToString(), (p, option) =>
            {
                _.playerData[p].selectedRotation = rotateValueOption;

                _.PrintToChat(player, $"Selected Rotation Value: {ChatColors.White}{rotateValueOption}");

                WasdManager.OpenMainMenu(player, RotateMenu);
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
                _.playerData[player].selectedSize = sizeValue.ToLower();

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
            GridMenu.Add(gridValue.ToString(), (p, option) =>
            {
                _.playerData[player].selectedGrid = gridValue;

                _.PrintToChat(p, $"Selected Grid: {ChatColors.White}{gridValue}");

                WasdManager.OpenMainMenu(player, openMainMenu);
            });
        }

        WasdManager.OpenMainMenu(player, GridMenu);
    }
}