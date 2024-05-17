using UnityEngine;
using Violoncello.Services;
using Violoncello.Structurization;
using Zenject;

namespace SecretHostel.DreamRogue {
   public class BulletPresenter : BulletViewModel.Presenter, ITickable {
      private BulletActiveState.Factory _bulletActiveStateFactory;
      private BulletCollisionState.Factory _bulletCollisionStateFactory;

      public BulletPresenter(
         BulletActiveState.Factory bulletActiveStateFactory,
         BulletCollisionState.Factory bulletCollisionStateFactory
      ) {
         _bulletActiveStateFactory = bulletActiveStateFactory;
         _bulletCollisionStateFactory = bulletCollisionStateFactory;
      }

      protected override void OnViewModelBound(BulletViewModel viewModel, IStateMachine<BulletViewModel.State> stateMachine) {
         base.OnViewModelBound(viewModel, stateMachine);

         stateMachine.AddState("active", _bulletActiveStateFactory.Create(viewModel));

         stateMachine.SetState("active");
      }

      protected override void OnViewModelUnbound(BulletViewModel viewModel) {
         base.OnViewModelUnbound(viewModel);


      }

      public void Tick() {
         foreach (var pair in ViewModelStateMachinePairs) {
            var viewModel = pair.Key;
            var stateMachine = pair.Value;

            if (viewModel.gameObject.activeInHierarchy) {
               stateMachine.CurrentState?.Tick(Time.deltaTime);
            }
         }
      }
   }
}