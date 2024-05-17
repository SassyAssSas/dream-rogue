using UnityEngine;
using UnityEngine.InputSystem;

namespace SecretHostel.DreamRogue {
   public class CommandRunnerAwaitingState : CommandRunnerViewModel.State {
      private CommandRunnerAwaitingStateConfig _config;

      private PlayerInput input;

      private CommandRunnerAwaitingState(CommandRunnerViewModel viewModel, CommandRunnerAwaitingStateConfig config) : base(viewModel) {
         _config = config;

         input = new();

         input.Command.Begin.performed += OnBeginPerformed;
      }

      public override void EnterState() {
         input.Enable();
      }

      public override void QuitState() {
         input.Disable();
      }

      private void OnBeginPerformed(InputAction.CallbackContext context) {
         FinishState(0);
      }

      public class Factory {
         private CommandRunnerAwaitingStateConfig _config;

         public Factory(CommandRunnerAwaitingStateConfig config) {
            _config = config;
         }

         public CommandRunnerAwaitingState Create(CommandRunnerViewModel viewModel) {
            return new CommandRunnerAwaitingState(viewModel, _config);
         }
      }
   }
}
