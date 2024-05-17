using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Violoncello.Extensions;

namespace SecretHostel.DreamRogue {
   public class PlayerDashState : PlayerViewModel.State {
      private PlayerDashStateConfig _config;

      private PlayerInput input;

      private CancellationTokenSource cts;

      private PlayerDashState(PlayerViewModel viewModel, PlayerDashStateConfig config) : base(viewModel) {
         _config = config;

         input = new();
         input.Enable();
      }

      public override void EnterState() {
         cts = new();

         DashAsync(cts.Token).Forget();
      }

      public override void QuitState() {
         if (cts != null) {
            cts.Cancel();
            cts.Dispose();

            cts = null;
         }
      }

      public override void Tick(float deltaTime) {
         if (input.Player.Quickfall.inProgress) {
            FinishState(1);
         }
      }

      private async UniTaskVoid DashAsync(CancellationToken cancellationToken) {
         var startPosition = ViewModel.transform.position;
         var targetPosition = ViewModel.transform.position + ViewModel.transform.forward * _config.DashDistance;

         var elapsedTime = 0f;

         while (elapsedTime < 1f) {
            if (cancellationToken.IsCancellationRequested) {
               return;
            }

            elapsedTime += Time.deltaTime / _config.DashTimeSeconds;

            ViewModel.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime);

            await UniTask.NextFrame();
         }

         Rigidbody.velocity = Rigidbody.velocity.But(y: -0.1f);

         FinishState(0);
      }

      public class Factory {
         private PlayerDashStateConfig _config;

         public Factory(PlayerDashStateConfig config) {
            _config = config;
         }

         public PlayerDashState Create(PlayerViewModel viewModel) {
            return new PlayerDashState(viewModel, _config);
         }
      }
   }
}
