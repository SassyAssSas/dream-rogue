using Violoncello.Services;

namespace SecretHostel.DreamRogue {
	public class LevelPresenter : LevelViewModel.Presenter {
		private LevelGenerationState.Factory _levelGenerationStateFactory;
		private LevelPlayState.Factory _levelPlayStateFactory;

      public LevelPresenter(
			LevelGenerationState.Factory levelGenerationStateFactory,
			LevelPlayState.Factory levelPlayStateFactory
      ) {
			_levelGenerationStateFactory = levelGenerationStateFactory;
			_levelPlayStateFactory = levelPlayStateFactory;
      }
      
		protected override void OnViewModelBound(LevelViewModel viewModel, IStateMachine<LevelViewModel.State> stateMachine) {
         base.OnViewModelBound(viewModel, stateMachine);

         stateMachine.AddState("generation", _levelGenerationStateFactory.Create(viewModel));
         stateMachine.AddState("play", _levelPlayStateFactory.Create(viewModel));

         stateMachine.AddTransition("generation", "play", 0);

         stateMachine.SetState("generation");
      }

      protected override void OnViewModelUnbound(LevelViewModel viewModel) {
         base.OnViewModelUnbound(viewModel);


      }
	}
}