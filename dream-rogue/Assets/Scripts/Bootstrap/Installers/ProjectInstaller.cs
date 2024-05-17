using UnityEngine;
using Violoncello.Services.Generic;
using Violoncello.Utilities;
using Zenject;

namespace SecretHostel.DreamRogue {
   [CreateAssetMenu(fileName = "ProjectInstaller", menuName = "Project Configuration/Project Installer")]
   public class ProjectInstaller : ScriptableObjectInstaller<ProjectInstaller> {
      [SerializeField] private MonoTransitionService _transitionService;

      public override void InstallBindings() {
         ValidateSerializedFields();

         Container.BindInterfacesAndSelfTo<MonoTransitionService>()
                  .FromComponentInNewPrefab(_transitionService)
                  .AsSingle();

         Container.BindInterfacesAndSelfTo<AssetsLoader>()
                  .AsSingle();
      }

      private void ValidateSerializedFields() {
         Assert.IsNotNull(_transitionService)
               .Throws("INSTALLATION FAILED: Transition service prefab was null.");
      }
   }
}
