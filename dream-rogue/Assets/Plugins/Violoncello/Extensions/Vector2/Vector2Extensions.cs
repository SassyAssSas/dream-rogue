using UnityEngine;

namespace Violoncello.Extensions {
   public static class Vector2Extensions {
      public static Vector2 But(this Vector2 vector, float? x = null, float? y = null) {
         return new Vector2(x ?? vector.x, y ?? vector.y);
      }

      public static Vector3 ToXZYVector3(this Vector2 original) {
         return new Vector3(original.x, 0f, original.y);
      }
   }
}
