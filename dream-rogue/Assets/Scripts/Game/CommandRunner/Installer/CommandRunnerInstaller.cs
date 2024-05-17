using UnityEngine;
using Violoncello.Structurization;
using Violoncello.Utilities;
using Zenject;

namespace SecretHostel.DreamRogue {
	[CreateAssetMenu(fileName = "CommandRunnerInstaller", menuName = "Project Configuration/Entities/CommandRunner/Installer")]
	public class CommandRunnerInstaller : ScriptableObjectInstaller<CommandRunnerInstaller> {
		[SerializeField] private CommandRunnerAwaitingStateConfig _commandRunnerAwaitingStateConfig;
		[SerializeField] private CommandRunnerInputStateConfig _commandRunnerInputStateConfig;
		[SerializeField] private CommandRunnerExecutingStateConfig _commandRunnerExecutingStateConfig;
	
		public override void InstallBindings() {
			ValidateSerializedFields();
			
			InstallAwaitingState();
			InstallInputState();
			InstallExecutingState();

			Container.Bind(typeof(EntityPresenter<CommandRunnerViewModel>), typeof(ITickable))
						.To<CommandRunnerPresenter>()
						.AsSingle();

			Container.Bind<EntityViewModel<CommandRunnerViewModel>>()
						.To<CommandRunnerViewModel>()
						.AsSingle();
		}
		
		private void InstallAwaitingState() {
			Container.BindInterfacesAndSelfTo<CommandRunnerAwaitingStateConfig>()
						.FromInstance(_commandRunnerAwaitingStateConfig)
						.AsSingle();
			
			Container.BindInterfacesAndSelfTo<CommandRunnerAwaitingState.Factory>()
						.AsSingle();
		}

		private void InstallInputState() {
			Container.BindInterfacesAndSelfTo<CommandRunnerInputStateConfig>()
						.FromInstance(_commandRunnerInputStateConfig)
						.AsSingle();
			
			Container.BindInterfacesAndSelfTo<CommandRunnerInputState.Factory>()
						.AsSingle();
		}

		private void InstallExecutingState() {
			Container.BindInterfacesAndSelfTo<CommandRunnerExecutingStateConfig>()
						.FromInstance(_commandRunnerExecutingStateConfig)
						.AsSingle();
			
			Container.BindInterfacesAndSelfTo<CommandRunnerExecutingState.Factory>()
						.AsSingle();
		}
		
		private void ValidateSerializedFields() {
			Assert.IsNotNull(_commandRunnerAwaitingStateConfig)
					.Throws("Installation failed: CommandRunnerAwaitingState Config was null.");

			Assert.IsNotNull(_commandRunnerInputStateConfig)
					.Throws("Installation failed: CommandRunnerInputState Config was null.");

			Assert.IsNotNull(_commandRunnerExecutingStateConfig)
					.Throws("Installation failed: CommandRunnerExecutingState Config was null.");
		}
	}
}