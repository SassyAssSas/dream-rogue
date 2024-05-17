using UnityEngine;
using Violoncello.Services;
using Zenject;

namespace SecretHostel.DreamRogue {
   public class BootstrapFinisher : MonoBehaviour {
      private ITransitionService _transitionService;

      [Inject]
      public void Construct(ITransitionService transitionService) {
         _transitionService = transitionService;
      }

      public void Start() {
         _transitionService.Transition("Menu", TransitionOptions.SimpleFade);
      }
   }
}
