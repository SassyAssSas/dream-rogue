using UnityEngine;
using Violoncello.Services;
using Violoncello.Structurization;
using Zenject;

namespace SecretHostel.DreamRogue {
	public class PlayerPresenter : PlayerViewModel.Presenter, ITickable, IFixedTickable {
		private PlayerWalkingState.Factory _playerMovementStateFactory;
		private PlayerFightingState.Factory _playerFightingStateFactory;
      private PlayerDashState.Factory _playerRocketStateFactory;
      private PlayerQuickfallState.Factory _playerQuickfallStateFactory;
      private PlayerJumpingState.Factory _playerJumpingStateFactory;

      public PlayerPresenter(
			PlayerWalkingState.Factory playerMovementStateFactory,
			PlayerFightingState.Factory playerFightingStateFactory,
         PlayerDashState.Factory playerRocketStateFactory,
         PlayerQuickfallState.Factory playerQuickfallStateFactory,
         PlayerJumpingState.Factory playerJumpingStateFactory
      ) {
			_playerMovementStateFactory = playerMovementStateFactory;
			_playerFightingStateFactory = playerFightingStateFactory;
         _playerRocketStateFactory = playerRocketStateFactory;
         _playerQuickfallStateFactory = playerQuickfallStateFactory;
         _playerJumpingStateFactory = playerJumpingStateFactory;
      }
      
      protected override void OnViewModelBound(PlayerViewModel viewModel, IStateMachine<PlayerViewModel.State> stateMachine) {
         base.OnViewModelBound(viewModel, stateMachine);

         const string walking = "walking";
         const string dash = "dash";
         const string quickfall = "quickfall";
         const string jumping = "jumping";

         stateMachine.AddState(walking, _playerMovementStateFactory.Create(viewModel));
         stateMachine.AddState(dash, _playerRocketStateFactory.Create(viewModel));
         stateMachine.AddState(quickfall, _playerQuickfallStateFactory.Create(viewModel));
         stateMachine.AddState(jumping, _playerJumpingStateFactory.Create(viewModel));

         stateMachine.AddTransition(walking, quickfall, 0);
         stateMachine.AddTransition(walking, jumping, 1);

         stateMachine.AddTransition(jumping, walking, 0);
         stateMachine.AddTransition(jumping, dash, 1);
         stateMachine.AddTransition(jumping, quickfall, 2);

         stateMachine.AddTransition(dash, walking, 0);
         stateMachine.AddTransition(dash, quickfall, 1);

         stateMachine.AddTransition(quickfall, walking, 0);

         stateMachine.SetState(walking);
      }

      protected override void OnViewModelUnbound(PlayerViewModel viewModel) {
         base.OnViewModelUnbound(viewModel);


      }

      public void Tick() {
         foreach (var pair in ViewModelStateMachinePairs) {
            var viewModel = pair.Key;
            var stateMachine = pair.Value;

            if (!viewModel.gameObject.activeInHierarchy) {
               continue;
            }

            stateMachine.CurrentState.Tick(Time.deltaTime);
         }
      }

      public void FixedTick() {
         foreach (var pair in ViewModelStateMachinePairs) {
            var viewModel = pair.Key;
            var stateMachine = pair.Value;

            if (!viewModel.gameObject.activeInHierarchy) {
               continue;
            }
            
            stateMachine.CurrentState.FixedTick(Time.fixedDeltaTime);
         }
      }
   }
}