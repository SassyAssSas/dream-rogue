using System;
using System.Collections.Generic;
using UnityEngine;
using Violoncello.Services;
using Violoncello.Utilities;
using Violoncello.Structurization;

namespace SecretHostel.DreamRogue {
	public class SpiderViewModel : PoolableEntityViewModel<SpiderViewModel> {
		public float MovementSpeed { get; private set; }
		private event Action<SpiderViewModel, float> OnMovementSpeedSetRequested;

		public float Damage { get; private set; }
		private event Action<SpiderViewModel, float> OnDamageSetRequested;

		public void RequestMovementSpeedSet(float value) {
			OnMovementSpeedSetRequested?.Invoke(this, value);
		}

		public void RequestDamageSet(float value) {
			OnDamageSetRequested?.Invoke(this, value);
		}

		public new abstract class Presenter<TState> : PoolableEntityViewModel<SpiderViewModel>.Presenter<TState> where TState : State {
			protected override void OnViewModelBound(SpiderViewModel viewModel, IStateMachine<TState> stateMachine) {
				base.OnViewModelBound(viewModel, stateMachine);
				
				viewModel.OnMovementSpeedSetRequested += OnMovementSpeedSetRequested;
				viewModel.OnDamageSetRequested += OnDamageSetRequested;
			}

			protected override void OnViewModelUnbound(SpiderViewModel viewModel) {
				base.OnViewModelUnbound(viewModel);
				
				viewModel.OnMovementSpeedSetRequested -= OnMovementSpeedSetRequested;
				viewModel.OnDamageSetRequested -= OnDamageSetRequested;	
			}

			protected virtual void OnMovementSpeedSetRequested(SpiderViewModel viewModel, float value) {
				Assert.That(ViewModelStateMachinePairs.TryGetValue(viewModel, out IStateMachine<TState> state))
						.Throws("Recieved event call from an unknown ViewModel.");
			
				state.CurrentState.OnMovementSpeedSetRequested(value);
			}

			protected virtual void OnDamageSetRequested(SpiderViewModel viewModel, float value) {
				Assert.That(ViewModelStateMachinePairs.TryGetValue(viewModel, out IStateMachine<TState> state))
						.Throws("Recieved event call from an unknown ViewModel.");
			
				state.CurrentState.OnDamageSetRequested(value);
			}
		}

		public new abstract class Presenter : Presenter<State> { 
		   
		}

		public new abstract class State : PoolableEntityViewModel<SpiderViewModel>.State {
			protected float MovementSpeed {
				get => ViewModel.MovementSpeed;
				set => ViewModel.MovementSpeed = value;
}

			protected float Damage {
				get => ViewModel.Damage;
				set => ViewModel.Damage = value;
}

			protected State(SpiderViewModel viewModel) : base(viewModel) {
				
			}

			public virtual void OnMovementSpeedSetRequested(float value) {
				
			}

			public virtual void OnDamageSetRequested(float value) {
				
			}
		}
	}
}