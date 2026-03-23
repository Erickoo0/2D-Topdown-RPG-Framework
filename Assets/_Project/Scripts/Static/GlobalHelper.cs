using UnityEngine;

public static class GlobalHelper
{
    public static string GenerateUniqueID(GameObject obj)
    {
        // Generates a Unique ID using the objects name and game objects position combined
        return ($"{obj.scene.name}_{obj.transform.position.x}_{obj.transform.position.y}");
    }
    
    public static Sprite GetAnimatedSprite (ItemData itemData, float speed = 5f)
    {
        // Safety Check
        if (itemData == null || itemData.itemIconAnimated == null || itemData.itemIconAnimated.Length == 0) return null;
        
        // Calculate current frame using time and modulo
        int index = Mathf.FloorToInt(Time.time * speed) % itemData.itemIconAnimated.Length;  
        return itemData.itemIconAnimated[index];
    }
    
    
}
