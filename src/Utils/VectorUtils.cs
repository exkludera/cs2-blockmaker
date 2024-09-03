using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Memory;
using System.Runtime.InteropServices;

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

    public static CBaseProp? GetClosestBlock(Vector start, Vector end, double threshold, CBaseProp excludeBlock)
    {
        CBaseProp? closestBlock = null;
        double closestDistance = double.MaxValue;

        foreach (var prop in Utilities.GetAllEntities().Where(e => e.DesignerName.Contains("prop_physics_override")))
        {
            var currentProp = prop.As<CBaseProp>();

            if (currentProp == excludeBlock)
                continue;

            var pos = currentProp.CBodyComponent!.SceneNode!.AbsOrigin!;

            bool isOnLine = IsPointOnLine(start, end, pos, threshold);

            if (isOnLine)
            {
                double distance = CalculateDistance(start, pos);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBlock = currentProp;
                }
            }
        }

        return closestBlock;
    }

    public static float CalculateDistance(Vector a, Vector b)
    {
        return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2) + Math.Pow(a.Z - b.Z, 2));
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

    public static (Vector position, QAngle rotation) GetEndXYZ(CCSPlayerController player, CBaseProp block, double distance = 250, bool grid = false, float gridValue = 0f, bool snapping = false)
    {
        Vector playerPos = new Vector(player.PlayerPawn.Value!.AbsOrigin!.X, player.PlayerPawn.Value.AbsOrigin.Y, player.PlayerPawn.Value.AbsOrigin.Z + player.PlayerPawn.Value!.CameraServices!.OldPlayerViewOffsetZ); 

        double angleA = -player.PlayerPawn.Value.EyeAngles.X;
        double angleB = player.PlayerPawn.Value.EyeAngles.Y;

        double radianA = (Math.PI / 180) * angleA;
        double radianB = (Math.PI / 180) * angleB;

        double x = playerPos.X + distance * Math.Cos(radianA) * Math.Cos(radianB);
        double y = playerPos.Y + distance * Math.Cos(radianA) * Math.Sin(radianB);
        double z = playerPos.Z + distance * Math.Sin(radianA);

        //snapping grid
        if (grid && gridValue != 0)
        {
            x = (float)Math.Round(x / gridValue) * gridValue;
            y = (float)Math.Round(y / gridValue) * gridValue;
            z = (float)Math.Round(z / gridValue) * gridValue;
        }

        //End Result
        Vector endPos = new Vector((float)x, (float)y, (float)z);
        QAngle endRotation = block.AbsRotation!;

        // Try to snap the position to the closest block
        if (snapping)
        {
            var closestBlock = GetClosestBlock(playerPos, endPos, 64, block);
            if (closestBlock != null)
            {
                endPos = SnapToClosestBlock(endPos, closestBlock);
                endRotation = closestBlock.AbsRotation!;
            }
        }

        return (endPos, endRotation);
    }

    public static Vector SnapToClosestBlock(Vector currentPos, CBaseProp closestBlock)
    {
        Vector closestBlockPos = currentPos;
        float closestDistance = float.MaxValue;

        Vector blockSizeMax = GetBlockSizeMax(closestBlock);
        Vector blockOrigin = closestBlock.AbsOrigin!;
        QAngle blockRotation = closestBlock.AbsRotation!;

        double yaw = blockRotation.Y * Math.PI / 180.0;
        double pitch = blockRotation.X * Math.PI / 180.0;
        double roll = blockRotation.Z * Math.PI / 180.0;

        Vector[] faceOffsets = new Vector[]
        {
            new Vector(-blockSizeMax.X, 0, 0), // -X face
            new Vector(blockSizeMax.X, 0, 0),  // +X face
            new Vector(0, -blockSizeMax.Y, 0), // -Y face
            new Vector(0, blockSizeMax.Y, 0),  // +Y face
            new Vector(0, 0, -blockSizeMax.Z), // -Z face
            new Vector(0, 0, blockSizeMax.Z)   // +Z face
        };

        for (int i = 0; i < faceOffsets.Length; ++i)
        {
            Vector testPos = blockOrigin + faceOffsets[i];

            float distance = CalculateDistance(currentPos, testPos);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestBlockPos = testPos;

                if (blockRotation.Z != 0)
                {
                    if (i == 4)
                        closestBlockPos.Z = testPos.Z + blockSizeMax.X;

                    if (i == 5)
                        closestBlockPos.Z = testPos.Z - blockSizeMax.X;
                }
            }
        }

        return closestBlockPos;
    }

    private static Vector RotateVector(Vector v, double yaw, double pitch, double roll)
    {
        // Rotate around the Z axis (roll)
        Vector temp = new Vector(
            v.X * (float)Math.Cos(roll) * v.Y * (float)Math.Sin(roll),
            v.X * (float)Math.Sin(roll) * v.Y * (float)Math.Cos(roll),
            v.Z
        );

        // Rotate around the X axis (pitch)
        Vector temp2 = new Vector(
            temp.X,
            temp.Y * (float)Math.Cos(pitch) * temp.Z * (float)Math.Sin(pitch),
            temp.Y * (float)Math.Sin(pitch) * temp.Z * (float)Math.Cos(pitch)
        );

        // Rotate around the Y axis (yaw)
        Vector result = new Vector(
            temp2.X * ((float)Math.Cos(yaw) * temp2.Z * (float)Math.Sin(yaw)) * (float)roll,
            temp2.Y,
            -temp2.X * (float)Math.Sin(yaw) * temp2.Z * (float)Math.Cos(yaw)
        );

        return result;
    }

    private static Vector GetBlockSizeMax(CBaseProp block)
    {
        return new Vector(block.Collision.Maxs.X, block.Collision.Maxs.Y, block.Collision.Maxs.Z) * 2;
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