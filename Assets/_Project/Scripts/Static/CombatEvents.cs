using System;
using UnityEngine;

public static class CombatEvents
{
    // Signals when a floating text gets requested
    public static event Action<int, Vector3> OnFloatingTextRequested;
    // Any script can call this method to request a floating number
    public static void RequestFloatingText(int amount, Vector3 position)
    {
        OnFloatingTextRequested?.Invoke(amount, position);
    }
}