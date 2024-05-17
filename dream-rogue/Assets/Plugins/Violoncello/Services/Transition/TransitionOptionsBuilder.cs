using System.Collections.Generic;
using UnityEngine;
using Violoncello.Utilities;

namespace Violoncello.Services {
   public class TransitionOptionsBuilder {
      private string entranceAnimationName;
      private string exitAnimationName;

      private Dictionary<string, object> extras = new();

      private string loadingSceneName;
      private string loadingSceneEntranceAnimationName;
      private string loadingSceneExitAnimationName;

      public TransitionOptions Build() {
         var options = new TransitionOptions() {
            EntranceAnimationName = entranceAnimationName,
            ExitAnimationName = exitAnimationName,
            Extras = new Dictionary<string, object>(extras)
         };

         if (loadingSceneName != null) {
            options.IntermediateLoadingSceneOptions = new(loadingSceneName) {
               EntranceAnimationName = loadingSceneEntranceAnimationName,
               ExitAnimationName = loadingSceneExitAnimationName
            };
         }

         return options;
      }

      public TransitionOptionsBuilder WithExtraData(string key, object data) {
         Assert.That(extras.TryAdd(key, data))
               .Throws($"Extra data with the {key} key already exists.");

         return this;
      }

      public TransitionOptionsBuilder WithEntranceAnimation(string animationName) {
         entranceAnimationName = animationName;

         return this;
      }

      public TransitionOptionsBuilder WithExitAnimation(string animationName) {
         exitAnimationName = animationName;

         return this;
      }

      public TransitionOptionsBuilder WithIntermediateLoadingScene(string sceneName, string entranceAnimationName = null, string exitAnimationName = null) {
         loadingSceneName = sceneName;
         loadingSceneEntranceAnimationName = entranceAnimationName;
         loadingSceneExitAnimationName = exitAnimationName;

         return this;
      }
   }
}
