using UnityEngine;
using Violoncello.Structurization;
using Violoncello.Utilities;
using Zenject;

namespace SecretHostel.DreamRogue {
	[CreateAssetMenu(fileName = "BulletInstaller", menuName = "Project Configuration/Entities/Bullet/Installer")]
	public class BulletInstaller : ScriptableObjectInstaller<BulletInstaller> {
		[SerializeField] private BulletActiveStateConfig _bulletActiveStateConfig;
		[SerializeField] private BulletCollisionStateConfig _bulletCollisionStateConfig;

      [SerializeField] private BulletViewModel _bulletPrefab;

      public override void InstallBindings() {
			ValidateSerializedFields();
			
			InstallActiveState();
			InstallCollisionState();

			Container.Bind(typeof(EntityPresenter<BulletViewModel>), typeof(ITickable))
						.To<BulletPresenter>()
						.AsSingle();

			Container.BindMemoryPool<BulletViewModel, BulletViewModel.Pool>()
						.WithInitialSize(100)
                  .FromComponentInNewPrefab(_bulletPrefab)
						.UnderTransformGroup("Bullets");
		}
		
		private void InstallActiveState() {
			Container.BindInterfacesAndSelfTo<BulletActiveStateConfig>()
						.FromInstance(_bulletActiveStateConfig)
						.AsSingle();
			
			Container.BindInterfacesAndSelfTo<BulletActiveState.Factory>()
						.AsSingle();
		}

		private void InstallCollisionState() {
			Container.BindInterfacesAndSelfTo<BulletCollisionStateConfig>()
						.FromInstance(_bulletCollisionStateConfig)
						.AsSingle();
			
			Container.BindInterfacesAndSelfTo<BulletCollisionState.Factory>()
						.AsSingle();
		}
		
		private void ValidateSerializedFields() {
         Assert.IsNotNull(_bulletPrefab)
               .Throws("Installation failed: BulletViewModel prefab was null.");

         Assert.IsNotNull(_bulletActiveStateConfig)
					.Throws("Installation failed: BulletActiveState Config was null.");

			Assert.IsNotNull(_bulletCollisionStateConfig)
					.Throws("Installation failed: BulletCollisionState Config was null.");
		}
	}
}