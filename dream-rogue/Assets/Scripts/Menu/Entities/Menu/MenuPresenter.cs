using UnityEngine;
using Violoncello.Services;
using Violoncello.Structurization;
using Zenject;

namespace SecretHostel.DreamRogue {
   public class MenuPresenter : MenuViewModel.Presenter, ITickable {
      private ITransitionService _transitionService;

      [Inject]
      public MenuPresenter(
         ITransitionService transitionService
      ) {
         _transitionService = transitionService;
      }

      protected override void OnViewModelBound(MenuViewModel viewModel, IStateMachine<MenuViewModel.State> stateMachine) {
         base.OnViewModelBound(viewModel, stateMachine);

         
      }

      protected override void OnViewModelUnbound(MenuViewModel viewModel) {
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

      protected override void OnPlayButtonPressed() {
         _transitionService.Transition("Game", TransitionOptions.SimpleFade);
      }
   }
}