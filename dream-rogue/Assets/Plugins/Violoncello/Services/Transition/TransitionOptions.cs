using System.Collections.Generic;

namespace Violoncello.Services {
   public class TransitionOptions {
      internal string ExitAnimationName { get; set; }
      internal string EntranceAnimationName { get; set; }

      internal Dictionary<string, object> Extras { get; set; }

      internal TransitionIntermediateLoadingSceneOptions IntermediateLoadingSceneOptions { get; set; }

      public static TransitionOptions SimpleFade => new() {
         ExitAnimationName = TransitionAnimations.Fade,
         EntranceAnimationName = TransitionAnimations.Fade
      };

      internal TransitionOptions() {

      }
   }
}
