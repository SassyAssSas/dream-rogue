using UnityEngine;
using UnityEngine.Tilemaps;
using Violoncello.Services;
using Violoncello.Structurization;

namespace SecretHostel.DreamRogue {
   public class LevelViewModel : EntityViewModel<LevelViewModel> {
      [SerializeField] private Tilemap _groundTilemap;

      public new abstract class Presenter<TState> : EntityViewModel<LevelViewModel>.Presenter<TState> where TState : State {
         protected override void OnViewModelBound(LevelViewModel viewModel, IStateMachine<TState> stateMachine) {
            base.OnViewModelBound(viewModel, stateMachine);


         }

         protected override void OnViewModelUnbound(LevelViewModel viewModel) {
            base.OnViewModelUnbound(viewModel);


         }
      }

      public new abstract class Presenter : Presenter<State> {

      }

      public new abstract class State : EntityViewModel<LevelViewModel>.State {
         protected Tilemap GroundTilemap => ViewModel._groundTilemap;

         protected State(LevelViewModel viewModel) : base(viewModel) {

         }
      }
   }
}