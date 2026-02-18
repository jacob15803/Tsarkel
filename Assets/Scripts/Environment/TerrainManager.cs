using UnityEngine;

namespace Tsarkel.Environment
{
    /// <summary>
    /// Manages terrain and environment.
    /// Can be extended for terrain deformation in the future.
    /// </summary>
    public class TerrainManager : MonoBehaviour
    {
        [Header("Terrain Reference")]
        [Tooltip("Terrain component reference")]
        [SerializeField] private Terrain terrain;
        
        /// <summary>
        /// Gets the terrain height at a given world position.
        /// </summary>
        public float GetTerrainHeight(Vector3 worldPosition)
        {
            if (terrain != null)
            {
                return terrain.SampleHeight(worldPosition);
            }
            
            // Fallback: raycast to find ground
            RaycastHit hit;
            if (Physics.Raycast(worldPosition + Vector3.up * 100f, Vector3.down, out hit, 200f))
            {
                return hit.point.y;
            }
            
            return worldPosition.y;
        }
        
        /// <summary>
        /// Gets the terrain normal at a given world position.
        /// </summary>
        public Vector3 GetTerrainNormal(Vector3 worldPosition)
        {
            if (terrain != null)
            {
                // Get terrain data
                TerrainData terrainData = terrain.terrainData;
                
                // Convert world position to terrain local coordinates
                Vector3 localPos = worldPosition - terrain.transform.position;
                Vector3 normalizedPos = new Vector3(
                    localPos.x / terrainData.size.x,
                    0f,
                    localPos.z / terrainData.size.z
                );
                
                // Get normal from terrain
                return terrainData.GetInterpolatedNormal(normalizedPos.x, normalizedPos.z);
            }
            
            // Fallback: raycast to find normal
            RaycastHit hit;
            if (Physics.Raycast(worldPosition + Vector3.up * 100f, Vector3.down, out hit, 200f))
            {
                return hit.normal;
            }
            
            return Vector3.up;
        }
        
        // Future: Terrain deformation methods can be added here
        // public void DeformTerrain(Vector3 position, float radius, float strength) { }
    }
}
