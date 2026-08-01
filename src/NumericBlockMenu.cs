using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using System.Drawing;
using System.Net;
using CssMenuManager = CounterStrikeSharp.API.Modules.Menu.MenuManager;
using ExternalMenuManager = CS2MenuManager.API.Class.MenuManager;

public static class NumericBlockMenu
{
    private enum Screen
    {
        Main,
        Blocks,
        Weapons,
        Rotate,
        Properties,
        Transparency,
        Effect,
        Color
    }

    private sealed class Session : IMenuInstance
    {
        public Session(CCSPlayerController player)
        {
            Player = player;
            Menu = new CenterHtmlMenu("Block Maker", Plugin.Instance);
        }

        public IMenu Menu { get; }
        public CCSPlayerController Player { get; }
        public bool CloseOnSelect => false;
        public int Page { get; private set; }
        public int CurrentOffset => Page * 7;
        public int NumPerPage => 9;
        public Stack<int> PrevPageOffsets { get; } = new();

        public Screen CurrentScreen { get; private set; } = Screen.Main;
        public CBaseProp? PropertyTarget { get; private set; }
        public string WeaponCategory { get; private set; } = "";
        public int BlockPageBeforeWeapons { get; private set; }
        public int PropertyPageBeforeSelection { get; private set; }

        public void Open(Screen screen, int page = 0)
        {
            CurrentScreen = screen;
            Page = Math.Max(0, page);
            Display();
        }

        public void OpenProperties(CBaseProp target, int page = 0)
        {
            PropertyTarget = target;
            Open(Screen.Properties, page);
        }

        public void OpenWeapons(string category)
        {
            WeaponCategory = category;
            BlockPageBeforeWeapons = Page;
            Open(Screen.Weapons);
        }

        public void OpenPropertySelection(Screen screen)
        {
            PropertyPageBeforeSelection = Page;
            Open(screen);
        }

        public void NextPage()
        {
            Page++;
            Display();
        }

        public void PrevPage()
        {
            if (Page > 0)
                Page--;

            Display();
        }

        public void Reset()
        {
            Open(Screen.Main);
        }

        public void Close()
        {
            Sessions.Remove(Player.Slot);

            var activeMenus = CssMenuManager.GetActiveMenus();
            if (activeMenus.TryGetValue(Player.Handle, out var activeMenu) && ReferenceEquals(activeMenu, this))
                activeMenus.Remove(Player.Handle);
        }

        public void Display()
        {
            if (!Player.IsValid)
            {
                Close();
                return;
            }

            Player.PrintToCenterHtml(Render(this), 1);
        }

        public void OnKeyPress(CCSPlayerController player, int key)
        {
            if (player.Slot != Player.Slot)
                return;

            HandleKey(this, key == 10 ? 0 : key);

            if (Sessions.TryGetValue(Player.Slot, out var activeSession) && ReferenceEquals(activeSession, this))
                Display();
        }
    }

    private static readonly Dictionary<int, Session> Sessions = new();

    public static void Open(CCSPlayerController player)
    {
        ExternalMenuManager.CloseActiveMenu(player);
        CssMenuManager.CloseActiveMenu(player);

        var session = new Session(player);
        Sessions[player.Slot] = session;
        CssMenuManager.GetActiveMenus()[player.Handle] = session;
        session.Display();
    }

    public static void OnTick()
    {
        foreach (var session in Sessions.Values.ToList())
            session.Display();
    }

    public static void CloseAll()
    {
        foreach (var session in Sessions.Values.ToList())
            session.Close();
    }

    private static void HandleKey(Session session, int key)
    {
        if (key < 0 || key > 9)
            return;

        switch (session.CurrentScreen)
        {
            case Screen.Main:
                HandleMain(session, key);
                break;
            case Screen.Blocks:
                HandleBlocks(session, key);
                break;
            case Screen.Weapons:
                HandleWeapons(session, key);
                break;
            case Screen.Rotate:
                HandleRotate(session, key);
                break;
            case Screen.Properties:
                HandleProperties(session, key);
                break;
            case Screen.Transparency:
                HandleTransparency(session, key);
                break;
            case Screen.Effect:
                HandleEffects(session, key);
                break;
            case Screen.Color:
                HandleColors(session, key);
                break;
        }
    }

    private static void HandleMain(Session session, int key)
    {
        var player = session.Player;
        var builder = Building.EnsureBuilder(player);

        if (session.Page > 0)
        {
            switch (key)
            {
                case 0:
                    session.Close();
                    break;
                case 1:
                    Commands.ConvertBlock(player);
                    break;
                case 2:
                    Commands.Godmode(player);
                    break;
                case 3:
                    var target = player.GetBlockAim();
                    if (target == null || !target.IsValid || !Blocks.Entities.ContainsKey(target))
                    {
                        Utils.PrintToChat(player, $"{ChatColors.Red}Could not find a block to edit properties");
                        return;
                    }

                    session.OpenProperties(target);
                    break;
                case 8:
                    session.Open(Screen.Main);
                    break;
            }

            return;
        }

        switch (key)
        {
            case 0:
                session.Close();
                break;
            case 1:
                session.Open(Screen.Blocks);
                break;
            case 2:
                Commands.CreateBlock(player);
                break;
            case 3:
                Commands.DeleteBlock(player);
                break;
            case 4:
                Commands.CycleBlockRotation(player);
                break;
            case 5:
                CycleSize(builder);
                break;
            case 6:
                Commands.Noclip(player);
                break;
            case 9:
                session.Open(Screen.Main, 1);
                break;
            case 8:
                session.Close();
                Server.NextFrame(() => Menu.Open(player, "Block Maker"));
                break;
        }
    }

    private static void HandleBlocks(Session session, int key)
    {
        var blocks = Blocks.Models.Data.GetAllBlocks();
        int maxPage = Math.Max(0, (blocks.Count - 1) / 6);

        if (key == 0)
        {
            session.Close();
            return;
        }

        if (key == 8)
        {
            if (session.Page > 0)
                session.PrevPage();
            else
                session.Open(Screen.Main);

            return;
        }

        if (key == 9)
        {
            if (session.Page < maxPage)
                session.NextPage();

            return;
        }

        if (key < 1 || key > 6)
            return;

        int index = session.Page * 6 + key - 1;
        if (index >= blocks.Count)
            return;

        var block = blocks[index];
        if (WeaponList.Categories.ContainsKey(block.Title))
        {
            session.OpenWeapons(block.Title);
            return;
        }

        SelectBlock(session, block.Title);
    }

    private static void HandleWeapons(Session session, int key)
    {
        var weapons = GetWeapons(session.WeaponCategory);
        int maxPage = Math.Max(0, (weapons.Count - 1) / 7);

        if (key == 0)
        {
            session.Close();
            return;
        }

        if (key == 8)
        {
            if (session.Page > 0)
                session.PrevPage();
            else
                session.Open(Screen.Blocks, session.BlockPageBeforeWeapons);

            return;
        }

        if (key == 9)
        {
            if (session.Page < maxPage)
                session.NextPage();

            return;
        }

        if (key < 1 || key > 7)
            return;

        int index = session.CurrentOffset + key - 1;
        if (index >= weapons.Count)
            return;

        SelectBlock(session, $"{session.WeaponCategory}.{weapons[index]}");
    }

    private static void SelectBlock(Session session, string blockType)
    {
        Building.EnsureBuilder(session.Player).BlockType = blockType;
        Utils.PrintToChat(session.Player, $"Selected Type: {ChatColors.White}{blockType}");
        session.Open(Screen.Main);
    }

    private static void HandleRotate(Session session, int key)
    {
        var player = session.Player;
        var builder = Building.EnsureBuilder(player);
        string[] rotations = ["Select Units", "Reset", "X-", "X+", "Y-", "Y+", "Z-", "Z+"];

        if (key == 0)
        {
            session.Close();
            return;
        }

        if (key == 8)
        {
            if (session.Page > 0)
                session.PrevPage();
            else
                session.Open(Screen.Main);

            return;
        }

        if (key == 9)
        {
            if (session.Page == 0)
                session.NextPage();

            return;
        }

        if (key < 1 || key > 7)
            return;

        int index = session.CurrentOffset + key - 1;
        if (index >= rotations.Length)
            return;

        if (index == 0)
        {
            builder.ChatInput = "Rotation";
            Utils.PrintToChat(player, "Write your desired number in the chat");
            return;
        }

        Commands.RotateBlock(player, rotations[index]);
    }

    private static void HandleProperties(Session session, int key)
    {
        if (!TryGetPropertyBlock(session, out var target, out var block))
            return;

        if (key == 0)
        {
            session.Close();
            return;
        }

        if (key == 8)
        {
            if (session.Page > 0)
                session.PrevPage();
            else
                session.Open(Screen.Main, 1);

            return;
        }

        if (key == 9)
        {
            if (session.Page == 0)
                session.NextPage();

            return;
        }

        if (key < 1 || key > 7)
            return;

        int index = session.CurrentOffset + key - 1;
        switch (index)
        {
            case 0:
                SetNumericProperty(session, target, "OnTop", true);
                break;
            case 1:
                SetNumericProperty(session, target, "Duration");
                break;
            case 2:
                SetNumericProperty(session, target, "Value");
                break;
            case 3:
                SetNumericProperty(session, target, "Cooldown");
                break;
            case 4:
                string[] teams = ["Both", "T", "CT"];
                int teamIndex = Array.FindIndex(teams, team => team.Equals(block.Team, StringComparison.OrdinalIgnoreCase));
                block.Team = teams[(teamIndex + 1 + teams.Length) % teams.Length];
                Utils.PrintToChat(session.Player, $"Changed {ChatColors.White}{block.Type} {ChatColors.Grey}Team to {ChatColors.White}{block.Team}");
                break;
            case 5:
                session.OpenPropertySelection(Screen.Transparency);
                break;
            case 6:
                session.OpenPropertySelection(Screen.Effect);
                break;
            case 7:
                session.OpenPropertySelection(Screen.Color);
                break;
        }
    }

    private static void SetNumericProperty(Session session, CBaseProp target, string property, bool toggle = false)
    {
        var builder = Building.EnsureBuilder(session.Player);
        builder.PropertyEntity[property] = target;
        builder.ChatInput = property;

        if (toggle)
        {
            Commands.Properties(session.Player, property, property);
            return;
        }

        Utils.PrintToChat(session.Player, $"Write your desired {property.ToLowerInvariant()} in the chat");
    }

    private static void HandleTransparency(Session session, int key)
    {
        string[] values = ["100%", "75%", "50%", "25%", "0%"];

        if (key == 0)
        {
            session.Close();
            return;
        }

        if (key == 8)
        {
            session.Open(Screen.Properties, session.PropertyPageBeforeSelection);
            return;
        }

        if (key < 1 || key > values.Length || !TryGetPropertyBlock(session, out var target, out var block))
            return;

        string value = values[key - 1];
        block.Transparency = value;

        var color = Utils.GetColor(block.Color);
        int alpha = Utils.GetAlpha(value);
        target.Render = Color.FromArgb(alpha, color.R, color.G, color.B);
        Utilities.SetStateChanged(target, "CBaseModelEntity", "m_clrRender");

        Utils.PrintToChat(session.Player, $"Changed {ChatColors.White}{block.Type} {ChatColors.Grey}Transparency to {ChatColors.White}{value}");
        session.Open(Screen.Properties, session.PropertyPageBeforeSelection);
    }

    private static void HandleEffects(Session session, int key)
    {
        var effects = GetEffects();
        HandlePagedPropertySelection(
            session,
            key,
            effects.Count,
            index =>
            {
                if (!TryGetPropertyBlock(session, out var target, out var block))
                    return;

                var effect = effects[index];
                var replacement = Blocks.ChangeEffect(session.Player, target, effect);
                if (replacement != null)
                    session.OpenProperties(replacement, session.PropertyPageBeforeSelection);
            }
        );
    }

    private static void HandleColors(Session session, int key)
    {
        var colors = Utils.ColorMapping.Keys.ToList();
        HandlePagedPropertySelection(
            session,
            key,
            colors.Count,
            index =>
            {
                if (!TryGetPropertyBlock(session, out var target, out var block))
                    return;

                string colorName = colors[index];
                block.Color = colorName;

                var color = Utils.GetColor(colorName);
                int alpha = Utils.GetAlpha(block.Transparency);
                target.Render = Color.FromArgb(alpha, color.R, color.G, color.B);
                Utilities.SetStateChanged(target, "CBaseModelEntity", "m_clrRender");

                Utils.PrintToChat(session.Player, $"Changed {ChatColors.White}{block.Type} {ChatColors.Grey}Color to {ChatColors.White}{colorName}");
                session.Open(Screen.Properties, session.PropertyPageBeforeSelection);
            }
        );
    }

    private static void HandlePagedPropertySelection(Session session, int key, int itemCount, Action<int> select)
    {
        int maxPage = Math.Max(0, (itemCount - 1) / 7);

        if (key == 0)
        {
            session.Close();
            return;
        }

        if (key == 8)
        {
            if (session.Page > 0)
                session.PrevPage();
            else
                session.Open(Screen.Properties, session.PropertyPageBeforeSelection);

            return;
        }

        if (key == 9)
        {
            if (session.Page < maxPage)
                session.NextPage();

            return;
        }

        if (key < 1 || key > 7)
            return;

        int index = session.CurrentOffset + key - 1;
        if (index < itemCount)
            select(index);
    }

    private static bool TryGetPropertyBlock(Session session, out CBaseProp target, out Blocks.Data block)
    {
        target = session.PropertyTarget!;

        if (target != null && target.IsValid && Blocks.Entities.TryGetValue(target, out block!))
            return true;

        block = null!;
        Utils.PrintToChat(session.Player, $"{ChatColors.Red}The selected block no longer exists");
        session.Open(Screen.Main, 1);
        return false;
    }

    private static void CycleSize(Building.BuilderData builder)
    {
        if (builder.BlockPole)
        {
            builder.BlockPole = false;
            builder.BlockSize = "Small";
            return;
        }

        switch (builder.BlockSize.ToLowerInvariant())
        {
            case "small":
                builder.BlockSize = "Normal";
                break;
            case "normal":
                builder.BlockSize = "Large";
                break;
            case "large":
                builder.BlockSize = "Normal";
                builder.BlockPole = true;
                break;
            default:
                builder.BlockSize = "Small";
                break;
        }
    }

    private static string Render(Session session)
    {
        return session.CurrentScreen switch
        {
            Screen.Main => RenderMain(session),
            Screen.Blocks => RenderBlocks(session),
            Screen.Weapons => RenderWeapons(session),
            Screen.Rotate => RenderRotate(session),
            Screen.Properties => RenderProperties(session),
            Screen.Transparency => RenderPagedList("Select Transparency", ["100%", "75%", "50%", "25%", "0%"], session.Page),
            Screen.Effect => RenderPagedList("Select Effect", GetEffects().Select(effect => effect.Title).ToList(), session.Page),
            Screen.Color => RenderPagedList("Select Color", Utils.ColorMapping.Keys.ToList(), session.Page),
            _ => ""
        };
    }

    private static string RenderMain(Session session)
    {
        var builder = Building.EnsureBuilder(session.Player);

        if (session.Page > 0)
        {
            return Layout(
                "Block Menu [2/2]",
                [
                    "1. Convert",
                    $"2. Godmode: {OnOff(builder.Godmode)}",
                    "3. Properties"
                ],
                NavigationFooter(previous: true, next: false)
            );
        }

        string size = builder.BlockPole ? "Pole" : builder.BlockSize;

        return Layout(
            "Block Menu [1/2]",
            [
                $"1. Block: {Encode(builder.BlockType)}",
                "2. Create",
                "3. Delete",
                "4. Rotate",
                $"5. Size: {Encode(size)}",
                $"6. Noclip: {OnOff(builder.Noclip)}"
            ],
            NavigationFooter(previous: true, next: true)
        );
    }

    private static string RenderBlocks(Session session)
    {
        var blocks = Blocks.Models.Data.GetAllBlocks();
        var names = blocks.Select(block => block.Title).ToList();
        int start = session.Page * 6;
        var lines = names
            .Skip(start)
            .Take(6)
            .Select((name, index) => $"{index + 1}. {Encode(name)}")
            .ToList();

        return Layout(
            $"Select Block [{session.Page + 1}/{Math.Max(1, (names.Count + 5) / 6)}]",
            lines,
            NavigationFooter(previous: true, next: start + 6 < names.Count)
        );
    }

    private static string RenderWeapons(Session session)
    {
        return RenderPagedList($"Select {Encode(session.WeaponCategory)}", GetWeapons(session.WeaponCategory), session.Page);
    }

    private static string RenderRotate(Session session)
    {
        float value = Building.EnsureBuilder(session.Player).RotationValue;
        string[] options = ["Select Units", "Reset", "X-", "X+", "Y-", "Y+", "Z-", "Z+"];
        return RenderPagedList($"Rotate ({value:0.##} Units)", options, session.Page);
    }

    private static string RenderProperties(Session session)
    {
        if (!TryGetPropertyBlock(session, out _, out var block))
            return RenderMain(session);

        var properties = block.Properties;
        string[] options =
        [
            $"OnTop: {OnOff(properties.OnTop)}",
            $"Duration: {properties.Duration:0.##}",
            $"Value: {properties.Value:0.##}",
            $"Cooldown: {properties.Cooldown:0.##}",
            $"Team: {block.Team}",
            $"Transparency: {block.Transparency}",
            $"Effect: {GetEffectTitle(block.Effect)}",
            $"Color: {block.Color}"
        ];

        return RenderPagedList($"Properties: {Encode(block.Type)}", options, session.Page);
    }

    private static string RenderPagedList(string title, IReadOnlyList<string> items, int page)
    {
        int start = page * 7;
        var lines = items
            .Skip(start)
            .Take(7)
            .Select((item, index) => $"{index + 1}. {Encode(item)}")
            .ToList();

        return Layout(
            $"{title} [{page + 1}/{Math.Max(1, (items.Count + 6) / 7)}]",
            lines,
            NavigationFooter(previous: true, next: start + 7 < items.Count)
        );
    }

    private static string Layout(string title, IEnumerable<string> lines, string? footer = null)
    {
        string body = string.Join("<br>", lines.Select(line => $"<font color='#d7d7d7'>{line}</font>"));
        string footerLine = string.IsNullOrWhiteSpace(footer) ? "" : $"<br>{footer}";
        return $"<font color='#f4df5b'><b>{title}</b></font><br>{body}{footerLine}";
    }

    private static string NavigationFooter(bool previous, bool next)
    {
        var parts = new List<string>();

        if (previous)
            parts.Add("<font color='#f4df5b'>!8 &lt;- Prev</font>");

        parts.Add("<font color='#ff5b5b'>!0 X Exit</font>");

        if (next)
            parts.Add("<font color='#f4df5b'>!9 -&gt; Next</font>");

        return string.Join("<font color='#d7d7d7'> | </font>", parts);
    }

    private static List<string> GetWeapons(string category)
    {
        if (!WeaponList.Categories.TryGetValue(category, out var weaponIds))
            return [];

        return weaponIds
            .Select(id => WeaponList.Weapons.FirstOrDefault(weapon => weapon.Designer == id)?.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Cast<string>()
            .ToList();
    }

    private static List<Blocks.Effect> GetEffects()
    {
        var effects = new List<Blocks.Effect> { new("None", "") };
        effects.AddRange(Plugin.Instance.Config.Settings.Blocks.Effects);
        return effects;
    }

    private static string GetEffectTitle(string particle)
    {
        if (string.IsNullOrWhiteSpace(particle) || particle.Equals("None", StringComparison.OrdinalIgnoreCase))
            return "None";

        return Plugin.Instance.Config.Settings.Blocks.Effects
            .FirstOrDefault(effect => effect.Particle.Equals(particle, StringComparison.OrdinalIgnoreCase))
            ?.Title ?? particle;
    }

    private static string OnOff(bool value) => value ? "On" : "Off";
    private static string Encode(string value) => WebUtility.HtmlEncode(value);
}
