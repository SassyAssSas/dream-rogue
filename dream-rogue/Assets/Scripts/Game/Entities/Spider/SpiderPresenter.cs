using UnityEngine;
using Violoncello.Services;
using Violoncello.Structurization;
using Zenject;

namespace SecretHostel.DreamRogue {
	public class SpiderPresenter : SpiderViewModel.Presenter, ITickable {
		private SpiderActiveState.Factory _spiderActiveStateFactory;

      public SpiderPresenter(
			SpiderActiveState.Factory spiderActiveStateFactory
      ) {
			_spiderActiveStateFactory = spiderActiveStateFactory;
      }
      
		protected override void OnViewModelBound(SpiderViewModel viewModel, IStateMachine<SpiderViewModel.State> stateMachine) {
         base.OnViewModelBound(viewModel, stateMachine);


      }

      protected override void OnViewModelUnbound(SpiderViewModel viewModel) {
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