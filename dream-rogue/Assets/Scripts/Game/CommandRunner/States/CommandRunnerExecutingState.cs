using UnityEngine;

namespace SecretHostel.DreamRogue {
   public class CommandRunnerExecutingState : CommandRunnerViewModel.State {
      private CommandRunnerExecutingStateConfig _config;

      private CommandRunnerExecutingState(CommandRunnerViewModel viewModel, CommandRunnerExecutingStateConfig config) : base(viewModel) {
	      _config = config;
      }

      public class Factory {
         private CommandRunnerExecutingStateConfig _config;

         public Factory(CommandRunnerExecutingStateConfig config) {
            _config = config;
         }

         public CommandRunnerExecutingState Create(CommandRunnerViewModel viewModel) {
            return new CommandRunnerExecutingState(viewModel, _config);
         }
      }
   }
}
