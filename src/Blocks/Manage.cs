using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using System.Drawing;

public partial class Blocks
{
    public static void CycleRotation(CCSPlayerController player)
    {
        var entity = player.GetBlockAim();

        if (entity == null)
        {
            Utils.PrintToChat(player, $"{ChatColors.Red}Could not find a block to rotate");
            return;
        }

        if (!Entities.TryGetValue(entity, out var block))
            return;

        if (Utils.BlockLocked(player, block))
            return;

        var current = block.Entity.AbsRotation!;
        QAngle rotation;
        string orientation;

        if (block.Pole)
        {
            // Pole models are long on local X: point that axis along world
            // X, Y, then Z for three visibly different orientations.
            if (IsRotation(current, 0f, 0f, 0f))
            {
                rotation = new QAngle(0f, 90f, 0f);
                orientation = "Horizontal Y";
            }
            else if (IsRotation(current, 0f, 90f, 0f))
            {
                rotation = new QAngle(90f, 0f, 0f);
                orientation = "Vertical";
            }
            else
            {
                rotation = new QAngle(0f, 0f, 0f);
                orientation = "Horizontal X";
            }
        }
        else
        {
            // Regular models are wide on local X/Y and thin on local Z:
            // point the thin axis along world Z, X, then Y.
            if (IsRotation(current, 0f, 0f, 0f))
            {
                rotation = new QAngle(90f, 0f, 0f);
                orientation = "Standing X";
            }
            else if (IsRotation(current, 90f, 0f, 0f))
            {
                rotation = new QAngle(90f, 0f, 90f);
                orientation = "Standing Y";
            }
            else
            {
                rotation = new QAngle(0f, 0f, 0f);
                orientation = "Horizontal";
            }
        }

        block.Entity.Teleport(null, rotation);

        if (config.Sounds.Building.Enabled)
            player.EmitSound(config.Sounds.Building.Rotate);

        Utils.PrintToChat(player, $"Rotated Block: {ChatColors.White}{orientation}");
    }

    private static bool IsSameAngle(float angle, float expected)
    {
        float normalized = ((angle % 360f) + 360f) % 360f;
        float normalizedExpected = ((expected % 360f) + 360f) % 360f;
        float difference = MathF.Abs(normalized - normalizedExpected);
        return MathF.Min(difference, 360f - difference) < 0.1f;
    }

    private static bool IsRotation(QAngle rotation, float pitch, float yaw, float roll) =>
        IsSameAngle(rotation.X, pitch) &&
        IsSameAngle(rotation.Y, yaw) &&
        IsSameAngle(rotation.Z, roll);

    public static void Position(CCSPlayerController player, string input, bool rotate)
    {
        var entity = player.GetBlockAim();

        float value = rotate ? Building.Builders[player.Slot].RotationValue : Building.Builders[player.Slot].PositionValue;

        if (entity == null)
        {
            Utils.PrintToChat(player, $"{ChatColors.Red}Could not find a block to modify position");
            return;
        }

        if (Entities.TryGetValue(entity, out var block))
        {
            if (Utils.BlockLocked(player, block))
                return;

            if (string.IsNullOrEmpty(input))
            {
                Utils.PrintToChat(player, $"{ChatColors.Red}Input option cannot be empty");
                return;
            }

            var Rotation = block.Entity.AbsRotation!;
            var Position = block.Entity.AbsOrigin!;

            QAngle rot = new(Rotation.X, Rotation.Y, Rotation.Z);
            Vector pos = new(Position.X, Position.Y, Position.Z);

            if (string.Equals(input, "x-", StringComparison.OrdinalIgnoreCase))
            {
                if (rotate) rot.X -= value;
                else pos.X -= value;
            }
            else if (string.Equals(input, "x+", StringComparison.OrdinalIgnoreCase))
            {
                if (rotate) rot.X += value;
                else pos.X += value;
            }
            else if (string.Equals(input, "y-", StringComparison.OrdinalIgnoreCase))
            {
                if (rotate) rot.Y -= value;
                else pos.Y -= value;
            }
            else if (string.Equals(input, "y+", StringComparison.OrdinalIgnoreCase))
            {
                if (rotate) rot.Y += value;
                else pos.Y += value;
            }
            else if (string.Equals(input, "z-", StringComparison.OrdinalIgnoreCase))
            {
                if (rotate) rot.Z -= value;
                else pos.Z -= value;
            }
            else if (string.Equals(input, "z+", StringComparison.OrdinalIgnoreCase))
            {
                if (rotate) rot.Z += value;
                else pos.Z += value;
            }
            else if (string.Equals(input, "reset", StringComparison.OrdinalIgnoreCase))
            {
                rot = new QAngle();
            }

            else
            {
                Utils.PrintToChat(player, $"{ChatColors.White}{input} {ChatColors.Red}is not a valid option");
                return;
            }

            if (rotate) block.Entity.Teleport(null, rot);
            else block.Entity.Teleport(pos);

            if (config.Sounds.Building.Enabled)
                player.EmitSound(config.Sounds.Building.Rotate);

            string text = $"{ChatColors.White}{input} {(string.Equals(input, "reset", StringComparison.OrdinalIgnoreCase) ? $"" : $"by {value} Units")}";

            if (rotate) Utils.PrintToChat(player, $"Rotated Block: {text}");
            else Utils.PrintToChat(player, $"Moved Block: {text}");
        }
    }

    public static void Convert(CCSPlayerController player)
    {
        var entity = player.GetBlockAim();

        if (entity == null)
        {
            Utils.PrintToChat(player, $"{ChatColors.Red}Could not find a block to convert");
            return;
        }

        if (entity.Entity == null || string.IsNullOrEmpty(entity.Entity.Name))
            return;

        if (Entities.TryGetValue(entity, out var block))
        {
            if (Utils.BlockLocked(player, block))
                return;

            block.Entity.Remove();
            Entities.Remove(block.Entity);

            var BuilderData = Building.Builders[player.Slot];

            CreateBlock(player, BuilderData.BlockType, BuilderData.BlockPole, BuilderData.BlockSize, entity.AbsOrigin!, entity.AbsRotation!, BuilderData.BlockColor, BuilderData.BlockTransparency, BuilderData.BlockTeam, BuilderData.BlockEffect?.Particle ?? "");

            Utils.PrintToChat(player, $"Converted -" +
                $" type: {ChatColors.White}{BuilderData.BlockType}{ChatColors.Grey}," +
                $" size: {ChatColors.White}{BuilderData.BlockSize}{ChatColors.Grey}," +
                $" color: {ChatColors.White}{BuilderData.BlockColor}{ChatColors.Grey}," +
                $" team: {ChatColors.White}{BuilderData.BlockTeam}{ChatColors.Grey}," +
                $" transparency: {ChatColors.White}{BuilderData.BlockTransparency},"
            );
        }
    }

    public static void Copy(CCSPlayerController player)
    {
        var entity = player.GetBlockAim();

        if (entity == null)
        {
            Utils.PrintToChat(player, $"{ChatColors.Red}Could not find a block to copy");
            return;
        }

        if (entity.Entity == null || string.IsNullOrEmpty(entity.Entity.Name))
            return;

        if (Entities.TryGetValue(entity, out var block))
        {
            if (Utils.BlockLocked(player, block))
                return;

            var BuilderData = Building.Builders[player.Slot];

            CreateBlock(player, block.Type, block.Pole, block.Size, entity.AbsOrigin!, entity.AbsRotation!, block.Color, block.Transparency, block.Team, block.Effect, block.Properties);

            if (config.Sounds.Building.Enabled)
                player.EmitSound(config.Sounds.Building.Create);

            Utils.PrintToChat(player, $"Copied -" +
                $" type: {ChatColors.White}{block.Type}{ChatColors.Grey}," +
                $" size: {ChatColors.White}{block.Size}{ChatColors.Grey}," +
                $" color: {ChatColors.White}{block.Color}{ChatColors.Grey}," +
                $" team: {ChatColors.White}{block.Team}{ChatColors.Grey}," +
                $" transparency: {ChatColors.White}{block.Transparency}"
            );
        }
    }

    public static void Lock(CCSPlayerController player)
    {
        var entity = player.GetBlockAim();

        if (entity == null)
        {
            Utils.PrintToChat(player, $"{ChatColors.Red}Could not find a block to lock");
            return;
        }

        if (entity.Entity == null || string.IsNullOrEmpty(entity.Entity.Name))
            return;

        if (Entities.TryGetValue(entity, out var block))
        {
            block.Properties.Locked = !block.Properties.Locked;

            Utils.PrintToChat(player, $"{(block.Properties.Locked ? "Locked" : "Unlocked")} -" +
                $" type: {ChatColors.White}{block.Type}{ChatColors.Grey}," +
                $" size: {ChatColors.White}{block.Size}{ChatColors.Grey}," +
                $" color: {ChatColors.White}{block.Color}{ChatColors.Grey}," +
                $" team: {ChatColors.White}{block.Team}{ChatColors.Grey}," +
                $" transparency: {ChatColors.White}{block.Transparency}"
            );
        }
    }

    private static bool lockedAll;
    public static void LockAll(CCSPlayerController player)
    {
        if (lockedAll)
        {
            foreach (var block in Entities.Values)
                block.Properties.Locked = false;

            Utils.PrintToChat(player, "Unlocked all blocks");

            lockedAll = false;
        }
        else
        {
            foreach (var block in Entities.Values)
                block.Properties.Locked = true;

            Utils.PrintToChat(player, "Locked all blocks");

            lockedAll = true;
        }
    }

    public static void RenderColor(CCSPlayerController player)
    {
        var entity = player.GetBlockAim();

        if (entity == null)
            return;

        if (entity.Entity == null || string.IsNullOrEmpty(entity.Entity.Name))
            return;

        if (Entities.TryGetValue(entity, out var block))
        {
            if (Utils.BlockLocked(player, block))
                return;

            var color = Building.Builders[player.Slot].BlockColor;

            var clr = Utils.GetColor(color);
            int alpha = Utils.GetAlpha(block.Transparency);
            entity.Render = Color.FromArgb(alpha, clr.R, clr.G, clr.B);
            Utilities.SetStateChanged(entity, "CBaseModelEntity", "m_clrRender");

            Entities[entity].Color = color;

            Utils.PrintToChat(player, $"Changed block color to {ChatColors.White}{color}");
        }
    }

    public static void ChangeProperties(CCSPlayerController player, string type, string input)
    {
        var BuilderData = Building.Builders[player.Slot];

        if (!BuilderData.PropertyEntity.TryGetValue(type, out var entity) || entity == null)
        {
            BuilderData.ChatInput = "";
            BuilderData.PropertyEntity.Clear();
            Utils.PrintToChat(player, $"{ChatColors.Red}No entity found for {type}");
            return;
        }

        bool isToggle = input == "Reset" || input == "OnTop" || input == "Locked";
        bool validNumber = float.TryParse(input, out float number) &&
                           (number > 0 || (type == "Cooldown" && number == -1));
        if (!isToggle && !validNumber)
        {
            Utils.PrintToChat(player, $"{ChatColors.Red}Invalid input value: {ChatColors.White}{input}");
            return;
        }

        if (Entities.TryGetValue(entity, out var block))
        {
            var properties = block.Properties;
            var blocktype = block.Type;

            switch (type)
            {
                case "Reset":
                    var defaultProperties = Properties.BlockProperties[blocktype.Split('.')[0]];
                    block.Properties = new Property
                    {
                        Cooldown = defaultProperties.Cooldown,
                        Value = defaultProperties.Value,
                        Duration = defaultProperties.Duration,
                        OnTop = defaultProperties.OnTop,
                        Locked = defaultProperties.Locked,
                        Builder = properties.Builder,
                    };
                    Utils.PrintToChat(player, $"{ChatColors.White}{blocktype} {ChatColors.Grey}properties has been reset");
                    break;
                case "OnTop":
                    properties.OnTop = !properties.OnTop;
                    Utils.PrintToChat(player, $"Changed {ChatColors.White}{blocktype} {ChatColors.Grey}{type} to {ChatColors.White}{(properties.OnTop ? "Enabled" : "Disabled")}{ChatColors.Grey}");
                    break;
                case "Locked":
                    properties.Locked = !properties.Locked;
                    Utils.PrintToChat(player, $"Changed {ChatColors.White}{blocktype} {ChatColors.Grey}{type} to {ChatColors.White}{(properties.Locked ? "Enabled" : "Disabled")}{ChatColors.Grey}");
                    break;
                case "Duration":
                    properties.Duration = number;
                    Utils.PrintToChat(player, $"Changed {ChatColors.White}{blocktype} {ChatColors.Grey}{type} to {ChatColors.White}{input}{ChatColors.Grey}");
                    break;
                case "Value":
                    properties.Value = number;
                    Utils.PrintToChat(player, $"Changed {ChatColors.White}{blocktype} {ChatColors.Grey}{type} to {ChatColors.White}{input}{ChatColors.Grey}");
                    break;
                case "Cooldown":
                    properties.Cooldown = number;
                    Utils.PrintToChat(player, $"Changed {ChatColors.White}{blocktype} {ChatColors.Grey}{type} to {ChatColors.White}{input}{ChatColors.Grey}");
                    break;
                default:
                    Utils.PrintToChat(player, $"{ChatColors.Red}Unknown property type: {type}");
                    break;
            }
        }

        BuilderData.ChatInput = "";
        BuilderData.PropertyEntity.Remove(type);
    }

    public static void Transparency(CCSPlayerController player, string value)
    {
        var entity = player.GetBlockAim();

        if (entity == null || entity.Entity == null || string.IsNullOrEmpty(entity.Entity.Name))
            return;

        if (Entities.TryGetValue(entity, out var block))
        {
            if (Utils.BlockLocked(player, block))
                return;

            Entities[entity].Transparency = value;

            var color = Utils.GetColor(block.Color);
            int alpha = Utils.GetAlpha(value);
            entity.Render = Color.FromArgb(alpha, color.R, color.G, color.B);
            Utilities.SetStateChanged(entity, "CBaseModelEntity", "m_clrRender");

            Utils.PrintToChat(player, $"Changed block transparency to {ChatColors.White}{value}");
        }
    }

    public static void ChangeEffect(CCSPlayerController player)
    {
        var entity = player.GetBlockAim();

        if (entity == null || entity.Entity == null || string.IsNullOrEmpty(entity.Entity.Name))
            return;

        var effect = Building.Builders[player.Slot].BlockEffect;
        ChangeEffect(player, entity, effect);
    }

    public static CBaseProp? ChangeEffect(CCSPlayerController player, CBaseProp entity, Effect effect)
    {
        if (!Entities.TryGetValue(entity, out var block))
            return null;

        if (Utils.BlockLocked(player, block))
            return null;

        Vector position = new(entity.AbsOrigin!.X, entity.AbsOrigin.Y, entity.AbsOrigin.Z);
        QAngle rotation = new(entity.AbsRotation!.X, entity.AbsRotation.Y, entity.AbsRotation.Z);
        string particle = string.IsNullOrWhiteSpace(effect.Particle) ? "None" : effect.Particle;

        var replacement = CreateBlock(
            player,
            block.Type,
            block.Pole,
            block.Size,
            position,
            rotation,
            block.Color,
            block.Transparency,
            block.Team,
            particle,
            block.Properties
        );

        if (replacement == null)
            return null;

        entity.Remove();
        Entities.Remove(entity);

        Utils.PrintToChat(player, $"Changed block effect to {ChatColors.White}{effect.Title}");
        return replacement;
    }
}
