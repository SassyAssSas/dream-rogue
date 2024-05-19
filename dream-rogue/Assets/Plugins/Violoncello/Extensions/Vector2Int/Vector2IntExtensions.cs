using UnityEngine;

namespace Violoncello.Extensions {
   public static class Vector2ExtIntensions {
      public static Vector2Int But(this Vector2Int vector, int? x = null, int? y = null) {
         return new Vector2Int(x ?? vector.x, y ?? vector.y);
      }

      public static Vector3 ToXZYVector3(this Vector2Int original) {
         return new Vector3(original.x, 0f, original.y);
      }

      public static Vector3Int ToXZYVector3Int(this Vector2Int original) {
         return new Vector3Int(original.x, 0, original.y);
      }
   }
}
