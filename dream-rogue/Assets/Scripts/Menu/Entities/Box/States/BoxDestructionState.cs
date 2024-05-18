using UnityEngine;

namespace SecretHostel.DreamRogue {
   public class BoxDestructionState : BoxViewModel.State {
      private BoxDestructionStateConfig _config;

      private BoxDestructionState(BoxViewModel viewModel, BoxDestructionStateConfig config) : base(viewModel) {
	      _config = config;
      }

      public class Factory {
         private BoxDestructionStateConfig _config;

         public Factory(BoxDestructionStateConfig config) {
            _config = config;
         }

         public BoxDestructionState Create(BoxViewModel viewModel) {
            return new BoxDestructionState(viewModel, _config);
         }
      }
   }
}
