using System;
using ExileCore.PoEMemory.MemoryObjects;
using SharpDX;

namespace DefaultNamespace;

public static class Helper
{
    private static long _lastTime;
    internal static float _deltaTime;

    public static void GetDeltaTime()
    {
        var now = DateTime.Now.Ticks;
        // ReSharper disable once PossibleLossOfFraction
        float dT = (now - _lastTime) / 10000;
        _lastTime = now;
        _deltaTime = dT;
    }
    
    internal static Random random = new Random();

    internal static float MoveTowards(float cur, float tar, float max)
    {
        if (Math.Abs(tar - cur) <= max)
            return tar;
        return cur + Math.Sign(tar - cur) * max;
    }
}