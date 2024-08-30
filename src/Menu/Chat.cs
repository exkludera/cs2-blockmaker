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
            Instance.Command_CreateBlock(player);
        });

        MainMenu.AddMenuOption("Delete Block", (player, menuOption) =>
        {
            Instance.Command_DeleteBlock(player);
        });

        MainMenu.AddMenuOption("Rotate Block", (player, menuOption) =>
        {
            float[] rotateValues = Instance.Config.Settings.Building.RotationValues;
            string[] rotateOptions = { "Reset", "X-", "X+", "Y-", "Y+", "Z-", "Z+" };

            RotateMenuOptions(player, rotateOptions, rotateValues);
        });

        MainMenu.AddMenuOption($"Block Settings", (player, menuOption) =>
        {
            ChatMenu BlockMenu = new("Block Settings");

            BlockMenu.AddMenuOption($"Size: " + Instance.playerData[player.Slot].BlockSize, (player, menuOption) =>
            {
                string[] sizeValues = { "Small", "Medium", "Large", "Pole" };

                SizeMenuOptions(player, MainMenu, sizeValues);
            });

            BlockMenu.AddMenuOption($"Grid: {Instance.playerData[player.Slot].GridValue} Units", (player, menuOption) =>
            {
                float[] gridValues = Instance.Config.Settings.Building.GridValues;

                GridMenuOptions(player, gridValues);
            });

            BlockMenu.AddMenuOption($"Type: {Instance.playerData[player.Slot].BlockType}", (player, menuOption) =>
            {
                TypeMenuOptions(player);
            });

            MenuManager.OpenChatMenu(player, BlockMenu);
        });

        MainMenu.AddMenuOption("Settings", (player, menuOption) =>
        {
            SettingsOptions(player);
        });

        MenuManager.OpenChatMenu(player, MainMenu);
    }

    private static void RotateMenuOptions(CCSPlayerController player, string[] rotateOptions, float[] rotateValues)
    {
        ChatMenu RotateMenu = new($"Rotate Block ({Instance.playerData[player.Slot].RotationValue} Units)");

        RotateMenu.AddMenuOption($"Select Units", (p, option) =>
        {
            RotateValuesMenuOptions(player, rotateOptions, rotateValues);
        });

        foreach (string rotateOption in rotateOptions)
        {
            RotateMenu.AddMenuOption(rotateOption, (p, option) =>
            {
                Instance.Command_RotateBlock(p, rotateOption);
            });
        }

        MenuManager.OpenChatMenu(player, RotateMenu);
    }

    private static void RotateValuesMenuOptions(CCSPlayerController player, string[] rotateOptions, float[] rotateValues)
    {
        ChatMenu RotateValuesMenu = new($"Rotate Values");

        foreach (float rotateValueOption in rotateValues)
        {
            RotateValuesMenu.AddMenuOption(rotateValueOption.ToString() + " Units", (p, option) =>
            {
                Instance.playerData[p.Slot].RotationValue = rotateValueOption;

                Instance.PrintToChat(player, $"Selected Rotation Value: {ChatColors.White}{rotateValueOption} Units");

                RotateMenuOptions(player, rotateOptions, rotateValues);
            });
        }

        MenuManager.OpenChatMenu(player, RotateValuesMenu);
    }

    private static void SizeMenuOptions(CCSPlayerController player, ChatMenu openMainMenu, string[] sizeValues)
    {
        ChatMenu SizeMenu = new($"Select Size ({Instance.playerData[player.Slot].BlockSize})");

        foreach (string sizeValue in sizeValues)
        {
            SizeMenu.AddMenuOption(sizeValue, (p, option) =>
            {
                Instance.playerData[player.Slot].BlockSize = sizeValue;

                Instance.PrintToChat(p, $"Selected Size: {ChatColors.White}{sizeValue}");

                SizeMenuOptions(player, openMainMenu, sizeValues);
            });
        }

        MenuManager.OpenChatMenu(player, SizeMenu);
    }

    private static void GridMenuOptions(CCSPlayerController player, float[] gridValues)
    {
        ChatMenu GridMenu = new($"Select Grid ({(Instance.playerData[player.Slot].Grid ? "ON" : "OFF")} - {Instance.playerData[player.Slot].GridValue})");

        GridMenu.AddMenuOption($"Toggle Grid", (p, option) =>
        {
            Instance.Command_Grid(player);

            GridMenuOptions(player, gridValues);
        });

        foreach (float gridValue in gridValues)
        {
            GridMenu.AddMenuOption(gridValue.ToString() + " Units", (p, option) =>
            {
                Instance.playerData[player.Slot].GridValue = gridValue;

                Instance.PrintToChat(p, $"Selected Grid: {ChatColors.White}{gridValue} Units");

                GridMenuOptions(player, gridValues);
            });
        }

        MenuManager.OpenChatMenu(player, GridMenu);
    }

    private static void TypeMenuOptions(CCSPlayerController player)
    {
        ChatMenu TypeMenu = new($"Select Type ({Instance.playerData[player.Slot].BlockType})");

        foreach (var block in Instance.BlockModels)
        {
            string blockName = block.Key;

            TypeMenu.AddMenuOption(blockName, (player, menuOption) =>
            {
                Instance.playerData[player.Slot].BlockType = blockName;

                Instance.PrintToChat(player, $"Selected Block: {ChatColors.White}{blockName}");

                TypeMenuOptions(player);
            });
        }
        MenuManager.OpenChatMenu(player, TypeMenu);
    }

    private static void SettingsOptions(CCSPlayerController player)
    {
        ChatMenu SettingsMenu = new("Settings");

        SettingsMenu.AddMenuOption("Build Mode: " + (Instance.buildMode ? "ON" : "OFF"), (player, menuOption) =>
        {
            Instance.Command_BuildMode(player);

            SettingsOptions(player);
        });

        SettingsMenu.AddMenuOption("Godmode: " + (Instance.playerData[player.Slot].Godmode ? "ON" : "OFF"), (player, menuOption) =>
        {
            Instance.Command_Godmode(player);

            SettingsOptions(player);
        });

        SettingsMenu.AddMenuOption("Noclip: " + (Instance.playerData[player.Slot].Noclip ? "ON" : "OFF"), (player, menuOption) =>
        {
            Instance.Command_Noclip(player);

            SettingsOptions(player);
        });

        SettingsMenu.AddMenuOption("Save Blocks", (player, menuOption) =>
        {
            Instance.Command_SaveBlocks(player);

            SettingsOptions(player);
        });

        MenuManager.OpenChatMenu(player, SettingsMenu);
    }
}