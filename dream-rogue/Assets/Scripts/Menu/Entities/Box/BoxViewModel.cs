using System;
using System.Collections.Generic;
using UnityEngine;
using Violoncello.Services;
using Violoncello.Utilities;
using Violoncello.Structurization;

namespace SecretHostel.DreamRogue {
	public class BoxViewModel : PoolableEntityViewModel<BoxViewModel> {
		public List<string> Loot { get; private set; }
		private event Action<BoxViewModel, List<string>> OnLootSetRequested;

		public void RequestLootSet(List<string> value) {
			OnLootSetRequested?.Invoke(this, value);
		}

		public new abstract class Presenter<TState> : PoolableEntityViewModel<BoxViewModel>.Presenter<TState> where TState : State {
			protected override void OnViewModelBound(BoxViewModel viewModel, IStateMachine<TState> stateMachine) {
				base.OnViewModelBound(viewModel, stateMachine);
				
				viewModel.OnLootSetRequested += OnLootSetRequested;
			}

			protected override void OnViewModelUnbound(BoxViewModel viewModel) {
				base.OnViewModelUnbound(viewModel);
				
				viewModel.OnLootSetRequested -= OnLootSetRequested;	
			}

			protected virtual void OnLootSetRequested(BoxViewModel viewModel, List<string> value) {
				Assert.That(ViewModelStateMachinePairs.TryGetValue(viewModel, out IStateMachine<TState> state))
						.Throws("Recieved event call from an unknown ViewModel.");
			
				state.CurrentState.OnLootSetRequested(value);
			}
		}

		public new abstract class Presenter : Presenter<State> { 
		   
		}

		public new abstract class State : PoolableEntityViewModel<BoxViewModel>.State {
			protected List<string> Loot {
				get => ViewModel.Loot;
				set => ViewModel.Loot = value;
}

			protected State(BoxViewModel viewModel) : base(viewModel) {
				
			}

			public virtual void OnLootSetRequested(List<string> value) {
				
			}
		}
	}
}