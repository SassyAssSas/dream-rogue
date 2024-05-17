using Zenject;
using Violoncello.Services;

namespace Violoncello.Structurization {
   public abstract class PoolableEntityViewModel<TDerived> : EntityViewModel<TDerived> where TDerived : PoolableEntityViewModel<TDerived> {
      protected Pool ContainingPool { get; private set; }

      public class Pool : MonoMemoryPool<TDerived> {
         protected override void OnCreated(TDerived viewModel) {
            base.OnCreated(viewModel);

            viewModel.ContainingPool = this;
         }
      }

      public new abstract class Presenter<TState> : EntityViewModel<TDerived>.Presenter<TState> where TState : State {
         protected override void OnViewModelBound(TDerived viewModel, IStateMachine<TState> stateMachine) {
            base.OnViewModelBound(viewModel, stateMachine);

            
         }

         protected override void OnViewModelUnbound(TDerived viewModel) {
            base.OnViewModelUnbound(viewModel);


         }

         protected void Despawn(TDerived viewModel) {
            viewModel.ContainingPool.Despawn(viewModel);
         }
      }

      public new abstract class Presenter : Presenter<State> { 
         
      }

      public new abstract class State : EntityViewModel<TDerived>.State {
         protected State(TDerived viewModel) : base(viewModel) {
            
         }

         protected void ViewModelDespawn() {
            ViewModel.ContainingPool.Despawn(ViewModel);
         }
      }
   }
}
