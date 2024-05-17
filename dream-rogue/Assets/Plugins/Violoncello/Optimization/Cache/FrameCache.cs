using UnityEngine;

namespace Violoncello.Optimization {
   public struct FrameCache<TCached> {
      public readonly bool Expired => lastUpdateTime != Time.timeAsDouble;

      public TCached Value { get; }

      private double lastUpdateTime;

      public FrameCache(TCached value) {
         Value = value;

         lastUpdateTime = lastUpdateTime = Time.timeAsDouble;
      }
   }
}
