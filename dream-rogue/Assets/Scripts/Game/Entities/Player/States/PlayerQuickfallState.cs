using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SecretHostel.DreamRogue {
   public class PlayerQuickfallState : PlayerViewModel.State {
      private PlayerQuickfallStateConfig _config;

      private RaycastHit[] groundHitsBuffer;

      private PlayerQuickfallState(PlayerViewModel viewModel, PlayerQuickfallStateConfig config) : base(viewModel) {
         _config = config;

         groundHitsBuffer = new RaycastHit[1];
      }

      public override void EnterState() {
         Quickfall().Forget();
      }

      private async UniTaskVoid Quickfall() {
         await UniTask.WaitForFixedUpdate();

         Rigidbody.velocity = Vector3.down * _config.FallVelocity;

         FinishState(0);
      }

      public class Factory {
         private PlayerQuickfallStateConfig _config;

         public Factory(PlayerQuickfallStateConfig config) {
            _config = config;
         }

         public PlayerQuickfallState Create(PlayerViewModel viewModel) {
            return new PlayerQuickfallState(viewModel, _config);
         }
      }
   }
}
