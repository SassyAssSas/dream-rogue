using UnityEngine;

namespace SecretHostel.DreamRogue {
   public class CommandRunnerInputState : CommandRunnerViewModel.State {
      private CommandRunnerInputStateConfig _config;

      private CommandRunnerInputState(CommandRunnerViewModel viewModel, CommandRunnerInputStateConfig config) : base(viewModel) {
	      _config = config;
      }

      public override void EnterState() {
         
      }

      

      public class Factory {
         private CommandRunnerInputStateConfig _config;

         public Factory(CommandRunnerInputStateConfig config) {
            _config = config;
         }

         public CommandRunnerInputState Create(CommandRunnerViewModel viewModel) {
            return new CommandRunnerInputState(viewModel, _config);
         }
      }
   }
}
