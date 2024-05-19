using UnityEngine;

namespace Violoncello.Extensions {
   public static class Vector3IntExtensions {
      public static Vector3Int But(this Vector3Int vector, int? x = null, int? y = null, int? z = null) {
         return new Vector3Int(x ?? vector.x, y ?? vector.y, z ?? vector.z);
      }

      public static Vector2Int ToXZVector2Int(this Vector3Int original) {
         return new Vector2Int(original.x, original.z);
      }

      public static Vector2 ToXZVector2(this Vector3Int original) {
         return new Vector2(original.x, original.z);
      }
   }
}
