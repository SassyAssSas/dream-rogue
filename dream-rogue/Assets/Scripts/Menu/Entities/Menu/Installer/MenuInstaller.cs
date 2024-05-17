using UnityEngine;
using Violoncello.Structurization;
using Violoncello.Utilities;
using Zenject;

namespace SecretHostel.DreamRogue {
   [CreateAssetMenu(fileName = "MenuInstaller", menuName = "Project Configuration/Entities/Menu/Installer")]
   public class MenuInstaller : ScriptableObjectInstaller<MenuInstaller> {
      public override void InstallBindings() {
         Container.Bind(typeof(EntityPresenter<MenuViewModel>), typeof(ITickable))
                  .To<MenuPresenter>()
                  .AsSingle();

         Container.Bind<EntityViewModel<MenuViewModel>>()
                  .To<MenuViewModel>()
                  .AsSingle();
      }
   }
}