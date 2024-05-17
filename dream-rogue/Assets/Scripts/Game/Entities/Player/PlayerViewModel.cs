using System;
using UnityEngine;
using Violoncello.Services;
using Violoncello.Utilities;
using Violoncello.Structurization;

namespace SecretHostel.DreamRogue {
	[RequireComponent(typeof(Rigidbody))]
	public class PlayerViewModel : EntityViewModel<PlayerViewModel> {
		public float Health { get; private set; }
		private event Action<PlayerViewModel, float> OnHealthSetRequested;

		public float MaxHealth { get; private set; }
		private event Action<PlayerViewModel, float> OnMaxHealthSetRequested;

		private float timeSinceDash;

      private Rigidbody _rigidBody;

      private void Awake() {
			_rigidBody = GetComponent<Rigidbody>();
      }

      public void RequestHealthSet(float value) {
			OnHealthSetRequested?.Invoke(this, value);
		}

		public void RequestMaxHealthSet(float value) {
			OnMaxHealthSetRequested?.Invoke(this, value);
		}

		public new abstract class Presenter<TState> : EntityViewModel<PlayerViewModel>.Presenter<TState> where TState : State {
			protected override void OnViewModelBound(PlayerViewModel viewModel, IStateMachine<TState> stateMachine) {
				base.OnViewModelBound(viewModel, stateMachine);
				
				viewModel.OnHealthSetRequested += OnHealthSetRequested;
				viewModel.OnMaxHealthSetRequested += OnMaxHealthSetRequested;
			}

			protected override void OnViewModelUnbound(PlayerViewModel viewModel) {
				base.OnViewModelUnbound(viewModel);
				
				viewModel.OnHealthSetRequested -= OnHealthSetRequested;
				viewModel.OnMaxHealthSetRequested -= OnMaxHealthSetRequested;	
			}

			protected virtual void OnHealthSetRequested(PlayerViewModel viewModel, float value) {
				Assert.That(ViewModelStateMachinePairs.TryGetValue(viewModel, out IStateMachine<TState> state))
						.Throws("Recieved event call from an unknown ViewModel.");
			
				state.CurrentState.OnHealthSetRequested(value);
			}

			protected virtual void OnMaxHealthSetRequested(PlayerViewModel viewModel, float value) {
				Assert.That(ViewModelStateMachinePairs.TryGetValue(viewModel, out IStateMachine<TState> state))
						.Throws("Recieved event call from an unknown ViewModel.");
			
				state.CurrentState.OnMaxHealthSetRequested(value);
			}
		}

		public new abstract class Presenter : Presenter<State> { 
		   
		}

		public new abstract class State : EntityViewModel<PlayerViewModel>.State {
			protected float Health {
				get => ViewModel.Health;
				set => ViewModel.Health = value;
			}

			protected float MaxHealth {
				get => ViewModel.MaxHealth;
				set => ViewModel.MaxHealth = value;
			}

         protected float TimeSinceDash {
            get => ViewModel.timeSinceDash;
            set => ViewModel.timeSinceDash = value;
         }

         protected Rigidbody Rigidbody {
            get => ViewModel._rigidBody;
         }

         protected State(PlayerViewModel viewModel) : base(viewModel) {
				
			}

			public virtual void OnHealthSetRequested(float value) {
				
			}

			public virtual void OnMaxHealthSetRequested(float value) {
				
			}
		}
	}
}