using UnityEngine;

namespace SecretHostel.DreamRogue {
   public class SpiderActiveState : SpiderViewModel.State {
      private SpiderActiveStateConfig _config;

      private SpiderActiveState(SpiderViewModel viewModel, SpiderActiveStateConfig config) : base(viewModel) {
	      _config = config;
      }

      public class Factory {
         private SpiderActiveStateConfig _config;

         public Factory(SpiderActiveStateConfig config) {
            _config = config;
         }

         public SpiderActiveState Create(SpiderViewModel viewModel) {
            return new SpiderActiveState(viewModel, _config);
         }
      }
   }
}
