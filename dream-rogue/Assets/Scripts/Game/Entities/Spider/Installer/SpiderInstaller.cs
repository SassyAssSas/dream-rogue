using UnityEngine;
using Violoncello.Structurization;
using Violoncello.Utilities;
using Zenject;

namespace SecretHostel.DreamRogue {
	[CreateAssetMenu(fileName = "SpiderInstaller", menuName = "Project Configuration/Entities/Spider/Installer")]
	public class SpiderInstaller : ScriptableObjectInstaller<SpiderInstaller> {
		[SerializeField] private SpiderActiveStateConfig _spiderActiveStateConfig;
	
		public override void InstallBindings() {
			ValidateSerializedFields();
			
			InstallActiveState();

			Container.Bind(typeof(EntityPresenter<SpiderViewModel>), typeof(ITickable))
						.To<SpiderPresenter>()
						.AsSingle();

			Container.Bind<EntityViewModel<SpiderViewModel>>()
						.To<SpiderViewModel>()
						.AsSingle();
		}
		
		private void InstallActiveState() {
			Container.BindInterfacesAndSelfTo<SpiderActiveStateConfig>()
						.FromInstance(_spiderActiveStateConfig)
						.AsSingle();
			
			Container.BindInterfacesAndSelfTo<SpiderActiveState.Factory>()
						.AsSingle();
		}
		
		private void ValidateSerializedFields() {
			Assert.IsNotNull(_spiderActiveStateConfig)
					.Throws("Installation failed: SpiderActiveState Config was null.");
		}
	}
}