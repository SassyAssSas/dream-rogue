using Cysharp.Threading.Tasks;
using UnityEngine;
using Violoncello.Utilities;
using UnityEngine.SceneManagement;

namespace Violoncello.Services.Generic {
   public class MonoTransitionService : MonoBehaviour, ITransitionService {
      private Animator _animator;

      public float CurrentTransitionProgress { get; private set; }
      public bool IsTransitioning { get; private set; }

      private string _targetSceneName;
      private TransitionOptions _transitionOptions;

      private void Awake() {
         _animator = GetComponent<Animator>();
         _transitionOptions = new();
      }

      public void Transition(string targetSceneName, TransitionOptions options = null) {
         Assert.That(!IsTransitioning)
               .Throws("Attempted to start a transition whilst the previous one hasn't finished yet");

         IsTransitioning = true;

         _targetSceneName = targetSceneName;
         _transitionOptions = options ?? new();

         BeginTransitionNextFrameAsync().Forget();
      }

      public T GetExtra<T>(string key) where T : class {
         Assert.That(_transitionOptions.Extras.TryGetValue(key, out object value))
               .Throws($"Couldn't find extra data with the {key} key");

         var unboxedValue = value as T;

         Assert.That(unboxedValue != null)
               .Throws($"Couldn't unbox extra data with the {key} key to {typeof(T)} type.");

         return unboxedValue;
      }

      private async UniTaskVoid BeginTransitionNextFrameAsync() {
         await UniTask.NextFrame();

         if (_transitionOptions.IntermediateLoadingSceneOptions != null) {
            await PerformTransitionAsync();
         }
         else {
            await PerformNoLoadingSceneTransitionAsync();
         }
      }

      private async UniTask PerformTransitionAsync() {
         var loadingSceneLoadAsyncOperation = BeginDefaultLoadSceneAsync(_transitionOptions.IntermediateLoadingSceneOptions.LoadingSceneName, LoadSceneMode.Single);
         var targetSceneLoadAsyncOperation = BeginDefaultLoadSceneAsync(_targetSceneName, LoadSceneMode.Single);
         
         await TryPlayExitAnimationAsync(_transitionOptions.ExitAnimationName);

         loadingSceneLoadAsyncOperation.allowSceneActivation = true;

         await TryPlayEntranceAnimationAsync(_transitionOptions.IntermediateLoadingSceneOptions.EntranceAnimationName);

         await UpdateProgressUntilSceneIsReadyAsync(targetSceneLoadAsyncOperation);

         await TryPlayExitAnimationAsync(_transitionOptions.IntermediateLoadingSceneOptions.ExitAnimationName);
         
         targetSceneLoadAsyncOperation.allowSceneActivation = true;

         IsTransitioning = false;

         await TryPlayEntranceAnimationAsync(_transitionOptions.EntranceAnimationName);
      }

      private async UniTask PerformNoLoadingSceneTransitionAsync() {
         var targetSceneLoadAsyncOperation = BeginDefaultLoadSceneAsync(_targetSceneName, LoadSceneMode.Single);

         await TryPlayExitAnimationAsync(_transitionOptions.ExitAnimationName);

         await UpdateProgressUntilSceneIsReadyAsync(targetSceneLoadAsyncOperation);

         targetSceneLoadAsyncOperation.allowSceneActivation = true;

         IsTransitioning = false;

         await TryPlayEntranceAnimationAsync(_transitionOptions.EntranceAnimationName);
      }

      private async UniTask UpdateProgressUntilSceneIsReadyAsync(AsyncOperation operation) {
         do {
            CurrentTransitionProgress = operation.progress;

            await UniTask.NextFrame();
         } while(operation.progress < 0.9f);
      }

      private async UniTask TryPlayEntranceAnimationAsync(string animationName) {
         if (string.IsNullOrEmpty(animationName)) {
            return;
         }

         await PlayAnimationAsync(animationName + "Entrance");
      }

      private async UniTask TryPlayExitAnimationAsync(string animationName) {
         if (string.IsNullOrEmpty(animationName)) {
            return;
         }

         await PlayAnimationAsync(animationName + "Exit");
      }

      private async UniTask PlayAnimationAsync(string animationName) {
         _animator.Play(animationName);

         await UniTask.NextFrame();
         await UniTask.WaitForSeconds(_animator.GetCurrentAnimatorClipInfo(0).Length);
      }

      public AsyncOperation BeginDefaultLoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode) {
         var operation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
         operation.allowSceneActivation = false;

         return operation;
      }
   }
}