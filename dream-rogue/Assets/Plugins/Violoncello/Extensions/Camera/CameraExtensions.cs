using System.Collections.Generic;
using UnityEngine;
using Violoncello.Optimization;

namespace Violoncello.Extensions {
   public static class CameraExtensions {
      private static Dictionary<Camera, FrameCache<Bounds>> cachedBounds = new();

      public static Bounds GetBounds(this Camera camera) {
         var hadCache = cachedBounds.TryGetValue(camera, out FrameCache<Bounds> cache);

         if (!hadCache || cache.Expired) {
            var cameraHeight = camera.orthographicSize * 2f;
            var cameraWidth = cameraHeight * camera.aspect;

            var bounds = new Bounds(camera.transform.position, new Vector3(cameraWidth, cameraHeight, float.PositiveInfinity));

            cachedBounds[camera] = new FrameCache<Bounds>(bounds);

            return bounds;
         }

         return cache.Value;
      }

      public static Vector2 GetRandomOffBoundsPosition(this Camera camera, float distanceFromEdge) {
         var cameraBounds = camera.GetBounds();

         var randomizeX = GetRandomBool();

         if (randomizeX) {
            return cameraBounds.center + new Vector3(
               x: Random.Range(-cameraBounds.extents.x, cameraBounds.extents.x),
               y: (cameraBounds.extents.y + distanceFromEdge) * GetRandomSide()
            );
         }

         return cameraBounds.center + new Vector3(
            x: (cameraBounds.extents.x + distanceFromEdge) * GetRandomSide(),
            y: Random.Range(-cameraBounds.extents.y, cameraBounds.extents.y)
         );
      }

      private static int GetRandomSide() {
         return Random.Range(0, 2) == 0 ? 1 : -1;
      }

      private static bool GetRandomBool() {
         return Random.Range(0, 2) == 0;
      }
   }
}
