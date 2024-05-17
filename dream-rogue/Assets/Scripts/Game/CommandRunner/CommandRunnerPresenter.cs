using UnityEngine;
using Violoncello.Services;
using Violoncello.Structurization;
using Zenject;

namespace SecretHostel.DreamRogue {
	public class CommandRunnerPresenter : CommandRunnerViewModel.Presenter, ITickable {
		private CommandRunnerAwaitingState.Factory _commandRunnerAwaitingStateFactory;
		private CommandRunnerInputState.Factory _commandRunnerInputStateFactory;
		private CommandRunnerExecutingState.Factory _commandRunnerExecutingStateFactory;

      public CommandRunnerPresenter(
			CommandRunnerAwaitingState.Factory commandRunnerAwaitingStateFactory,
			CommandRunnerInputState.Factory commandRunnerInputStateFactory,
			CommandRunnerExecutingState.Factory commandRunnerExecutingStateFactory
      ) {
			_commandRunnerAwaitingStateFactory = commandRunnerAwaitingStateFactory;
			_commandRunnerInputStateFactory = commandRunnerInputStateFactory;
			_commandRunnerExecutingStateFactory = commandRunnerExecutingStateFactory;
      }
      
		protected override void OnViewModelBound(CommandRunnerViewModel viewModel, IStateMachine<CommandRunnerViewModel.State> stateMachine) {
         base.OnViewModelBound(viewModel, stateMachine);

         stateMachine.AddState("awaiting", _commandRunnerAwaitingStateFactory.Create(viewModel));
         stateMachine.AddState("input", _commandRunnerInputStateFactory.Create(viewModel));
         stateMachine.AddState("executing", _commandRunnerExecutingStateFactory.Create(viewModel));

         stateMachine.AddTransition("awaiting", "input", 0);

         stateMachine.AddTransition("input", "executing", 0);
         stateMachine.AddTransition("input", "awaiting", 1);

         stateMachine.AddTransition("executing", "awaiting", 0);

         stateMachine.SetState("awaiting");
      }

      protected override void OnViewModelUnbound(CommandRunnerViewModel viewModel) {
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