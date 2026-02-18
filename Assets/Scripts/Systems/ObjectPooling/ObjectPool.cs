using System.Collections.Generic;
using UnityEngine;

namespace Tsarkel.Systems.ObjectPooling
{
    /// <summary>
    /// Generic object pool for efficient object reuse.
    /// Prevents garbage collection spikes by reusing objects instead of instantiating/destroying.
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        [Header("Pool Settings")]
        [Tooltip("Prefab to pool")]
        [SerializeField] private GameObject prefab;
        
        [Tooltip("Initial pool size")]
        [SerializeField] private int initialSize = 10;
        
        [Tooltip("Maximum pool size (0 = unlimited)")]
        [SerializeField] private int maxSize = 0;
        
        [Tooltip("Whether to expand pool if empty")]
        [SerializeField] private bool allowExpansion = true;
        
        private Queue<GameObject> pool = new Queue<GameObject>();
        private List<GameObject> activeObjects = new List<GameObject>();
        
        /// <summary>
        /// Number of objects currently in the pool.
        /// </summary>
        public int PoolSize => pool.Count;
        
        /// <summary>
        /// Number of objects currently active.
        /// </summary>
        public int ActiveCount => activeObjects.Count;
        
        private void Awake()
        {
            if (prefab == null)
            {
                Debug.LogError("ObjectPool: Prefab is not assigned!");
                return;
            }
            
            // Pre-populate pool
            for (int i = 0; i < initialSize; i++)
            {
                CreatePooledObject();
            }
        }
        
        /// <summary>
        /// Gets an object from the pool.
        /// </summary>
        /// <param name="position">Position to spawn at</param>
        /// <param name="rotation">Rotation to spawn with</param>
        /// <returns>Pooled object, or null if pool is empty and expansion is disabled</returns>
        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            GameObject obj = null;
            
            // Try to get from pool
            if (pool.Count > 0)
            {
                obj = pool.Dequeue();
            }
            else if (allowExpansion)
            {
                // Create new object if pool is empty and expansion is allowed
                obj = CreatePooledObject();
            }
            
            if (obj != null)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.SetActive(true);
                activeObjects.Add(obj);
            }
            
            return obj;
        }
        
        /// <summary>
        /// Returns an object to the pool.
        /// </summary>
        /// <param name="obj">Object to return</param>
        public void Return(GameObject obj)
        {
            if (obj == null) return;
            
            if (activeObjects.Contains(obj))
            {
                activeObjects.Remove(obj);
            }
            
            obj.SetActive(false);
            
            // Only return to pool if under max size
            if (maxSize == 0 || pool.Count < maxSize)
            {
                pool.Enqueue(obj);
            }
            else
            {
                // Destroy if pool is full
                Destroy(obj);
            }
        }
        
        /// <summary>
        /// Creates a new pooled object.
        /// </summary>
        private GameObject CreatePooledObject()
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            pool.Enqueue(obj);
            return obj;
        }
        
        /// <summary>
        /// Returns all active objects to the pool.
        /// </summary>
        public void ReturnAll()
        {
            while (activeObjects.Count > 0)
            {
                Return(activeObjects[0]);
            }
        }
        
        /// <summary>
        /// Clears the pool and destroys all objects.
        /// </summary>
        public void Clear()
        {
            ReturnAll();
            
            while (pool.Count > 0)
            {
                GameObject obj = pool.Dequeue();
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
        }
    }
}
