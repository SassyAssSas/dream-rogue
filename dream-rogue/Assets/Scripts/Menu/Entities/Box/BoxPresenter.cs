using UnityEngine;
using Violoncello.Services;
using Violoncello.Structurization;
using Zenject;

namespace SecretHostel.DreamRogue {
   public class BoxPresenter : BoxViewModel.Presenter, ITickable {
      private BoxIdleState.Factory _boxIdleStateFactory;
      private BoxDestructionState.Factory _boxDestructionStateFactory;

      public BoxPresenter(
         BoxIdleState.Factory boxIdleStateFactory,
         BoxDestructionState.Factory boxDestructionStateFactory
      ) {
         _boxIdleStateFactory = boxIdleStateFactory;
         _boxDestructionStateFactory = boxDestructionStateFactory;
      }

      protected override void OnViewModelBound(BoxViewModel viewModel, IStateMachine<BoxViewModel.State> stateMachine) {
         base.OnViewModelBound(viewModel, stateMachine);


      }

      protected override void OnViewModelUnbound(BoxViewModel viewModel) {
         base.OnViewModelUnbound(viewModel);


      }

      public void Tick() {
         foreach (var pair in ViewModelStateMachinePairs) {
            var viewModel = pair.Key;
            var stateMachine = pair.Value;

            if (viewModel.gameObject.activeInHierarchy) {
               stateMachine.CurrentState?.Tick(Time.deltaTime);
            }
         }
      }
   }
}