using UnityEngine;

namespace SecretHostel.DreamRogue {
   public class BulletActiveState : BulletViewModel.State {
      private BulletActiveStateConfig _config;

      private Vector3[] Positions = new Vector3[2];

      private BulletActiveState(BulletViewModel viewModel, BulletActiveStateConfig config) : base(viewModel) {
	      _config = config;
      }

      public override void Tick(float deltaTime) {
         LineRenderer.GetPositions(Positions);

         var positionDelta = _config.MovementSpeed * deltaTime * Direction;

         LineRenderer.SetPosition(1, Positions[1] + positionDelta);

         if (Vector3.Distance(Positions[0], Positions[1]) > _config.MaxLength) {
            LineRenderer.SetPosition(0, Positions[0] + positionDelta);
         }
      }

      public class Factory {
         private BulletActiveStateConfig _config;

         public Factory(BulletActiveStateConfig config) {
            _config = config;
         }

         public BulletActiveState Create(BulletViewModel viewModel) {
            return new BulletActiveState(viewModel, _config);
         }
      }
   }
}
