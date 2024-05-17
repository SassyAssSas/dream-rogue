using UnityEngine;

namespace SecretHostel.DreamRogue {
   public class ButtonSelectedState : ButtonViewModel.State {
      private ButtonSelectedStateConfig _config;

      private ButtonSelectedState(ButtonViewModel viewModel, ButtonSelectedStateConfig config) : base(viewModel) {
         _config = config;
      }

      public override void EnterState() {
         Text = $"> {RawText} <";
      }

      public override void OnDeselected() {
         FinishState(0);
      }

      public class Factory {
         private ButtonSelectedStateConfig _config;

         public Factory(ButtonSelectedStateConfig config) {
            _config = config;
         }

         public ButtonSelectedState Create(ButtonViewModel viewModel) {
            return new ButtonSelectedState(viewModel, _config);
         }
      }
   }
}
