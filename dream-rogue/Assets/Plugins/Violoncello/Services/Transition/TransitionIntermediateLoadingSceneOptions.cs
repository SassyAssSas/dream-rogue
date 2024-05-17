using Violoncello.Utilities;

namespace Violoncello.Services {
   internal class TransitionIntermediateLoadingSceneOptions {
      public string LoadingSceneName { get; }

      public string EntranceAnimationName { get; set; }
      public string ExitAnimationName { get; set; }

      public TransitionIntermediateLoadingSceneOptions(string loadingSceneName) {
         Assert.That(!string.IsNullOrEmpty(loadingSceneName))
               .Throws("Loading scene name was null or empty.");

         LoadingSceneName = loadingSceneName;
      }
   }
}
