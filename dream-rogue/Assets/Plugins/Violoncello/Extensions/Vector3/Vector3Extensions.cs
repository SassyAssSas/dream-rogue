using UnityEngine;

namespace Violoncello.Extensions {
   public static class Vector3Extensions {
      public static Vector3 But(this Vector3 vector, float? x = null, float? y = null, float? z = null) {
         return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
      }

      public static Vector2 ToXZVector2(this Vector3 original) {
         return new Vector2(original.x, original.z);
      }
   }
}
