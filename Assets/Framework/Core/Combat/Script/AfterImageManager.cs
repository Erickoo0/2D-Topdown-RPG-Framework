using UnityEngine;
using System.Collections.Generic;

public class AfterImageManager : MonoBehaviour
{
   public static AfterImageManager Instance { get; private set; }
   
   [SerializeField] private AfterImage afterImagePrefab;
   [SerializeField] private int poolSize = 10;
   
   private readonly Queue<AfterImage> _afterImagePool = new Queue<AfterImage>();

   private void Awake()
   {
      if (Instance != null && Instance != this)
      {
         Destroy(gameObject);
         return;
      }
      
      Instance = this;
      
      // Pre-create the object pool
      for (int i = 0; i < poolSize; i++)
      {
         AfterImage afterImage = CreateAfterImage();
         _afterImagePool.Enqueue(afterImage);
      }
   }

   public void SpawnAfterImage(Sprite sprite, Vector3 position)
   {
      AfterImage afterImage;

      // Check if Queue has available objects
      if (_afterImagePool.Count > 0)
      {
         afterImage = _afterImagePool.Dequeue();
      }
      else // If the Queue is empty (too many after images on screen), make a new object
      {
         afterImage = CreateAfterImage();
      }

      afterImage.Initialize(sprite, position, ReturnToPool);
   }

   private void ReturnToPool(AfterImage afterImage)
   {
      afterImage.gameObject.SetActive(false);
      _afterImagePool.Enqueue(afterImage);
   }
   
   private AfterImage CreateAfterImage()
   {
      AfterImage afterImage = Instantiate(afterImagePrefab, transform);
      afterImage.gameObject.SetActive(false);
      return afterImage;
   }
}
