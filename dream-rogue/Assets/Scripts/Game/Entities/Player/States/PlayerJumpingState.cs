using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Violoncello.Extensions;

namespace SecretHostel.DreamRogue {
   public class PlayerJumpingState : PlayerViewModel.State {
      private PlayerJumpingStateConfig _config;

      private PlayerInput input;

      private RaycastHit[] groundHitsBuffer;

      private CancellationTokenSource cts;

      private bool canDash = false;

      private PlayerJumpingState(PlayerViewModel viewModel, PlayerJumpingStateConfig config) : base(viewModel) {
         _config = config;

         input = new();
         input.Enable();

         groundHitsBuffer = new RaycastHit[1];
      }

      public override void EnterState() {
         cts = new();

         JumpAsync(cts.Token).Forget();
      }

      public override void QuitState() {
         cts.Cancel();
         cts.Dispose();
      }

      public override void Tick(float deltaTime) {
         if (input.Player.Dash.inProgress && canDash) {
            FinishState(1);
            return;
         }

         if (input.Player.Quickfall.inProgress) {
            FinishState(2);
         }

         CalculateRotation(deltaTime);
      }

      public override void FixedTick(float fixedDeltaTime) {
         CalculateMotion(fixedDeltaTime);
      }

      private async UniTaskVoid JumpAsync(CancellationToken cancellationToken) {
         if (!IsGroundClose(_config.PrepareJumpGroundDistance)) {
            FinishState(0);
            return;
         }

         if (Rigidbody.velocity.y > 0f) {
            FinishState(0);
            return;
         }

         canDash = false;

         await UniTask.WaitUntil(() => IsGroundClose(_config.JumpGroundDistance), cancellationToken: cancellationToken);
         if (cancellationToken.IsCancellationRequested) {
            return;
         }

         canDash = true;

         Rigidbody.velocity = Rigidbody.velocity.But(y: _config.JumpForce);

         await UniTask.WaitUntil(() => IsGroundClose(_config.JumpGroundDistance) && Rigidbody.velocity.y < 0f, cancellationToken: cancellationToken);
         if (cancellationToken.IsCancellationRequested) {
            return;
         }

         FinishState(0);
      }

      private bool IsGroundClose(float checkDistance) {
         var origin = ViewModel.transform.position;
         origin.y -= ViewModel.transform.localScale.y / 2 - 0.01f;

         var hitsAmount = Physics.RaycastNonAlloc(new Ray(origin, Vector3.down), groundHitsBuffer, checkDistance, 1 << 3);

         return hitsAmount > 0;
      }

      private void CalculateRotation(float deltaTime) {
         var direction = input.Player.Walking.ReadValue<Vector3>();
         direction.y = 0f;

         if (direction == Vector3.zero) {
            return;
         }

         if (ViewModel.transform.rotation.eulerAngles.y == 180 && direction == Vector3.forward) {
            var rotation = ViewModel.transform.rotation;

            rotation.eulerAngles = rotation.eulerAngles.But(y: 179);

            ViewModel.transform.rotation = rotation;
         }

         var forward = Vector3.RotateTowards(ViewModel.transform.forward, direction, _config.RotationSpeed * deltaTime, 1f);

         ViewModel.transform.forward = forward;
      }

      private void CalculateMotion(float fixedDeltaTime) {
         var direction = input.Player.Walking.ReadValue<Vector3>();
         direction.y = 0f;

         if (direction == Vector3.zero) {
            var zeroVelocity = Vector3.zero;
            zeroVelocity.y = Rigidbody.velocity.y;

            Rigidbody.velocity = zeroVelocity;

            return;
         }

         var speedModifierT = (Vector3.Dot(direction, ViewModel.transform.forward) + 1f) * 0.05f;
         var speedModifier = Mathf.Lerp(0.5f, 1f, speedModifierT);

         var velocity = _config.HorizontalMovementSpeed * speedModifier * ViewModel.transform.forward;
         velocity.y = Rigidbody.velocity.y;

         Rigidbody.velocity = velocity;
      }

      public class Factory {
         private PlayerJumpingStateConfig _config;

         public Factory(PlayerJumpingStateConfig config) {
            _config = config;
         }

         public PlayerJumpingState Create(PlayerViewModel viewModel) {
            return new PlayerJumpingState(viewModel, _config);
         }
      }
   }
}
