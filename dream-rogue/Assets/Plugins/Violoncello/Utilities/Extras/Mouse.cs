using UnityEngine;
using Violoncello.Optimization;

namespace Violoncello.Utilities {
   public static class Mouse {
      private static FrameCache<Vector2> _cachedPosition;
      public static Vector2 Position { get => GetRelativePosition(Camera.main); }

      public static Vector2 GetRelativePosition(Camera camera) {
         return _cachedPosition.Expired
            ? camera.ScreenToWorldPoint(Input.mousePosition)
            : _cachedPosition.Value;
      }
   }
}
