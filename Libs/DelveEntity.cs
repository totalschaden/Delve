

using SharpDX;

namespace Delve.Libs;

public class DelveEntity
{
    public DelveEntity(Vector3 pos, string path, bool isOpened, float rendererPosZ, Vector2 gridPos, bool isAlive, uint id)
    {
        Pos = pos;
        Path = path;
        IsOpened = isOpened;
        RendererPosZ = rendererPosZ;
        GridPos = gridPos;
        IsAlive = isAlive;
        Id = id;
    }

    public Vector3 Pos { get; set; }
    
    public float RendererPosZ { get; set; }
    
    public Vector2 GridPos { get; set; }
    public string Path { get; set; }
    public bool IsOpened { get; set; }
    
    public bool IsAlive { get; set; }
    
    public uint Id { get; set; }
    
    
}