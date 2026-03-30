using UnityEngine;

public static class GlobalHelper
{
    public static float _animationFPS = 10f; // Adjust to change how fast all items animate
    
    public static string GenerateUniqueID(GameObject obj)
    {
        // Generates a Unique ID using the objects name and game objects position combined
        return ($"{obj.scene.name}_{obj.transform.position.x}_{obj.transform.position.y}");
    }
    
    public static Sprite GetAnimatedSprite(ItemData itemData)
    {
        // Safety Check
        if (itemData == null) return null;

        // Animation Logic, if not animated, method will return frameIndex 0
        int frameIndex = Mathf.FloorToInt(Time.time * _animationFPS) % itemData.ItemIcon.Length;
        return itemData.ItemIcon[frameIndex];
    }
}
