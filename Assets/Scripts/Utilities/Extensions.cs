using UnityEngine;

namespace Tsarkel.Utilities
{
    /// <summary>
    /// Extension methods for common operations.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Checks if a float is approximately zero.
        /// </summary>
        public static bool IsApproximatelyZero(this float value, float threshold = 0.01f)
        {
            return Mathf.Abs(value) < threshold;
        }
        
        /// <summary>
        /// Clamps a vector's magnitude.
        /// </summary>
        public static Vector3 ClampMagnitude(this Vector3 vector, float maxMagnitude)
        {
            if (vector.magnitude > maxMagnitude)
            {
                return vector.normalized * maxMagnitude;
            }
            return vector;
        }
        
        /// <summary>
        /// Gets a random point on a circle.
        /// </summary>
        public static Vector3 RandomPointOnCircle(float radius)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
        }
        
        /// <summary>
        /// Gets a random point in a circle.
        /// </summary>
        public static Vector3 RandomPointInCircle(float radius)
        {
            Vector2 randomCircle = Random.insideUnitCircle * radius;
            return new Vector3(randomCircle.x, 0f, randomCircle.y);
        }
    }
}
