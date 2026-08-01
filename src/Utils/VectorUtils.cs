using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

public static class VectorUtils
{
    private const float SnapActivationDistance = 10.0f;
    private const float SnapComparisonEpsilon = 0.001f;

    public static (Vector position, QAngle rotation) GetEndXYZ(CCSPlayerController player, CBaseProp block, double distance = 250, bool grid = false, float gridValue = 0f, bool snapping = false, float snapValue = 0f)
    {
        if (Blocks.Entities.TryGetValue(Building.BuilderHolds[player].Entity, out var locked))
        {
            if (Blocks.Entities[locked.Entity].Properties.Locked)
            {
                if (Building.BuilderHolds[player].LockedMessage == false)
                    Utils.PrintToChat(player, $"{ChatColors.Red}Block is locked");

                Building.BuilderHolds[player].LockedMessage = true;

                return (block.AbsOrigin!, block.AbsRotation!);
            }
        }

        var pawn = player.Pawn()!;
        var playerpos = pawn.AbsOrigin!;

        Vector aim = new(playerpos.X, playerpos.Y, playerpos.Z + pawn.ViewOffset.Z); 

        double angleA = -pawn.EyeAngles.X;
        double angleB = pawn.EyeAngles.Y;
        double radianA = (Math.PI / 180) * angleA;
        double radianB = (Math.PI / 180) * angleB;
        double x = aim.X + distance * Math.Cos(radianA) * Math.Cos(radianB);
        double y = aim.Y + distance * Math.Cos(radianA) * Math.Sin(radianB);
        double z = aim.Z + distance * Math.Sin(radianA);

        if (grid && gridValue != 0)
        {
            x = (float)Math.Round(x / gridValue) * gridValue;
            y = (float)Math.Round(y / gridValue) * gridValue;
            z = (float)Math.Round(z / gridValue) * gridValue;
        }

        Vector endPos = new((float)x, (float)y, (float)z);
        QAngle endRotation = block.AbsRotation!;

        if (snapping && TryGetSnapPosition(block, endPos, snapValue, out Vector snapPosition))
        {
            endPos = snapPosition;
        }

        return (endPos, endRotation);
    }

    public static bool TryGetSnapPosition(CBaseProp block, Vector proposedPosition, float snapGap, out Vector snapPosition)
    {
        snapPosition = proposedPosition;

        if (!Blocks.Entities.TryGetValue(block, out var heldBlock) ||
            block.AbsRotation == null)
            return false;

        Vector heldHalfSize = (block.Collision.Maxs - block.Collision.Mins) * 0.5f;
        Vector[] heldAxes = GetLocalAxes(block.AbsRotation);

        bool foundSnap = false;
        float bestSurfaceDistance = float.MaxValue;
        float bestMovementDistance = float.MaxValue;

        foreach (var targetBlock in Blocks.Entities.Values)
        {
            CBaseProp target = targetBlock.Entity;

            if (target == null ||
                !target.IsValid ||
                target.AbsOrigin == null ||
                target.AbsRotation == null ||
                target.Handle == block.Handle)
                continue;

            Vector targetHalfSize = (target.Collision.Maxs - target.Collision.Mins) * 0.5f;
            Vector[] targetAxes = GetLocalAxes(target.AbsRotation);
            Vector centerDelta = proposedPosition - target.AbsOrigin;

            for (int axisIndex = 0; axisIndex < 3; axisIndex++)
            {
                for (int direction = -1; direction <= 1; direction += 2)
                {
                    Vector normal = targetAxes[axisIndex] * direction;
                    float normalDistance = Dot(centerDelta, normal);

                    // The opposite direction handles positions on the other side.
                    if (normalDistance < 0)
                        continue;

                    float targetExtent = GetComponent(targetHalfSize, axisIndex);
                    float heldExtent = ProjectedHalfExtent(heldHalfSize, heldAxes, normal);
                    float surfaceGap = normalDistance - targetExtent - heldExtent;
                    float surfaceDistance = Math.Abs(surfaceGap);

                    if (surfaceDistance > SnapActivationDistance)
                        continue;

                    // Match the classic face traces: the held block's center must
                    // project inside the target face rather than merely being near
                    // the target entity's center-radius.
                    int tangentA = (axisIndex + 1) % 3;
                    int tangentB = (axisIndex + 2) % 3;
                    if (Math.Abs(Dot(centerDelta, targetAxes[tangentA])) > GetComponent(targetHalfSize, tangentA) + SnapComparisonEpsilon ||
                        Math.Abs(Dot(centerDelta, targetAxes[tangentB])) > GetComponent(targetHalfSize, tangentB) + SnapComparisonEpsilon)
                        continue;

                    Vector candidatePosition =
                        target.AbsOrigin +
                        normal * (targetExtent + heldExtent + snapGap);
                    float movementDistance = CalculateDistance(proposedPosition, candidatePosition);

                    bool isBetterSurface =
                        surfaceDistance < bestSurfaceDistance - SnapComparisonEpsilon;
                    bool isSameSurfaceButCloser =
                        Math.Abs(surfaceDistance - bestSurfaceDistance) <= SnapComparisonEpsilon &&
                        movementDistance < bestMovementDistance;

                    if (!isBetterSurface && !isSameSurfaceButCloser)
                        continue;

                    foundSnap = true;
                    bestSurfaceDistance = surfaceDistance;
                    bestMovementDistance = movementDistance;
                    snapPosition = candidatePosition;
                }
            }
        }

        return foundSnap;
    }

    private static Vector[] GetLocalAxes(QAngle rotation)
    {
        double pitch = rotation.X * Math.PI / 180.0;
        double yaw = rotation.Y * Math.PI / 180.0;
        double roll = rotation.Z * Math.PI / 180.0;

        double sinPitch = Math.Sin(pitch);
        double cosPitch = Math.Cos(pitch);
        double sinYaw = Math.Sin(yaw);
        double cosYaw = Math.Cos(yaw);
        double sinRoll = Math.Sin(roll);
        double cosRoll = Math.Cos(roll);

        Vector forward = new(
            (float)(cosPitch * cosYaw),
            (float)(cosPitch * sinYaw),
            (float)-sinPitch
        );
        Vector right = new(
            (float)(-sinRoll * sinPitch * cosYaw + cosRoll * sinYaw),
            (float)(-sinRoll * sinPitch * sinYaw - cosRoll * cosYaw),
            (float)(-sinRoll * cosPitch)
        );
        Vector up = new(
            (float)(cosRoll * sinPitch * cosYaw + sinRoll * sinYaw),
            (float)(cosRoll * sinPitch * sinYaw - sinRoll * cosYaw),
            (float)(cosRoll * cosPitch)
        );

        return [forward, right, up];
    }

    private static float ProjectedHalfExtent(Vector halfSize, Vector[] axes, Vector direction)
    {
        return
            Math.Abs(Dot(direction, axes[0])) * halfSize.X +
            Math.Abs(Dot(direction, axes[1])) * halfSize.Y +
            Math.Abs(Dot(direction, axes[2])) * halfSize.Z;
    }

    private static float GetComponent(Vector vector, int axis)
    {
        return axis switch
        {
            0 => vector.X,
            1 => vector.Y,
            _ => vector.Z
        };
    }

    public static Vector Cross(Vector a, Vector b)
    {
        return new(
            a.Y * b.Z - a.Z * b.Y,
            a.Z * b.X - a.X * b.Z,
            a.X * b.Y - a.Y * b.X
        );
    }

    public static float Dot(Vector a, Vector b)
    {
        return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    }

    public static float CalculateDistance(Vector a, Vector b)
    {
        return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2) + Math.Pow(a.Z - b.Z, 2));
    }

    public static bool IsWithinBounds(Vector entityPosition, Vector playerPosition, Vector entitySize, Vector playerSize)
    {
        bool overlapX = Math.Abs(entityPosition.X - playerPosition.X) <= (entitySize.X + playerSize.X) / 2;
        bool overlapY = Math.Abs(entityPosition.Y - playerPosition.Y) <= (entitySize.Y + playerSize.Y) / 2;
        bool overlapZ = Math.Abs(entityPosition.Z - playerPosition.Z) <= (entitySize.Z + playerSize.Z) / 2;

        return overlapX && overlapY && overlapZ;
    }

    public static bool CheckOnTop(Blocks.Data block, CCSPlayerPawn pawn)
    {
        Vector playerMaxs = pawn.Collision.Maxs * 2;
        Vector blockMaxs = block.Entity.Collision.Maxs * 2;

        Vector blockOrigin = block.Entity.AbsOrigin!;
        Vector pawnOrigin = pawn.AbsOrigin!;
        QAngle blockRotation = block.Entity.AbsRotation!;

        if (!IsTopOnly(blockOrigin, pawnOrigin, blockMaxs, playerMaxs, blockRotation))
            return false;

        return true;
    }
    public static bool IsTopOnly(Vector entityPosition, Vector playerPosition, Vector entitySize, Vector playerSize, QAngle entityRotation)
    {
        Vector forward = new(
            (float)(Math.Cos(entityRotation.Y * Math.PI / 180) * Math.Cos(entityRotation.X * Math.PI / 180)),
            (float)(Math.Sin(entityRotation.Y * Math.PI / 180) * Math.Cos(entityRotation.X * Math.PI / 180)),
            (float)(-Math.Sin(entityRotation.X * Math.PI / 180))
        );
        Vector right = new(
            (float)(Math.Cos((entityRotation.Y + 90) * Math.PI / 180)),
            (float)(Math.Sin((entityRotation.Y + 90) * Math.PI / 180)),
            0
        );
        Vector up = Cross(forward, right);

        Vector[] faceDirections =
        {
            -forward,  // -X face
            forward,   // +X face
            -right,    // -Y face
            right,     // +Y face
            -up,       // -Z face
            up         // +Z face
        };

        // Find the face with the most positive Z-component (most "upward")
        int topFaceIndex = 0;
        float maxZ = float.MinValue;
        for (int i = 0; i < faceDirections.Length; i++)
        {
            if (faceDirections[i].Z > maxZ)
            {
                maxZ = faceDirections[i].Z;
                topFaceIndex = i;
            }
        }

        Vector topFaceNormal = faceDirections[topFaceIndex];
        Vector localX, localY, localZ;
        float faceWidth, faceHeight, faceDepth;

        // Map the face to its local coordinate system and dimensions
        switch (topFaceIndex)
        {
            case 0: // -X face
            case 1: // +X face
                    // For +X face: faceWidth along Y (right), faceHeight along Z (up)
                localX = up;    // Map Z-axis (up) to faceHeight
                localY = right; // Map Y-axis (right) to faceWidth
                localZ = topFaceNormal;
                faceWidth = entitySize.Y;  // Y-axis (right)
                faceHeight = entitySize.Z; // Z-axis (up)
                faceDepth = entitySize.X;  // X-axis (forward)
                break;
            case 2: // -Y face
            case 3: // +Y face
                    // For +Y face: faceWidth along X (forward), faceHeight along Z (up)
                localX = up;     // Map Z-axis (up) to faceHeight
                localY = forward; // Map X-axis (forward) to faceWidth
                localZ = topFaceNormal;
                faceWidth = entitySize.X;  // X-axis (forward)
                faceHeight = entitySize.Z; // Z-axis (up)
                faceDepth = entitySize.Y;  // Y-axis (right)
                break;
            case 4: // -Z face
            case 5: // +Z face
            default:
                // For +Z face: faceWidth along X (right), faceHeight along Y (forward)
                localX = right;  // Map X-axis (right) to faceWidth
                localY = forward; // Map Y-axis (forward) to faceHeight
                localZ = topFaceNormal;
                faceWidth = entitySize.X;  // X-axis (right)
                faceHeight = entitySize.Y; // Y-axis (forward)
                faceDepth = entitySize.Z;  // Z-axis (up)
                break;
        }

        // Calculate the center of the top face
        Vector topFaceCenter = entityPosition + topFaceNormal * (faceDepth / 2);

        // Player position relative to the top face center
        Vector relativePos = playerPosition - topFaceCenter;

        // Project relative position onto the block's local axes
        float localXProj = Dot(relativePos, localX); // Along local X (should map to faceHeight)
        float localYProj = Dot(relativePos, localY); // Along local Y (should map to faceWidth)
        float localZProj = Dot(relativePos, localZ); // Along local Z (normal)

        // Boundary checks with dynamic tolerance based on player size
        float boundaryToleranceX = playerSize.X / 2 + 2.0f; // Account for player's collision box width
        float boundaryToleranceY = playerSize.Y / 2 + 2.0f; // Account for player's collision box height
        float triggerThreshold = 2.0f;
        float zTolerance = 0.1f; // Small tolerance for Z-axis
        bool overlapX = Math.Abs(localXProj) <= (faceHeight / 2) + boundaryToleranceX; // localXProj maps to faceHeight
        bool overlapY = Math.Abs(localYProj) <= (faceWidth / 2) + boundaryToleranceY;  // localYProj maps to faceWidth
        bool onTop = localZProj >= -zTolerance && localZProj <= (playerSize.Z + triggerThreshold);

        // Additional logging for debugging
        /*Console.WriteLine($"entityPosition: {entityPosition}, playerPosition: {playerPosition}, entityRotation: {entityRotation}");
        Console.WriteLine($"topFaceCenter: {topFaceCenter}, relativePos: {relativePos}");
        Console.WriteLine($"entitySize: {entitySize}, playerSize: {playerSize}");
        Console.WriteLine($"TopFace: {topFaceIndex}, localXProj: {localXProj}, localYProj: {localYProj}, localZProj: {localZProj}");
        Console.WriteLine($"faceWidth: {faceWidth}, faceHeight: {faceHeight}, overlapX: {overlapX}, overlapY: {overlapY}, onTop: {onTop}");*/

        return overlapX && overlapY && onTop;
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

        public Vector ToVector() => new(X, Y, Z);
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

        public QAngle ToQAngle() => new(Pitch, Yaw, Roll);
    }
}
