using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using static BlockBuilder.Plugin;
using CounterStrikeSharp.API.Modules.Utils;

namespace BlockBuilder;

public static class MenuChat
{
    public static void OpenMenu(CCSPlayerController player)
    {
        ChatMenu MainMenu = new("Block Builder");

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
            ChatMenu BlockMenu = new("Select Block");

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
                ChatMenu TypeMenu = new("Select Type");

                foreach (var block in _.Config.Blocks)
                {
                    string blockName = block.Key;

                    TypeMenu.AddMenuOption(blockName, (player, menuOption) =>
                    {
                        _.playerData[player].selectedBlock = blockName;

                        _.PrintToChat(player, $"Selected Block: {ChatColors.White}{blockName}");

                        MenuManager.OpenChatMenu(player, MainMenu);
                    });
                }
                MenuManager.OpenChatMenu(player, TypeMenu);
            });

            MenuManager.OpenChatMenu(player, BlockMenu);
        });

        MainMenu.AddMenuOption("save blocks", (player, menuOption) =>
        {
            _.Command_SaveBlocks(player);
        });

        MenuManager.OpenChatMenu(player, MainMenu);
    }

    private static void RotateMenuOptions(CCSPlayerController player, string[] rotateOptions)
    {
        ChatMenu RotateMenu = new("Rotate Block");

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

                MenuManager.OpenChatMenu(player, RotateMenu);
            });
        }

        MenuManager.OpenChatMenu(player, RotateMenu);
    }

    private static void RotateValuesMenuOptions(CCSPlayerController player, ChatMenu RotateMenu, float[] rotateValues)
    {
        ChatMenu RotateValuesMenu = new("Rotate Values");

        foreach (float rotateValueOption in rotateValues)
        {
            RotateValuesMenu.AddMenuOption(rotateValueOption.ToString(), (p, option) =>
            {
                _.playerData[p].selectedRotation = rotateValueOption;

                _.PrintToChat(player, $"Selected Rotation Value: {ChatColors.White}{rotateValueOption}");

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
                _.playerData[player].selectedSize = sizeValue.ToLower();

                _.PrintToChat(p, $"Selected Size: {ChatColors.White}{sizeValue}");

                MenuManager.OpenChatMenu(player, openMainMenu);
            });
        }

        MenuManager.OpenChatMenu(player, SizeMenu);
    }

    private static void GridMenuOptions(CCSPlayerController player, ChatMenu openMainMenu, float[] gridValues)
    {
        ChatMenu GridMenu = new ChatMenu("Select Grid");

        foreach (float gridValue in gridValues)
        {
            GridMenu.AddMenuOption(gridValue.ToString(), (p, option) =>
            {
                _.playerData[player].selectedGrid = gridValue;

                _.PrintToChat(p, $"Selected Grid: {ChatColors.White}{gridValue}");

                MenuManager.OpenChatMenu(player, openMainMenu);
            });
        }

        MenuManager.OpenChatMenu(player, GridMenu);
    }
}