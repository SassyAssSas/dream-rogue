using UnityEngine;
using Violoncello.Structurization;
using Violoncello.Utilities;
using Zenject;

namespace SecretHostel.DreamRogue {
	[CreateAssetMenu(fileName = "ButtonInstaller", menuName = "Project Configuration/Entities/Button/Installer")]
	public class ButtonInstaller : ScriptableObjectInstaller<ButtonInstaller> {
		[SerializeField] private ButtonIdleStateConfig _buttonIdleStateConfig;
		[SerializeField] private ButtonSelectedStateConfig _buttonSelectedStateConfig;
	
		public override void InstallBindings() {
			ValidateSerializedFields();
			
			InstallIdleState();
			InstallSelectedState();

			Container.Bind(typeof(EntityPresenter<ButtonViewModel>), typeof(ITickable))
						.To<ButtonPresenter>()
						.AsSingle();

			Container.Bind<EntityViewModel<ButtonViewModel>>()
						.To<ButtonViewModel>()
						.AsSingle();
		}
		
		private void InstallIdleState() {
			Container.BindInterfacesAndSelfTo<ButtonIdleStateConfig>()
						.FromInstance(_buttonIdleStateConfig)
						.AsSingle();
			
			Container.BindInterfacesAndSelfTo<ButtonIdleState.Factory>()
						.AsSingle();
		}

		private void InstallSelectedState() {
			Container.BindInterfacesAndSelfTo<ButtonSelectedStateConfig>()
						.FromInstance(_buttonSelectedStateConfig)
						.AsSingle();
			
			Container.BindInterfacesAndSelfTo<ButtonSelectedState.Factory>()
						.AsSingle();
		}
		
		private void ValidateSerializedFields() {
			Assert.IsNotNull(_buttonIdleStateConfig)
					.Throws("Installation failed: ButtonIdleState Config was null.");

			Assert.IsNotNull(_buttonSelectedStateConfig)
					.Throws("Installation failed: ButtonSelectedState Config was null.");
		}
	}
}