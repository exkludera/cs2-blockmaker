using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Memory;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace BlockMaker
{
    public static class RayTrace
    {
        public static CounterStrikeSharp.API.Modules.Utils.Vector Vector3toVector(Vector3 vec)
        {
            return new CounterStrikeSharp.API.Modules.Utils.Vector(vec.X, vec.Y, vec.Z);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate bool TraceShapeDelegate(
            nint GameTraceManager,
            nint vecStart,
            nint vecEnd,
            nint skip,
            ulong mask,
            byte a6,
            GameTrace* pGameTrace
        );

        public static void DrawBeam(CounterStrikeSharp.API.Modules.Utils.Vector startPos, CounterStrikeSharp.API.Modules.Utils.Vector endPos, Color color)
        {
            var beam = Utilities.CreateEntityByName<CBeam>("beam");
            if (beam == null)
            {
                Console.WriteLine("Failed to create beam entity.");
                return;
            }

            beam.Render = color;
            beam.Width = 1.5f;
            beam.Teleport(startPos, new CounterStrikeSharp.API.Modules.Utils.QAngle(), new CounterStrikeSharp.API.Modules.Utils.Vector());
            beam.EndPos.Add(endPos);
            beam.DispatchSpawn();
        }

        private static TraceShapeDelegate _traceShape = null!;

        private static nint TraceFunc = NativeAPI.FindSignature(Addresses.ServerPath, "4C 8B DC 49 89 5B ? 49 89 6B ? 49 89 73 ? 57 41 56 41 57 48 81 EC ? ? ? ? 0F 57 C0");
        private static nint GameTraceManager = NativeAPI.FindSignature(Addresses.ServerPath, "48 8B 0D ? ? ? ? 48 8D 45 ? 48 89 44 24 ? 4C 8D 44 24 ? C7 44 24 ? ? ? ? ? 48 8D 54 24 ? 4C 8B CB");

        public static unsafe Vector3? TraceShape(CounterStrikeSharp.API.Modules.Utils.Vector _origin, CounterStrikeSharp.API.Modules.Utils.QAngle _viewangles, bool drawResult = false, bool fromPlayer = false)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                TraceFunc = NativeAPI.FindSignature(Addresses.ServerPath, "48 B8 ? ? ? ? ? ? ? ? 55 48 89 E5 41 57 41 56 49 89 D6 41 55");
                GameTraceManager = NativeAPI.FindSignature(Addresses.ServerPath, "48 8D 05 ? ? ? ? F3 0F 58 8D ? ? ? ? 31 FF");
            }

            var _gameTraceManagerAddress = Address.GetAbsoluteAddress(GameTraceManager, 3, 7);

            _traceShape = Marshal.GetDelegateForFunctionPointer<TraceShapeDelegate>(TraceFunc);

            var _forward = new CounterStrikeSharp.API.Modules.Utils.Vector();

            // Get forward vector from view angles
            NativeAPI.AngleVectors(_viewangles.Handle, _forward.Handle, 0, 0);
            var _endOrigin = new CounterStrikeSharp.API.Modules.Utils.Vector(_origin.X + _forward.X * 8192, _origin.Y + _forward.Y * 8192, _origin.Z + _forward.Z * 8192);

            var d = 50;

            if (fromPlayer)
            {
                _origin.X += _forward.X * d;
                _origin.Y += _forward.Y * d;
                _origin.Z += _forward.Z * d;
            }

            var _trace = stackalloc GameTrace[1];

            var result = _traceShape(*(nint*)_gameTraceManagerAddress, _origin.Handle, _endOrigin.Handle, 0, 0x1C1003, 4, _trace);

            if (result)
            {
                return _trace->EndPos;
            }

            return null;
        }
    }

    internal static class Address
    {
        static unsafe public nint GetAbsoluteAddress(nint addr, nint offset, int size)
        {
            int code = *(int*)(addr + offset);
            return addr + code + size;
        }

        static public nint GetCallAddress(nint a)
        {
            return GetAbsoluteAddress(a, 1, 5);
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x35)]
    public unsafe struct Ray
    {
        [FieldOffset(0)] public Vector3 Start;
        [FieldOffset(0xC)] public Vector3 End;
        [FieldOffset(0x18)] public Vector3 Mins;
        [FieldOffset(0x24)] public Vector3 Maxs;
        [FieldOffset(0x34)] public byte UnkType;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x44)]
    public unsafe struct TraceHitboxData
    {
        [FieldOffset(0x38)] public int HitGroup;
        [FieldOffset(0x40)] public int HitboxId;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0xB8)]
    public unsafe struct GameTrace
    {
        [FieldOffset(0)] public void* Surface;
        [FieldOffset(0x8)] public void* HitEntity;
        [FieldOffset(0x10)] public TraceHitboxData* HitboxData;
        [FieldOffset(0x50)] public uint Contents;
        [FieldOffset(0x78)] public Vector3 StartPos;
        [FieldOffset(0x84)] public Vector3 EndPos;
        [FieldOffset(0x90)] public Vector3 Normal;
        [FieldOffset(0x9C)] public Vector3 Position;
        [FieldOffset(0xAC)] public float Fraction;
        [FieldOffset(0xB6)] public bool AllSolid;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x3a)]
    public unsafe struct TraceFilter
    {
        [FieldOffset(0)] public void* Vtable;
        [FieldOffset(0x8)] public ulong Mask;
        [FieldOffset(0x20)] public fixed uint SkipHandles[4];
        [FieldOffset(0x30)] public fixed ushort arrCollisions[2];
        [FieldOffset(0x34)] public uint Unk1;
        [FieldOffset(0x38)] public byte Unk2;
        [FieldOffset(0x39)] public byte Unk3;
    }

    public unsafe struct TraceFilterV2
    {
        public ulong Mask;
        public fixed ulong V1[2];
        public fixed uint SkipHandles[4];
        public fixed ushort arrCollisions[2];
        public short V2;
        public byte V3;
        public byte V4;
        public byte V5;
    }
}