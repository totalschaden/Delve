using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using DefaultNamespace;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SharpDX;
using ExileCore;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using ExileCore.PoEMemory.Elements;
using ExileCore.PoEMemory.Components;
using ExileCore.Shared.Helpers;
using Delve.Libs;
using Microsoft.VisualBasic.Logging;

namespace Delve
{
    [SuppressMessage("Interoperability", "CA1416:Plattformkompatibilität überprüfen")]
    public partial class DelveCore : BaseSettingsPlugin<DelveSettings>
    {
        private RectangleF DrawRect;

        public float CurrentDelveMapZoom = 0.635625f;

        private List<DelveEntity> DelveEntities;

        public Version version = Assembly.GetExecutingAssembly().GetName().Version;
        public string PluginVersion;
        public DateTime buildDate;
        public static int Selected;

        public static int idPop;

        private bool IsAzuriteMine;

        public FossilTiers FossilList = new();
        public LargeMapData LargeMapInformation { get; set; }
        private IconStore iconStore;
        
        
        
        private float updateCooldown = 250;

        public override bool Initialise()
        {
            AtlasTextureInit();
            Input.RegisterKey(Settings.DebugHotkey);
            iconStore = new IconStore(CreateAtlas(
                    Path.Join(DirectoryFullName, "Textures", "chart", "icons.json"),
                    Path.Join(DirectoryFullName, "Textures", "chart", "icons.png")),
                Path.Join(DirectoryFullName, "chart_icons.json"));

            buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);

            Name = "Delve";
            PluginVersion = $"{version}";
            DelveEntities = new List<DelveEntity>();
            
            GameController.Area.OnAreaChange += AreaChange;

            if (File.Exists($@"{DirectoryFullName}\Fossil_Tiers.json"))
            {
                var jsonFile = File.ReadAllText($@"{DirectoryFullName}\Fossil_Tiers.json");
                FossilList = JsonConvert.DeserializeObject<FossilTiers>(jsonFile, JsonSettings);
            }
            else
            {
                LogError("Error loading Fossil_Tiers.json, Please re download from Random Features github repository", 10);
            }

            return true;
        }

        public static readonly JsonSerializerSettings JsonSettings = new()
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };

        public override void AreaChange(AreaInstance area)
        {
            base.AreaChange(area);
            DelveEntities = new List<DelveEntity>();
            IsAzuriteMine = GameController.Area.CurrentArea.Name.Contains("Azurite Mine");
        }

        private void DrawData(string icon, string data)
        {
            // var textSize = Graphics.MeasureText(data, 20, FontAlign.Left | FontAlign.Center);
            var textSize = Graphics.MeasureText(data, 20);

            DrawRect.Width = DrawRect.Height + textSize.X + 10;

            Graphics.DrawBox(DrawRect, Color.Black);

            var imgRect = DrawRect;
            imgRect.Width = imgRect.Height;
            //TODO: Need init images
            Graphics.DrawImage(Path.Combine(DirectoryFullName, icon), imgRect);
            Graphics.DrawFrame(DrawRect, Color.Gray, 2);

            var textPos = DrawRect.TopLeft;
            textPos.Y += DrawRect.Height / 2;
            textPos.X += DrawRect.Height + 3;

            Graphics.DrawText(data, textPos, Color.White, 20, FontAlign.Left | FontAlign.Center);

            DrawRect.X += DrawRect.Width + 1;
        }

        public override void Render()
        {
            base.Render();
            if (!Settings.Enable.Value || !IsAzuriteMine) return;
            Helper.GetDeltaTime();
            updateCooldown = Helper.MoveTowards(updateCooldown, 0, Helper._deltaTime);
            if (updateCooldown == 0)
                UpdateEntitys();
            var map = GameController.Game.IngameState.IngameUi.Map;
            var largeMap = map.LargeMap.AsObject<SubMap>();
           MapIcon._mapScale = GameController.IngameState.Camera.Height / 677f * largeMap.Zoom;
           
            // SubterraneanChart MineMap = GameController.Game.IngameState.IngameUi.MineMap;
            SubterraneanChart mineMap = GameController.Game.IngameState.IngameUi.DelveWindow;
            if (Settings.DelveMineMapConnections.Value) DrawMineMapConnections(mineMap);
            if (Settings.DelveGridMap.Value) DelveMapNodes(mineMap);
            if (!mineMap.IsVisible) RenderMapImages();
            var scale = mineMap.Children[0].Children[0].Children[2].Scale;
            CurrentDelveMapZoom = scale;
            if (Settings.DebugHotkey.PressedOnce())
                Settings.DebugMode.Value = !Settings.DebugMode.Value;
            if (Settings.DebugMode.Value)
            {
                foreach (var entity in DelveEntities)
                {
                    if (entity.Path.StartsWith("Metadata/Terrain/Leagues/Delve/Objects/DelveWall") || entity.Path.StartsWith("Metadata/Terrain/Leagues/Delve/Objects/DelveLight"))
                        continue;

                    var chestIsOpened = entity?.IsOpened;

                    if (Settings.ShouldHideOnOpen.Value && chestIsOpened == true)
                        continue;
                    var TextToDisplay = entity.Path.Replace("Metadata/Chests/DelveChests/", "");
                    var textBox = Graphics.MeasureText(TextToDisplay, 0);
                    var screenPosition = GameController.Game.IngameState.Camera.WorldToScreen(entity.Pos);
                    Graphics.DrawBox(new RectangleF((screenPosition.X - textBox.X/2) - 10, screenPosition.Y - textBox.Y/2, textBox.X + 20, textBox.Y * 2), Color.White);
                    Graphics.DrawText(TextToDisplay, screenPosition, Color.Black, 20, FontAlign.Center);

                    /*
                    TextToDisplay = "Cached -> IsOpened: " + entity.IsOpened;
                    textBox = Graphics.MeasureText(TextToDisplay, 0);
                    screenPosition.Y += 25;
                    Graphics.DrawBox(new RectangleF((screenPosition.X - textBox.X/2) - 10, screenPosition.Y - textBox.Y/2, textBox.X + 20, textBox.Y * 2), Color.White);
                    Graphics.DrawText(TextToDisplay, screenPosition, Color.Black, 20, FontAlign.Center);
                    
                    TextToDisplay = "Real -> IsOpened: " + GameController.Entities.First(x => x != null && x.Id == entity.Id)?.IsOpened;
                    textBox = Graphics.MeasureText(TextToDisplay, 0);
                    screenPosition.Y += 25;
                    Graphics.DrawBox(new RectangleF((screenPosition.X - textBox.X/2) - 10, screenPosition.Y - textBox.Y/2, textBox.X + 20, textBox.Y * 2), Color.White);
                    Graphics.DrawText(TextToDisplay, screenPosition, Color.Black, 20, FontAlign.Center);
                    */
                }
            }
        }

        private void DelveMapNodes(SubterraneanChart mineMap)
        {
            if (!Settings.DelveGridMap) return;
            if (!mineMap.IsVisible) return;
            try
            {
                var largeGridList = mineMap.Children[0].Children[0].Children[2].Children.ToList();
                var scale = mineMap.Children[0].Children[0].Children[2].Scale;
                CurrentDelveMapZoom = scale;

                if (scale != Settings.DelveGridMapScale) return;
                //LogMessage($"Count: {largeGrids.Count}", 5);
                for (var i = 0; i < largeGridList.Count; i++)
                {
                    var largeGrid = largeGridList[i];

                    if (!largeGrid.GetClientRect().Intersects(mineMap.GetClientRect())) continue;

                    var smallGridList = largeGrid.Children.ToList();
                    for (var j = 0; j < smallGridList.Count - 1; j++)
                    {
                        var smallGrid = smallGridList[j];

                        //var newRec = new RectangleF(
                        //    (smallGrid.GetClientRect().X * 5.69f) * scale, (smallGrid.GetClientRect().Y * 5.69f) * scale,
                        //    (smallGrid.GetClientRect().Width), smallGrid.GetClientRect().Height);

                        if (smallGrid.GetClientRect().Intersects(mineMap.GetClientRect()))
                            Graphics.DrawFrame(smallGrid.GetClientRect(), Color.DarkGray, 1);
                    }
                }
            }
            catch
            {

            }
        }

        public class LargeMapData
        {
            public Camera @Camera { get; set; }
            public ExileCore.PoEMemory.Elements.Map @MapWindow { get; set; }
            public RectangleF @MapRec { get; set; }
            public Vector2 @PlayerPos { get; set; }
            public float @PlayerPosZ { get; set; }
            public Vector2 @ScreenCenter { get; set; }
            public float @Diag { get; set; }
            public float @K { get; set; }
            public float @Scale { get; set; }

            public LargeMapData(GameController GC)
            {
                @Camera = GC.Game.IngameState.Camera;
                @MapWindow = GC.Game.IngameState.IngameUi.Map;
                @MapRec = @MapWindow.GetClientRect();
                @PlayerPos = GC.Player.GetComponent<Positioned>().GridPos;
                @PlayerPosZ = GC.Player.GetComponent<Render>().Z;
                @ScreenCenter = new Vector2(@MapRec.Width / 2, @MapRec.Height / 2).Translate(0, -20)
                                   + new Vector2(@MapRec.X, @MapRec.Y)
                                   + new Vector2(@MapWindow.LargeMapShiftX, @MapWindow.LargeMapShiftY);
                @Diag = (float)Math.Sqrt(@Camera.Width * @Camera.Width + @Camera.Height * @Camera.Height);
                @K = @Camera.Width < 1024f ? 1120f : 1024f;
                @Scale = @K / @Camera.Height * @Camera.Width * 3f / 4f / @MapWindow.LargeMapZoom;
            }
        }

        private void DrawToLargeMiniMap(DelveEntity entity)
        {
            if (entity.IsOpened)
                return;

            var icon = GetMapIcon(entity);
            if (icon == null)
            {
                return;
            }

            var iconZ = entity.RendererPosZ;

            try
            {
                var point = LargeMapInformation.ScreenCenter
                            + MapIcon.TranslateGridDeltaToMapDelta((Vector2)(entity.GridPos - LargeMapInformation.PlayerPos),
                                ((float)iconZ - LargeMapInformation.PlayerPosZ));

                var size = icon.Size * 2; // icon.SizeOfLargeIcon.GetValueOrDefault(icon.Size * 2);
                // LogMessage($"{Name}: entity.Path - {entity.Path}");
                Graphics.DrawImage(icon.Texture, new RectangleF(point.X - size / 2f, point.Y - size / 2f, size, size), icon.Color);
            }
            catch (NullReferenceException) { }
        }

        private void DrawToSmallMiniMap(DelveEntity entity)
        {
            if (entity.IsOpened)
                return;
            
            var icon = GetMapIcon(entity);
            if (icon == null)
            {
                return;
            }

            var smallMinimap = GameController.Game.IngameState.IngameUi.Map.SmallMiniMap;
            var playerPos = GameController.Player.GetComponent<Positioned>().GridPos;
            var posZ = GameController.Player.GetComponent<Render>().Z;
            const float scale = 240f;
            var mapRect = smallMinimap.GetClientRect();
            var mapCenter = new Vector2(mapRect.X + mapRect.Width / 2, mapRect.Y + mapRect.Height / 2).Translate(0, 0);
            var diag = Math.Sqrt(mapRect.Width * mapRect.Width + mapRect.Height * mapRect.Height) / 2.0;
            var iconZ = entity.RendererPosZ;

            try
            {
                var point = mapCenter + MapIcon.DeltaInWorldToMinimapDelta(entity.GridPos - playerPos, diag, scale, ((float)iconZ - posZ) / 20);
                var size = icon.Size;
                var rect = new RectangleF(point.X - size / 2f, point.Y - size / 2f, size, size);
                mapRect.Contains(ref rect, out var isContain);
                if (isContain)
                {
                    // LogMessage($"{Name}: entity.Path - {entity.Path}");
                    Graphics.DrawImage(icon.Texture, rect, icon.Color);
                }
            }
            catch (NullReferenceException) { }
        }

        private void UpdateEntitys()
        {
            updateCooldown = 250;
            if (!IsAzuriteMine)
                return;
            var entitys = GameController.Entities;
            
            foreach (var entity in entitys)
            {
                if (entity == null)
                    continue;
                if (!entity.Path.StartsWith("Metadata/Chests/DelveChests/") &&
                    !entity.Path.StartsWith("Metadata/Terrain/Leagues/Delve/Objects/DelveWall") &&
                    !entity.Path.StartsWith("Metadata/Terrain/Leagues/Delve/Objects/DelveLight")) continue;
                
                var delveEntityToUpdate = DelveEntities.First(x => x != null &&
                                                                   x.Id == entity.Id);

                if (delveEntityToUpdate == null)
                    continue;
                delveEntityToUpdate.IsAlive = entity.IsAlive;
                delveEntityToUpdate.IsOpened = entity.IsOpened;
            }
        }

        public override void EntityAdded(Entity entity)
        {
            if (!IsAzuriteMine || entity == null || DelveEntities.Exists(x => x.Id == entity.Id))
                return;

            if (!entity.Path.StartsWith("Metadata/Chests/DelveChests/") &&
                !entity.Path.StartsWith("Metadata/Terrain/Leagues/Delve/Objects/DelveWall") &&
                !entity.Path.StartsWith("Metadata/Terrain/Leagues/Delve/Objects/DelveLight")) return;
            var isOpen = entity.IsOpened;
            var renderer = entity.GetComponent<Render>();
            var rendererPosZ = renderer != null ? renderer.Pos.Z : entity.Pos.Z; 
            var gridPos = entity.GridPos;
            var id = entity.Id;

            DelveEntities.Add(new DelveEntity( entity.Pos, entity.Path, isOpen, rendererPosZ, gridPos, entity.IsAlive, id));
        }

        public class FossilTiers
        {
            [JsonProperty("t1")]
            public string[] T1 { get; set; }

            [JsonProperty("t2")]
            public string[] T2 { get; set; }

            [JsonProperty("t3")]
            public string[] T3 { get; set; }
        }

        private void RenderMapImages()
        {
            if (GameController.Game.IngameState.IngameUi.Map.LargeMap.IsVisible)
            {
                LargeMapInformation = new LargeMapData(GameController);
                foreach (var entity in DelveEntities)
                {
                    if (entity is null) continue;
                    DrawToLargeMiniMap(entity);
                }
            }
            else if (GameController.Game.IngameState.IngameUi.Map.SmallMiniMap.IsVisible)
            {
                foreach (var entity in DelveEntities)
                {
                    if (entity is null) continue;
                    DrawToSmallMiniMap(entity);
                }
            }
        }

        private void DrawMineMapConnections(SubterraneanChart MineMap)
        {
            if (!MineMap.IsVisible || !Settings.DrawPaths)
                return;

            int width = 0;
            RectangleF connection_area = RectangleF.Empty;
            RectangleF MineMapArea = MineMap.GetClientRect();
            float reducedWidth = ((100 - Settings.ShowRadiusPercentage.Value) * MineMapArea.Width)/200;
            float reduceHeight = ((100 - Settings.ShowRadiusPercentage.Value) * MineMapArea.Height)/200;
            MineMapArea.Inflate(0 - reducedWidth, 0 - reduceHeight);
            foreach (var zone in MineMap.GridElement.Cells.Where(cell => cell.GetClientRect().Intersects(MineMapArea)))
            {
                foreach (var block in zone.Cells.Where(x => x.Type != ""))
                {
                    var blockRect = block.GetClientRect();
                    var blockCenter = blockRect.Center;
                    
                    if ((!block.PathIsHidden() && block.IsCellVisible() && Settings.OnlyShowHiddenPaths) || !MineMapArea.Intersects(blockRect))
                        continue;
                    
                    var connections = block.Children.Where(x => x is {IsVisible: false} && ((int)x.Width == 10 || (int)x.Width == 4)).ToArray();
                    int i = 0;
                    int count = connections.Count();

                    var biomeColor = Settings.ColoredPaths
                        ? block.BiomeName() switch
                        {
                            "AbyssalDepths" => Color.DarkGreen,
                            "AbyssalCity" => Color.DarkGreen,
                            "PetrifiedForest" => Color.SandyBrown,
                            "MagmaFissure" => Color.DarkRed,
                            "FungalCaverns" => Color.DarkViolet,
                            "FrozenHollow" => Color.DarkBlue,
                            "PrimevalRuins" => Color.Blue,
                            "VaalOutpost" => Color.Orange,
                            "SulphurVents" => Color.Brown,
                            _ => Color.Yellow
                        }
                        : Color.Yellow;
                        
                    if (Settings.DrawStraightPaths)
                    {
                        var points = new List<Vector2>();
                        if (block.Children.Any(x => x.Width == 10 && x.Y != 0))
                            points.Add(blockRect.BottomMiddle());
                        if (block.Children.Any(x => x.Width == 10 && x.Y == 0))
                            points.Add(blockRect.TopMiddle());
                        if (block.Children.Any(x => x.Width == 4 && x.X == 0))
                            points.Add(blockRect.LeftMiddle());
                        if (block.Children.Any(x => x.Width == 4 && x.X != 0))
                            points.Add(blockRect.RightMiddle());
                        
                        foreach (var connection in points)
                        {
                            Graphics.DrawLine(blockCenter, connection, 4, biomeColor);
                        }
                    }
                    else
                    {
                        foreach (var connection in connections)
                        {
                            //Graphics.DrawFrame(connection.GetClientRect(), Color.Yellow, 1);
                            int correctionOffset = 15;
                            var connectionCenter = connection.GetClientRect().Center;
                            /*
                            //This need a Zoomlevel Based Value.
                            if (connection.Width == 10)
                                connectionCenter.X += correctionOffset;
                            if (connection.Width == 4)
                                connectionCenter.Y += correctionOffset;
                            */
                            
                            if (i < count)
                            {
                                Graphics.DrawLine(connectionCenter, blockCenter, 4, biomeColor);
                            }
                            i++;
                                    
                        }
                    }
                    if (!block.IsCellVisible() && block.HasEncounter())
                    {
                        if (!MineMapArea.Contains(blockCenter))
                            continue;
                        var icon = iconStore.GetByFeatureId(block.Type());
                        if (icon == null)
                            continue;
                        var iconRect = blockRect;
                        iconRect.Inflate(-iconRect.Width / 7, -iconRect.Height / 7);
                        Graphics.DrawImage(icon, iconRect);
                    }

                }
            }
        }
    }
}
