using UnityEngine;
using System;

public static class CombatEvents
{
    public static event Action<int, Vector3> OnFloatingNumberSpawnRequested;
    
    // Called from requesters to spawn a floating number
    public static void RequestingFloatingNumberSpawn(int amount, Vector3 position)
    {
        OnFloatingNumberSpawnRequested?.Invoke(amount, position);
    }
}
