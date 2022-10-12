using System.Diagnostics.CodeAnalysis;
using ExileCore.PoEMemory.Elements;
using ExileCore.Shared.Helpers;
using GameOffsets.Native;
using SharpDX;
[SuppressMessage("Interoperability", "CA1416:Plattformkompatibilität überprüfen")]
public static class Extensions
{
    public static Vector2 TopMiddle(this RectangleF r)
    {
        return new Vector2(r.Center.X, r.Top);
    }

    public static Vector2 BottomMiddle(this RectangleF r)
    {
        return new Vector2(r.Center.X, r.Bottom);
    }

    public static Vector2 Clamp(this Vector2 v, RectangleF rectangle)
    {
        v = Vector2.Max(v, rectangle.TopLeft);
        v = Vector2.Min(v, rectangle.BottomRight);
        return v;
    }

    public static Vector2 LeftMiddle(this RectangleF r)
    {
        return new Vector2(r.Left, r.Center.Y);
    }

    public static Vector2 RightMiddle(this RectangleF r)
    {
        return new Vector2(r.Right, r.Center.Y);
    }

    public static bool HasEncounter(this DelveCell cell) => cell.M.Read<bool>(cell.Address + 0x48B);
    public static bool IsCellVisible(this DelveCell cell) => cell.M.Read<bool>(cell.Address + 0x48C);
    public static bool PathIsHidden(this DelveCell cell) => cell.M.Read<bool>(cell.Address + 0x48F);
    public static bool WasVisited(this DelveCell cell) => cell.M.Read<bool>(cell.Address + 0x490);
    public static string IconPath(this DelveCell cell) => cell.M.ReadStringU(cell.M.Read<long>(cell.Address + 0x5A8, 0x0));
    public static string Type(this DelveCell cell) => cell.M.ReadStringU(cell.M.Read<long>(cell.Address + 0x658, 0x0));
    public static string TypeHuman(this DelveCell cell) => cell.M.ReadStringU(cell.M.Read<long>(cell.Address + 0x658, 0x8));
    public static string BiomeName(this DelveCell cell) => cell.M.ReadStringU(cell.M.Read<long>(cell.Address + 0x668, 0x0));
    public static string BiomeDisplayName (this DelveCell cell)=> cell.M.ReadStringU(cell.M.Read<long>(cell.Address + 0x668, 0x8));
    public static bool IsFeatureHovered(this DelveCell cell) => cell.M.Read<bool>(cell.Address + 0x6A7);
    public static bool IsCellHovered(this DelveCell cell)=> cell.M.Read<bool>(cell.Address + 0x6A8);
    public static string Mods(this DelveCell cell) => cell. M.Read<NativeStringU>(cell.Address + 0x510).ToString(cell.M);
}