using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Memory;
using System.Runtime.InteropServices;

namespace BlockBuilder;

public static class VectorUtils
{
    private static bool IsPointOnLine(Vector start, Vector end, Vector point, double threshold)
    {
        // Calculate the direction vector of the line
        double lineDirectionX = end.X - start.X;
        double lineDirectionY = end.Y - start.Y;
        double lineDirectionZ = end.Z - start.Z;

        // Calculate the vector from start to the point
        double pointVectorX = point.X - start.X;
        double pointVectorY = point.Y - start.Y;
        double pointVectorZ = point.Z - start.Z;

        // Calculate the scalar projection of pointVector onto the lineDirection
        double scalarProjection = (pointVectorX * lineDirectionX + pointVectorY * lineDirectionY + pointVectorZ * lineDirectionZ) /
                                 (lineDirectionX * lineDirectionX + lineDirectionY * lineDirectionY + lineDirectionZ * lineDirectionZ);

        // Check if the scalar projection is within [0, 1], meaning the point is between start and end
        if (scalarProjection >= 0 && scalarProjection <= 1)
        {
            // Calculate the closest point on the line to the given point
            double closestPointX = start.X + scalarProjection * lineDirectionX;
            double closestPointY = start.Y + scalarProjection * lineDirectionY;
            double closestPointZ = start.Z + scalarProjection * lineDirectionZ;

            // Calculate the distance between the given point and the closest point on the line
            double distance = Math.Sqrt(Math.Pow(point.X - closestPointX, 2) + Math.Pow(point.Y - closestPointY, 2) + Math.Pow(point.Z - closestPointZ, 2));

            // Check if the distance is within the specified threshold
            return distance <= threshold;
        }

        // Point is not between start and end
        return false;
    }

    public static CBaseProp? GetBlockAimTarget(this CCSPlayerController player)
    {
        var GameRules = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").FirstOrDefault()?.GameRules;

        if (GameRules is null)
            return null;

        VirtualFunctionWithReturn<IntPtr, IntPtr, IntPtr> findPickerEntity = new(GameRules.Handle, 27);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) findPickerEntity = new(GameRules.Handle, 28);

        var target = new CBaseProp(findPickerEntity.Invoke(GameRules.Handle, player.Handle));

        if (target != null && target.IsValid && target.Entity != null && target.DesignerName.Contains("prop_physics_override")) return target;

        return null;
    }

    public static CBaseProp? GetClosestProp(Vector start, Vector end, double threshold)
    {
        CBaseProp? ent = null;

        foreach (var prop in Utilities.GetAllEntities().Where(e => e.DesignerName.Contains("prop_physics_override")))
        {
            //bool isOnLine = IsPointOnLine(start, end, player.PlayerPawn?.Value.CBodyComponent?.SceneNode?.AbsOrigin);
            var pos = prop.As<CBaseProp>().CBodyComponent!.SceneNode!.AbsOrigin!;
            pos = new Vector(pos.X, pos.Y, pos.Z + 30);
            bool isOnLine = IsPointOnLine(start, end, pos, threshold);

            if (isOnLine)
            {
                ent = prop.As<CBaseProp>();
                break;
            }
        }

        return ent;
    }

    public static float CalculateDistance(Vector a, Vector b)
    {
        return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2) + Math.Pow(a.Z - b.Z, 2));
    }

    public static Vector GetEndXYZ(CCSPlayerController player, double distance = 250)
    {
        double karakterX = (float)player.PlayerPawn.Value!.AbsOrigin!.X;
        double karakterY = (float)player.PlayerPawn.Value.AbsOrigin.Y;
        double karakterZ = (float)player.PlayerPawn.Value.AbsOrigin.Z + player.PlayerPawn.Value!.CameraServices!.OldPlayerViewOffsetZ;

        // Açý deðerleri
        double angleA = -player.PlayerPawn.Value.EyeAngles.X;   // (-90, 90) arasýnda
        double angleB = player.PlayerPawn.Value.EyeAngles.Y; // (-180, 180) arasýnda

        // Açýlarý dereceden radyana çevir
        double radianA = (Math.PI / 180) * angleA;
        double radianB = (Math.PI / 180) * angleB;

        // Açýlara göre XYZ koordinatlarýný hesapla
        double x = karakterX + distance * Math.Cos(radianA) * Math.Cos(radianB);
        double y = karakterY + distance * Math.Cos(radianA) * Math.Sin(radianB);
        double z = karakterZ + distance * Math.Sin(radianA);

        return new Vector((float)x, (float)y, (float)z);
    }

    public static Vector AnglesToDirection(Vector angles)
    {
        float pitch = -angles.X * (float)(Math.PI / 180.0);
        float yaw = angles.Y * (float)(Math.PI / 180.0);

        float x = (float)(Math.Cos(pitch) * Math.Cos(yaw));
        float y = (float)(Math.Cos(pitch) * Math.Sin(yaw));
        float z = (float)Math.Sin(pitch);

        return new Vector(x, y, z);
    }

    public static Vector AddInFrontOf(Vector origin, QAngle angles, float units)
    {
        Vector direction = AnglesToDirection(new Vector(angles.X, angles.Y, angles.Z));
        return origin + (direction * units);
    }

    public class VectorDTO
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public VectorDTO() { }

        public VectorDTO(Vector vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        public Vector ToVector()
        {
            return new Vector(X, Y, Z);
        }
    }

    public class QAngleDTO
    {
        public float Pitch { get; set; }
        public float Yaw { get; set; }
        public float Roll { get; set; }

        public QAngleDTO() { }

        public QAngleDTO(QAngle qangle)
        {
            Pitch = qangle.X;
            Yaw = qangle.Y;
            Roll = qangle.Z;
        }

        public QAngle ToQAngle()
        {
            return new QAngle(Pitch, Yaw, Roll);
        }
    }

}