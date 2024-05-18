using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Violoncello.Extensions;

namespace SecretHostel.DreamRogue {
   public class PlayerQuickfallState : PlayerViewModel.State {
      private PlayerQuickfallStateConfig _config;

      private RaycastHit[] groundHitsBuffer;
      private Collider[] entityHitsBuffer;

      private PlayerQuickfallState(PlayerViewModel viewModel, PlayerQuickfallStateConfig config) : base(viewModel) {
         _config = config;

         groundHitsBuffer = new RaycastHit[1];
         entityHitsBuffer = new Collider[20];
      }

      public override void EnterState() {
         Quickfall().Forget();
      }

      private async UniTaskVoid Quickfall() {
         await UniTask.WaitForFixedUpdate();

         if (IsGroundClose(0.25f)) {
            FinishState(0);
            return;
         }

         Rigidbody.velocity = Vector3.down * _config.FallVelocity;

         await UniTask.WaitUntil(() => IsGroundClose(0.25f));

         var hitsAmount = Physics.OverlapSphereNonAlloc(ViewModel.transform.position, 1.5f, entityHitsBuffer);

         for (int i = 0; i < hitsAmount; i++) {
            if (entityHitsBuffer[i].gameObject == ViewModel.gameObject) {
               continue;
            }

            if (!entityHitsBuffer[i].TryGetComponent(out Rigidbody rigigbody)) {
               continue;
            }

            var forceDirection = rigigbody.transform.position - ViewModel.transform.position;
            forceDirection = forceDirection.But(y: 0f)
                                           .normalized
                                           .But(y: _config.EntityKnockbackYVelocity);

            rigigbody.AddForce(forceDirection * _config.EntityKnockbackMagnitude, ForceMode.Impulse);
         }

         FinishState(0);
      }

      private bool IsGroundClose(float checkDistance) {
         var origin = ViewModel.transform.position;
         origin.y -= ViewModel.transform.localScale.y / 2 - 0.01f;

         var hitsAmount = Physics.RaycastNonAlloc(new Ray(origin, Vector3.down), groundHitsBuffer, checkDistance, 1 << 3);

         return hitsAmount > 0;
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
