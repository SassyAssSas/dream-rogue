using UnityEngine;

namespace Violoncello.Extensions {
   public static class Vector2Extensions{
      public static void SetX(this Vector2 vector, float value) {
         vector.x = value;
      }

      public static void SetY(this Vector2 vector, float value) {
         vector.y = value;
      }

      public static Vector2 But(this Vector2 vector, float? x = null, float? y = null) {
         return new Vector2(x ?? vector.x, y ?? vector.y);
      }
   }
}
