using UnityEngine;
using Violoncello.Services;
using Violoncello.Structurization;
using Zenject;

namespace SecretHostel.DreamRogue {
   public class MenuPresenter : MenuViewModel.Presenter, ITickable {
      private ITransitionService _transitionService;

      private bool isLoadingGame;

      [Inject]
      public MenuPresenter(
         ITransitionService transitionService
      ) {
         _transitionService = transitionService;
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
         if (isLoadingGame) {
            return;
         }

         _transitionService.Transition("Game", TransitionOptions.SimpleFade);

         isLoadingGame = true;
      }
   }
}