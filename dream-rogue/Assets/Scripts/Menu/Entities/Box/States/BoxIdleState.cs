using UnityEngine;

namespace SecretHostel.DreamRogue {
   public class BoxIdleState : BoxViewModel.State {
      private BoxIdleStateConfig _config;

      private BoxIdleState(BoxViewModel viewModel, BoxIdleStateConfig config) : base(viewModel) {
	      _config = config;
      }

      public class Factory {
         private BoxIdleStateConfig _config;

         public Factory(BoxIdleStateConfig config) {
            _config = config;
         }

         public BoxIdleState Create(BoxViewModel viewModel) {
            return new BoxIdleState(viewModel, _config);
         }
      }
   }
}
