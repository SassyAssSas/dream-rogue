using UnityEngine;
using Violoncello.Structurization;
using Violoncello.Utilities;
using Zenject;

namespace SecretHostel.DreamRogue {
	[CreateAssetMenu(fileName = "LevelInstaller", menuName = "Project Configuration/Entities/Level/Installer")]
	public class LevelInstaller : ScriptableObjectInstaller<LevelInstaller> {
		[SerializeField] private LevelGenerationStateConfig _levelGenerationStateConfig;
		[SerializeField] private LevelPlayStateConfig _levelPlayStateConfig;
	
		public override void InstallBindings() {
			ValidateSerializedFields();
			
			InstallGenerationState();
			InstallPlayState();

			Container.Bind<EntityPresenter<LevelViewModel>>()
						.To<LevelPresenter>()
						.AsSingle();

			Container.Bind<EntityViewModel<LevelViewModel>>()
						.To<LevelViewModel>()
						.AsSingle();
		}
		
		private void InstallGenerationState() {
			Container.BindInterfacesAndSelfTo<LevelGenerationStateConfig>()
						.FromInstance(_levelGenerationStateConfig)
						.AsSingle();
			
			Container.BindInterfacesAndSelfTo<LevelGenerationState.Factory>()
						.AsSingle();
		}

		private void InstallPlayState() {
			Container.BindInterfacesAndSelfTo<LevelPlayStateConfig>()
						.FromInstance(_levelPlayStateConfig)
						.AsSingle();
			
			Container.BindInterfacesAndSelfTo<LevelPlayState.Factory>()
						.AsSingle();
		}
		
		private void ValidateSerializedFields() {
			Assert.IsNotNull(_levelGenerationStateConfig)
					.Throws("Installation failed: LevelGenerationState Config was null.");

			Assert.IsNotNull(_levelPlayStateConfig)
					.Throws("Installation failed: LevelPlayState Config was null.");
		}
	}
}