using UnityEngine;

namespace SecretHostel.DreamRogue {
   public class PlayerFightingState : PlayerViewModel.State {
      private PlayerFightingStateConfig _config;

      private PlayerFightingState(PlayerViewModel viewModel, PlayerFightingStateConfig config) : base(viewModel) {
	      _config = config;
      }

      public class Factory {
         private PlayerFightingStateConfig _config;

         public Factory(PlayerFightingStateConfig config) {
         _config = config;
         }

         public PlayerFightingState Create(PlayerViewModel viewModel) {
            return new PlayerFightingState(viewModel, _config);
         }
      }
   }
}
