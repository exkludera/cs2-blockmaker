using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using static BlockMaker.Plugin;

namespace BlockMaker;

public static class MenuChat
{
    private static string? BlockType;
    private static string? BlockSize;
    private static bool Grid;
    private static float GridValue;
    private static float RotationValue;

    public static void OpenMenu(CCSPlayerController player)
    {
        ChatMenu MainMenu = new("Block Builder");

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
            string[] rotateOptions = { "Reset", "X-", "X+", "Y-", "Y+", "Z-", "Z+" };

            RotateMenuOptions(player, rotateOptions);
        });

        MainMenu.AddMenuOption($"Block Settings", (player, menuOption) =>
        {
            ChatMenu BlockMenu = new("Block Settings");

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

            BlockMenu.AddMenuOption($"Type: " + BlockType, (player, menuOption) =>
            {
                ChatMenu TypeMenu = new("Select Type");

                foreach (var block in _.BlockModels)
                {
                    string blockName = block.Key;

                    TypeMenu.AddMenuOption(blockName, (player, menuOption) =>
                    {
                        _.playerData[player].BlockType = blockName;

                        _.PrintToChat(player, $"Selected Block: {ChatColors.White}{blockName}");

                        MenuManager.OpenChatMenu(player, MainMenu);
                    });
                }
                MenuManager.OpenChatMenu(player, TypeMenu);
            });

            MenuManager.OpenChatMenu(player, BlockMenu);
        });

        MainMenu.AddMenuOption("Global Settings", (player, menuOption) =>
        {
            ChatMenu SettingsMenu = new("Global Settings");

            SettingsMenu.AddMenuOption("Toggle BuildMode", (player, menuOption) =>
            {
                _.Command_BuildMode(player, false);

                MenuManager.OpenChatMenu(player, MainMenu);
            });

            SettingsMenu.AddMenuOption("Save Blocks", (player, menuOption) =>
            {
                _.Command_SaveBlocks(player, false);

                MenuManager.OpenChatMenu(player, MainMenu);
            });

            MenuManager.OpenChatMenu(player, SettingsMenu);
        });

        MenuManager.OpenChatMenu(player, MainMenu);
    }

    private static void RotateMenuOptions(CCSPlayerController player, string[] rotateOptions)
    {
        ChatMenu RotateMenu = new("Rotate Block");

        RotateMenu.AddMenuOption($"Value: {RotationValue} Units", (p, option) =>
        {
            float[] rotateValues = _.Config.Settings.GridValues;
            RotateValuesMenuOptions(player, RotateMenu, rotateValues);
        });

        foreach (string rotateOption in rotateOptions)
        {
            RotateMenu.AddMenuOption(rotateOption, (p, option) =>
            {
                _.Command_RotateBlock(p, false, rotateOption);
            });
        }

        MenuManager.OpenChatMenu(player, RotateMenu);
    }

    private static void RotateValuesMenuOptions(CCSPlayerController player, ChatMenu RotateMenu, float[] rotateValues)
    {
        ChatMenu RotateValuesMenu = new("Rotate Values");

        foreach (float rotateValueOption in rotateValues)
        {
            RotateValuesMenu.AddMenuOption(rotateValueOption.ToString() + " Units", (p, option) =>
            {
                _.playerData[p].RotationValue = rotateValueOption;

                _.PrintToChat(player, $"Selected Rotation Value: {ChatColors.White}{rotateValueOption} Units");

                MenuManager.OpenChatMenu(player, RotateMenu);
            });
        }

        MenuManager.OpenChatMenu(player, RotateValuesMenu);
    }

    private static void SizeMenuOptions(CCSPlayerController player, ChatMenu openMainMenu, string[] sizeValues)
    {
        ChatMenu SizeMenu = new("Select Size");

        foreach (string sizeValue in sizeValues)
        {
            SizeMenu.AddMenuOption(sizeValue, (p, option) =>
            {
                _.playerData[player].BlockSize = sizeValue;

                _.PrintToChat(p, $"Selected Size: {ChatColors.White}{sizeValue}");

                MenuManager.OpenChatMenu(player, openMainMenu);
            });
        }

        MenuManager.OpenChatMenu(player, SizeMenu);
    }

    private static void GridMenuOptions(CCSPlayerController player, float[] gridValues)
    {
        ChatMenu GridMenu = new("Select Grid");

        GridMenu.AddMenuOption($"Grid: {(Grid ? "Enabled" : "Disabled")}", (p, option) =>
        {
            _.Command_Grid(player, false);
        });

        foreach (float gridValue in gridValues)
        {
            GridMenu.AddMenuOption(gridValue.ToString() + " Units", (p, option) =>
            {
                _.playerData[player].GridValue = gridValue;

                _.PrintToChat(p, $"Selected Grid: {ChatColors.White}{gridValue} Units");
            });
        }

        MenuManager.OpenChatMenu(player, GridMenu);
    }
}