using Unity.VisualScripting;
using UnityEngine;

namespace SecretHostel.DreamRogue {
   public class ButtonIdleState : ButtonViewModel.State {
      private ButtonIdleStateConfig _config;

      private ButtonIdleState(ButtonViewModel viewModel, ButtonIdleStateConfig config) : base(viewModel) {
	      _config = config;
      }

      public override void EnterState() {
         Text = RawText;
      }

      public override void OnSelected() {
         FinishState(0);
      }

      public class Factory {
         private ButtonIdleStateConfig _config;

         public Factory(ButtonIdleStateConfig config) {
            _config = config;
         }

         public ButtonIdleState Create(ButtonViewModel viewModel) {
            return new ButtonIdleState(viewModel, _config);
         }
      }
   }
}
