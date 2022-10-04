using System;
using ExileCore;
using ExileCore.PoEMemory.Elements;
using ExileCore.Shared.AtlasHelper;
using SharpDX;

namespace Delve.Libs
{
    public class MapIcon
    {
        public AtlasTexture Texture { get; set; }
        public Color Color { get; set; } = Color.White;
        public float Size { get; set; } = 13;
        private const int TileToGridConversion = 23;
        private const int TileToWorldConversion = 250;
        public const float GridToWorldMultiplier = TileToWorldConversion / (float)TileToGridConversion;
        internal static double _mapScale;
        private const double CameraAngle = 38.7 * Math.PI / 180;
        private static readonly float CameraAngleCos = (float)Math.Cos(CameraAngle);
        private static readonly float CameraAngleSin = (float)Math.Sin(CameraAngle);

        public MapIcon()
        {
        }

        public MapIcon(AtlasTexture texture, Color color, float size)
        {
            Texture = texture;
            Color = color;
            Size = size;
        }
        public MapIcon(AtlasTexture texture, float size)
        {
            Texture = texture;
            Size = size;
        }
        internal static Vector2 TranslateGridDeltaToMapDelta(Vector2 delta, float deltaZ)
        {
            deltaZ /= GridToWorldMultiplier; //z is normally "world" units, translate to grid
            return (float)_mapScale * new Vector2((delta.X - delta.Y) * CameraAngleCos, (deltaZ - (delta.X + delta.Y)) * CameraAngleSin);
        }
        public static Vector2 DeltaInWorldToMinimapDelta(Vector2? delta, double diag, float scale, float deltaZ = 0)
        {
            const float CAMERA_ANGLE = 38 * MathUtil.Pi / 180;

            // Values according to 40 degree rotation of cartesian coordiantes, still doesn't seem right but closer
            var cos = (float) (diag * Math.Cos(CAMERA_ANGLE) / scale);
            var sin = (float) (diag * Math.Sin(CAMERA_ANGLE) / scale); // possible to use cos so angle = nearly 45 degrees

            // 2D rotation formulas not correct, but it's what appears to work?
            return new Vector2((delta.Value.X - delta.Value.Y) * cos, deltaZ - (delta.Value.X + delta.Value.Y) * sin);
        }
    }
}
