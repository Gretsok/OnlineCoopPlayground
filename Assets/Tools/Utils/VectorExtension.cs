using UnityEngine;

namespace Tools.Utils
{
    public static class VectorExtension
    {
        public static Vector3 Flatten(this Vector3 a_vector, float a_height = 0)
        {
            return new Vector3(a_vector.x, a_height, a_vector.z);
        }
    }
}
