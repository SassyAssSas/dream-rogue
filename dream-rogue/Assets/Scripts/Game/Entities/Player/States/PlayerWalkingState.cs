using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using Violoncello.Extensions;

namespace SecretHostel.DreamRogue {
   public class PlayerWalkingState : PlayerViewModel.State {
      private PlayerWalkingStateConfig _config;

      private PlayerInput input;
      
      private PlayerWalkingState(PlayerViewModel viewModel, PlayerWalkingStateConfig config) : base(viewModel) {
	      _config = config;

         input = new();
         input.Enable();
      }

      public override void Tick(float deltaTime) {
         if (input.Player.Quickfall.inProgress) {
            FinishState(0);
            return;
         }

         if (input.Player.Jump.inProgress) {
            FinishState(1);
            return;
         }

         CalculateRotation(deltaTime);
      }

      public override void FixedTick(float fixedDeltaTime) {
         CalculateMotion(fixedDeltaTime);
      }
      
      private void CalculateRotation(float deltaTime) {
         var direction = input.Player.Walking.ReadValue<Vector3>();
         direction.y = 0f;

         if (direction == Vector3.zero) {
            return;
         }

         var dot = Mathf.Round(Vector3.Dot(ViewModel.transform.forward, direction)); // ROUND IS IMPORTANT

         if (dot == -1f) {
            var newY = ViewModel.transform.rotation.eulerAngles.y + 1;
            ViewModel.transform.rotation = ViewModel.transform.rotation.ButEuler(y: newY);
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

         var velocity = _config.MovementSpeed * speedModifier * ViewModel.transform.forward;
         velocity.y = Rigidbody.velocity.y;

         Rigidbody.velocity = velocity;
      }

      public class Factory {
         private PlayerWalkingStateConfig _config;

         public Factory(PlayerWalkingStateConfig config) {
            _config = config;
         }

         public PlayerWalkingState Create(PlayerViewModel viewModel) {
            return new PlayerWalkingState(viewModel, _config);
         }
      }
   }
}
