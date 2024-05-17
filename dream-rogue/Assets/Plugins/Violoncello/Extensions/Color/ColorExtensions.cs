using UnityEngine;

namespace Violoncello.Extensions {
   public static class ColorExtensions {
      public static void SetR(this Color color, float value) {
         color.r = value;
      }

      public static void SetG(this Color color, float value) {
         color.g = value;
      }

      public static void SetB(this Color color, float value) {
         color.b = value;
      }

      public static void SetA(this Color color, float value) {
         color.a = value;
      }

      public static Color But(this Color color, float? r = null, float? g = null, float? b = null, float? a = null) {
         return new Color(r ?? color.r, g ?? color.g, b ?? color.b, a ?? color.a);
      }
   }
}
