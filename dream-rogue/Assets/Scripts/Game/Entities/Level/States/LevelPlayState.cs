using UnityEngine;

namespace SecretHostel.DreamRogue {
   public class LevelPlayState : LevelViewModel.State {
      private LevelPlayStateConfig _config;

      private LevelPlayState(LevelViewModel viewModel, LevelPlayStateConfig config) : base(viewModel) {
	      _config = config;
      }

      public class Factory {
         private LevelPlayStateConfig _config;

         public Factory(LevelPlayStateConfig config) {
         _config = config;
         }

         public LevelPlayState Create(LevelViewModel viewModel) {
            return new LevelPlayState(viewModel, _config);
         }
      }
   }
}
