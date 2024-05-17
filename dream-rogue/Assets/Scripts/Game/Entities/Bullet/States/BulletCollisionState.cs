using UnityEngine;

namespace SecretHostel.DreamRogue {
   public class BulletCollisionState : BulletViewModel.State {
      private BulletCollisionStateConfig _config;

      private BulletCollisionState(BulletViewModel viewModel, BulletCollisionStateConfig config) : base(viewModel) {
	      _config = config;
      }

      public class Factory {
         private BulletCollisionStateConfig _config;

         public Factory(BulletCollisionStateConfig config) {
            _config = config;
         }

         public BulletCollisionState Create(BulletViewModel viewModel) {
            return new BulletCollisionState(viewModel, _config);
         }
      }
   }
}
