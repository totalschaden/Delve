using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using ExileCore.Shared.AtlasHelper;
using Newtonsoft.Json;

namespace DefaultNamespace;
public record ChartFeature(string Id, string Name, string Description, string Image);

[SuppressMessage("Interoperability", "CA1416:Plattformkompatibilität überprüfen")]
public class IconStore
{
    private readonly Dictionary<string, ChartFeature> _features;
    private readonly AtlasTexturesProcessor _textures;

    public IconStore(AtlasTexturesProcessor textures, string featureDescriptionPath)
    {
        _textures = textures;
        _features = JsonConvert.DeserializeObject<List<ChartFeature>>(File.ReadAllText(featureDescriptionPath)).ToDictionary(x => x.Id);
    }

    public AtlasTexture GetByFeatureId(string id)
    {
        if (!_features.TryGetValue(id, out var feature) || string.IsNullOrEmpty(feature.Image))
        {
            return null;
        }

        return _textures.GetTextureByName(feature.Image.Replace("Art/2DArt/UIImages/InGame/Delve/MapEncounters/", null));
    }
}