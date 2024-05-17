using UnityEngine;
using Violoncello.Services;
using Violoncello.Structurization;
using Zenject;

namespace SecretHostel.DreamRogue {
   public class ButtonPresenter : ButtonViewModel.Presenter, ITickable {
      private ButtonIdleState.Factory _buttonIdleStateFactory;
      private ButtonSelectedState.Factory _buttonSelectedStateFactory;

      public ButtonPresenter(
         ButtonIdleState.Factory buttonIdleStateFactory,
         ButtonSelectedState.Factory buttonSelectedStateFactory
      ) {
         _buttonIdleStateFactory = buttonIdleStateFactory;
         _buttonSelectedStateFactory = buttonSelectedStateFactory;
      }

      protected override void OnViewModelBound(ButtonViewModel viewModel, IStateMachine<ButtonViewModel.State> stateMachine) {
         base.OnViewModelBound(viewModel, stateMachine);

         stateMachine.AddState("idle", _buttonIdleStateFactory.Create(viewModel));
         stateMachine.AddState("selected", _buttonSelectedStateFactory.Create(viewModel));

         stateMachine.AddTransition("idle", "selected", 0);
         stateMachine.AddTransition("selected", "idle", 0);

         stateMachine.SetState("idle");
      }

      protected override void OnViewModelUnbound(ButtonViewModel viewModel) {
         base.OnViewModelUnbound(viewModel);


      }

      public void Tick() {
         foreach (var pair in ViewModelStateMachinePairs) {
            var viewModel = pair.Key;
            var stateMachine = pair.Value;

            if (viewModel.gameObject.activeInHierarchy) {
               stateMachine?.CurrentState.Tick(Time.deltaTime);
            }
         }
      }
   }
}