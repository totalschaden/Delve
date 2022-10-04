

using SharpDX;

namespace Delve.Libs;

public class DelveEntity
{
    public DelveEntity(Vector3 pos, string path, bool isOpened, Vector3 rendererPos, Vector2 gridPos, bool isAlive)
    {
        Pos = pos;
        Path = path;
        IsOpened = isOpened;
        RendererPos = rendererPos;
        GridPos = gridPos;
        IsAlive = isAlive;
    }

    public Vector3 Pos { get; set; }
    
    public Vector3? RendererPos { get; set; }
    
    public Vector2 GridPos { get; set; }
    public string Path { get; set; }
    public bool IsOpened { get; set; }
    
    public bool IsAlive { get; set; }
    
    
}