using UnityEngine;
using Violoncello.Structurization;
using Violoncello.Utilities;
using Zenject;

namespace SecretHostel.DreamRogue {
	[CreateAssetMenu(fileName = "BoxInstaller", menuName = "Project Configuration/Entities/Box/Installer")]
	public class BoxInstaller : ScriptableObjectInstaller<BoxInstaller> {
		[SerializeField] private BoxIdleStateConfig _boxIdleStateConfig;
		[SerializeField] private BoxDestructionStateConfig _boxDestructionStateConfig;
	
		public override void InstallBindings() {
			ValidateSerializedFields();
			
			InstallIdleState();
			InstallDestructionState();

			Container.Bind(typeof(EntityPresenter<BoxViewModel>), typeof(ITickable))
						.To<BoxPresenter>()
						.AsSingle();

			Container.Bind<EntityViewModel<BoxViewModel>>()
						.To<BoxViewModel>()
						.AsSingle();
		}
		
		private void InstallIdleState() {
			Container.BindInterfacesAndSelfTo<BoxIdleStateConfig>()
						.FromInstance(_boxIdleStateConfig)
						.AsSingle();
			
			Container.BindInterfacesAndSelfTo<BoxIdleState.Factory>()
						.AsSingle();
		}

		private void InstallDestructionState() {
			Container.BindInterfacesAndSelfTo<BoxDestructionStateConfig>()
						.FromInstance(_boxDestructionStateConfig)
						.AsSingle();
			
			Container.BindInterfacesAndSelfTo<BoxDestructionState.Factory>()
						.AsSingle();
		}
		
		private void ValidateSerializedFields() {
			Assert.IsNotNull(_boxIdleStateConfig)
					.Throws("Installation failed: BoxIdleState Config was null.");

			Assert.IsNotNull(_boxDestructionStateConfig)
					.Throws("Installation failed: BoxDestructionState Config was null.");
		}
	}
}