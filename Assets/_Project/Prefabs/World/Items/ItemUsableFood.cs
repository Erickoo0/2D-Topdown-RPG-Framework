using UnityEngine;

public class ItemUsableFood : MonoBehaviour, IUsable
{
    public void Use()
    {
        Debug.unityLogger.Log("Chomp chomp, ate food!");
        Destroy(gameObject);
    }
}
