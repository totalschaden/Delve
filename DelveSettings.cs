using System.Diagnostics.CodeAnalysis;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using SharpDX;

namespace Delve
{
    [SuppressMessage("Interoperability", "CA1416:Plattformkompatibilität überprüfen")]
    public class DelveSettings : ISettings
    {
        // Delve Pathways
        public ToggleNode DelvePathWays = new(true);
        public RangeNode<int> DelvePathWaysNodeSize = new(7, 1, 200);
        public ColorBGRA DelvePathWaysNodeColor = new(255, 140, 0, 255);

        public ToggleNode DelveWall { get; set; } = new(true);
        public RangeNode<int> DelveWallSize { get; set; } = new(18, 1, 200);
        public ColorNode DelveWallColor { get; set; } = new ColorBGRA(255, 255, 255, 255);

        // Delve Chests
        public ToggleNode DelveChests = new(true);

        public ToggleNode DelveMiningSuppliesDynamiteChest { get; set; } = new(true);
        public RangeNode<int> DelveMiningSuppliesDynamiteChestSize { get; set; } = new(15, 1, 200);
        public ColorNode DelveMiningSuppliesDynamiteChestColor { get; set; } = new ColorBGRA(255, 255, 255, 255);

        public ToggleNode DelveMiningSuppliesFlaresChest { get; set; } = new(true);
        public RangeNode<int> DelveMiningSuppliesFlaresChestSize { get; set; } = new(15, 1, 200);
        public ColorNode DelveMiningSuppliesFlaresChestColor { get; set; } = new ColorBGRA(255, 255, 255, 255);

        public ToggleNode DelveAzuriteVeinChest { get; set; } = new(true);
        public RangeNode<int> DelveAzuriteVeinChestSize { get; set; } = new(15, 1, 200);
        public ColorNode DelveAzuriteVeinChestColor { get; set; } = new ColorBGRA(0, 115, 255, 255);

        public ToggleNode DelveCurrencyChest { get; set; } = new(true);
        public RangeNode<int> DelveCurrencyChestSize { get; set; } = new(15, 1, 200);
        public ColorNode DelveCurrencyChestColor { get; set; } = new ColorBGRA(255, 255, 255, 255);

        public ToggleNode DelveResonatorChest { get; set; } = new(true);
        public RangeNode<int> DelveResonatorChestSize { get; set; } = new(15, 1, 200);
        public ColorNode DelveResonatorChestColor { get; set; } = new ColorBGRA(255, 255, 255, 255);

        public ToggleNode DelveFossilChest { get; set; } = new(true);
        public RangeNode<int> DelveFossilChestSize { get; set; } = new(15, 1, 200);
        public ColorNode DelveFossilChestColor { get; set; } = new ColorBGRA(255, 255, 255, 255);

        // Catch all remaining Delve chests
        public ToggleNode DelvePathwayChest { get; set; } = new(true);
        public RangeNode<int> DelvePathwayChestSize { get; set; } = new(15, 1, 200);
        public ColorNode DelvePathwayChestColor { get; set; } = new ColorBGRA(0, 131, 0, 255);

        // Delve Mine Map Connections
        public ToggleNode DelveMineMapConnections { get; set; } = new (true);
        public RangeNode<int> ShowRadiusPercentage { get; set; } = new (80, 0, 100);

        public ToggleNode DebugMode { get; set; } = new (false);
        public ToggleNode ShouldHideOnOpen { get; set; } = new (false);
        
        public ToggleNode DrawPaths { get; set; } = new (true);

        public ToggleNode ColoredPaths { get; set; } = new(true);

        public ToggleNode OnlyShowHiddenPaths { get; set; } = new(false);

        public ToggleNode DrawStraightPaths { get; set; } = new(true);
        public HotkeyNode DebugHotkey { get; set; } = new(System.Windows.Forms.Keys.Menu);
        // Delve Map Grid
        public ToggleNode DelveGridMap { get; set; } = new(true);
        public float DelveGridMapScale { get; set; } = 0.635625f;
        public ToggleNode Enable { get; set; } = new(false);
    }
}
