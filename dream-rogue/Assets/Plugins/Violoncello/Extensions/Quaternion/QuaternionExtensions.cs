using UnityEngine;

namespace Violoncello.Extensions {
   public static class QuaternionExtensions {
      public static Quaternion ButEuler(this Quaternion quatertion, float? x = null, float? y = null, float? z = null) {
         return Quaternion.Euler(x ?? quatertion.x, y ?? quatertion.y, z ?? quatertion.z);
      }
   }
}
