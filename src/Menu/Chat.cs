using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using static BlockMaker.Plugin;

namespace BlockMaker;

public static class MenuChat
{
    public static void OpenMenu(CCSPlayerController player)
    {
        ChatMenu MainMenu = new("Block Builder");

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
            ChatMenu BlockMenu = new("Block Settings");

            BlockMenu.AddMenuOption($"Size: {_.playerData[player].Size}", (player, menuOption) =>
            {
                string[] sizeValues = { "Small", "Medium", "Large", "Pole" };

                SizeMenuOptions(player, MainMenu, sizeValues);
            });

            BlockMenu.AddMenuOption($"Grid: {_.playerData[player].Grid} Units", (player, menuOption) =>
            {
                float[] gridValues = { 0.0f, 16.0f, 32.0f, 64.0f, 128.0f, 256.0f, 512.0f };

                GridMenuOptions(player, MainMenu, gridValues);
            });

            BlockMenu.AddMenuOption($"Type: {_.playerData[player].Block}", (player, menuOption) =>
            {
                ChatMenu TypeMenu = new("Select Type");

                foreach (var block in _.BlockModels)
                {
                    string blockName = block.Key;

                    TypeMenu.AddMenuOption(blockName, (player, menuOption) =>
                    {
                        _.playerData[player].Block = blockName;

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
                _.Command_ToggleBuildMode(player);

                MenuManager.OpenChatMenu(player, MainMenu);
            });

            SettingsMenu.AddMenuOption("Save Blocks", (player, menuOption) =>
            {
                _.Command_SaveBlocks(player);

                MenuManager.OpenChatMenu(player, MainMenu);
            });

            MenuManager.OpenChatMenu(player, SettingsMenu);
        });

        MenuManager.OpenChatMenu(player, MainMenu);
    }

    private static void RotateMenuOptions(CCSPlayerController player, string[] rotateOptions)
    {
        ChatMenu RotateMenu = new("Rotate Block");

        RotateMenu.AddMenuOption($"Value: {_.playerData[player].Rotation} Units", (p, option) =>
        {
            float[] rotateValues = _.Config.Settings.GridValues;
            RotateValuesMenuOptions(player, RotateMenu, rotateValues);
        });

        foreach (string rotateOption in rotateOptions)
        {
            RotateMenu.AddMenuOption(rotateOption, (p, option) =>
            {
                _.Command_RotateBlock(p, rotateOption);
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
                _.playerData[p].Rotation = rotateValueOption;

                _.PrintToChat(player, $"Selected Rotation Value: {ChatColors.White}{rotateValueOption} Units");
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
                _.playerData[player].Size = sizeValue;

                _.PrintToChat(p, $"Selected Size: {ChatColors.White}{sizeValue}");

                MenuManager.OpenChatMenu(player, openMainMenu);
            });
        }

        MenuManager.OpenChatMenu(player, SizeMenu);
    }

    private static void GridMenuOptions(CCSPlayerController player, ChatMenu openMainMenu, float[] gridValues)
    {
        ChatMenu GridMenu = new("Select Grid");

        foreach (float gridValue in gridValues)
        {
            GridMenu.AddMenuOption(gridValue.ToString() + " Units", (p, option) =>
            {
                _.playerData[player].Grid = gridValue;

                _.PrintToChat(p, $"Selected Grid: {ChatColors.White}{gridValue} Units");

                MenuManager.OpenChatMenu(player, openMainMenu);
            });
        }

        MenuManager.OpenChatMenu(player, GridMenu);
    }
}