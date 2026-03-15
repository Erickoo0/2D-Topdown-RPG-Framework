using UnityEngine;

public class ItemUsableObject : MonoBehaviour, IUsable
{
    public void Use()
    {
        Debug.unityLogger.Log("Haa! item Thrown");
    }
}
