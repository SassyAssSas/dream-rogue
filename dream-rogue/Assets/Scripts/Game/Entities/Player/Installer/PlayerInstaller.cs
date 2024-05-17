using System;
using UnityEngine;
using Violoncello.Structurization;
using Violoncello.Utilities;
using Zenject;

namespace SecretHostel.DreamRogue {
	[CreateAssetMenu(fileName = "PlayerInstaller", menuName = "Project Configuration/Entities/Player/Installer")]
	public class TestEntityInstaller : ScriptableObjectInstaller<TestEntityInstaller> {
		[SerializeField] private PlayerWalkingStateConfig _playerMovementStateConfig;
		[SerializeField] private PlayerFightingStateConfig _playerFightingStateConfig;
      [SerializeField] private PlayerDashStateConfig _playerDashStateConfig;
      [SerializeField] private PlayerQuickfallStateConfig _playerQuickfallStateConfig;
      [SerializeField] private PlayerJumpingStateConfig _playerJumpingStateConfig;

      public override void InstallBindings() {
			ValidateSerializedFields();

			InstallJumpingState();
			InstallQuickfallState();
         InstallMovementState();
			InstallFightingState();
			InstallDashState();

         Container.Bind(typeof(EntityPresenter<PlayerViewModel>), typeof(ITickable), typeof(IFixedTickable))
						.To<PlayerPresenter>()
						.AsSingle();

			Container.Bind<EntityViewModel<PlayerViewModel>>()
						.To<PlayerViewModel>()
						.AsSingle();
		}
		
		private void InstallMovementState() {
			Container.BindInterfacesAndSelfTo<PlayerWalkingStateConfig>()
						.FromInstance(_playerMovementStateConfig)
						.AsSingle();
			
			Container.BindInterfacesAndSelfTo<PlayerWalkingState.Factory>()
						.AsSingle();
		}

		private void InstallFightingState() {
			Container.BindInterfacesAndSelfTo<PlayerFightingStateConfig>()
						.FromInstance(_playerFightingStateConfig)
						.AsSingle();
			
			Container.BindInterfacesAndSelfTo<PlayerFightingState.Factory>()
						.AsSingle();
		}

      private void InstallDashState() {
         Container.BindInterfacesAndSelfTo<PlayerDashStateConfig>()
                  .FromInstance(_playerDashStateConfig)
                  .AsSingle();

         Container.BindInterfacesAndSelfTo<PlayerDashState.Factory>()
                  .AsSingle();
      }

      private void InstallJumpingState() {
         Container.BindInterfacesAndSelfTo<PlayerJumpingStateConfig>()
                  .FromInstance(_playerJumpingStateConfig)
                  .AsSingle();

         Container.BindInterfacesAndSelfTo<PlayerJumpingState.Factory>()
                  .AsSingle();
      }

      private void InstallQuickfallState() {
         Container.BindInterfacesAndSelfTo<PlayerQuickfallStateConfig>()
                  .FromInstance(_playerQuickfallStateConfig)
                  .AsSingle();

         Container.BindInterfacesAndSelfTo<PlayerQuickfallState.Factory>()
                  .AsSingle();
      }

      private void ValidateSerializedFields() {
			Assert.IsNotNull(_playerMovementStateConfig)
					.Throws("Installation failed: PlayerMovementState Config was null.");

			Assert.IsNotNull(_playerFightingStateConfig)
					.Throws("Installation failed: PlayerFightingState Config was null.");

         Assert.IsNotNull(_playerDashStateConfig)
					.Throws("Installation failed: PlayerDashState Config was null.");

         Assert.IsNotNull(_playerQuickfallStateConfig)
               .Throws("Installation failed: PlayerQuickfallState Config was null.");

         Assert.IsNotNull(_playerJumpingStateConfig)
               .Throws("Installation failed: PlayerJumpingState Config was null.");
      }
	}
}