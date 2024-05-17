using System;
using System.Collections.Generic;
using UnityEngine;
using Violoncello.Services.Generic;
using Violoncello.Services;
using Violoncello.Utilities;
using Zenject;

namespace Violoncello.Structurization {
   public class EntityViewModel<TDerived> : MonoBehaviour where TDerived : EntityViewModel<TDerived> {
      protected event Action OnDestroying;

      private EntityPresenter<TDerived> _presenter;

      [Inject]
      private void Construct(EntityPresenter<TDerived> presenter) {
         _presenter = presenter;
      }

      protected virtual void Start() {
         _presenter.BindViewModel(this as TDerived);

         OnDestroying += () => _presenter.UnbindViewModel(this as TDerived);
      }

      private void OnDestroy() {
         OnDestroying?.Invoke();
      }

      public abstract class Presenter<TState> : EntityPresenter<TDerived> where TState : State {
         protected Dictionary<TDerived, IStateMachine<TState>> ViewModelStateMachinePairs { get; }

         protected bool AnyViewModelsBound => ViewModelStateMachinePairs.Count > 0;

         public Presenter() {
            ViewModelStateMachinePairs = new();
         }

         public override sealed void BindViewModel(TDerived viewModel) {
            Assert.That(!ViewModelStateMachinePairs.ContainsKey(viewModel))
                  .Throws("Attempted to bind the same ViewModel twice to one Presenter.");

            var stateMachine = new StateMachine<TState>();

            ViewModelStateMachinePairs.Add(viewModel, stateMachine);

            OnViewModelBound(viewModel, stateMachine);
         }

         public override sealed void UnbindViewModel(TDerived viewModel) {
            ViewModelStateMachinePairs.Remove(viewModel);

            OnViewModelUnbound(viewModel);
         }

         protected virtual void OnViewModelBound(TDerived viewModel, IStateMachine<TState> stateMachine) {
            
         }

         protected virtual void OnViewModelUnbound(TDerived viewModel) {
            
         }
      }

      public abstract class Presenter : Presenter<State> {

      }

      public abstract class State : Services.State {
         protected TDerived ViewModel { get; }

         public State(TDerived viewModel) {
            ViewModel = viewModel;
         }
      }
   }
}
