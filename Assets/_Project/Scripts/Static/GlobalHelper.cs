using UnityEngine;

public static class GlobalHelper
{
    public static string GenerateUniqueID(GameObject obj)
    {
        // Generates a Unique ID using the objects name and game objects position combined
        return ($"{obj.scene.name}_{obj.transform.position.x}_{obj.transform.position.y}");
    }
}
