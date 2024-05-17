using System;
using System.Collections.Generic;
using UnityEngine;
using Violoncello.Services;
using Violoncello.Utilities;
using Violoncello.Structurization;

namespace SecretHostel.DreamRogue {
	[RequireComponent(typeof(LineRenderer))]
	public class BulletViewModel : PoolableEntityViewModel<BulletViewModel> {
		private LineRenderer _lineRenderer;

		public float Damage { get; private set; }
      private event Action<BulletViewModel, float> OnDamageSetRequested;

      public Vector3 Direction { get; private set; }

      private void Awake() {
			_lineRenderer = GetComponent<LineRenderer>();
      }

      public void RequestDamageSet(float value) {
			OnDamageSetRequested?.Invoke(this, value);
		}

		public new class Pool : PoolableEntityViewModel<BulletViewModel>.Pool {
			public BulletViewModel SpawnAndInitialize(Vector3 position, Vector3 direction) {
				var bullet = Spawn();

				for (int i = 0; i < bullet._lineRenderer.positionCount; i++) {
					bullet._lineRenderer.SetPosition(i, position);
				}

				bullet.Direction = direction.normalized;

				return bullet;
			}	
		}

      public new abstract class Presenter<TState> : PoolableEntityViewModel<BulletViewModel>.Presenter<TState> where TState : State {
			protected override void OnViewModelBound(BulletViewModel viewModel, IStateMachine<TState> stateMachine) {
				base.OnViewModelBound(viewModel, stateMachine);
				
				viewModel.OnDamageSetRequested += OnDamageSetRequested;
			}

			protected override void OnViewModelUnbound(BulletViewModel viewModel) {
				base.OnViewModelUnbound(viewModel);
				
				viewModel.OnDamageSetRequested -= OnDamageSetRequested;	
			}

			protected virtual void OnDamageSetRequested(BulletViewModel viewModel, float value) {
				Assert.That(ViewModelStateMachinePairs.TryGetValue(viewModel, out IStateMachine<TState> state))
						.Throws("Recieved event call from an unknown ViewModel.");
			
				state.CurrentState.OnDamageSetRequested(value);
			}
		}
		
		public new abstract class Presenter : Presenter<State> { 
		   
		}

		public new abstract class State : PoolableEntityViewModel<BulletViewModel>.State {
			protected LineRenderer LineRenderer => ViewModel._lineRenderer;

			protected float Damage {
				get => ViewModel.Damage;
				set => ViewModel.Damage = value;
			}

         protected Vector3 Direction {
            get => ViewModel.Direction;
            set => ViewModel.Direction = value;
         }

         protected State(BulletViewModel viewModel) : base(viewModel) {
				
			}

			public virtual void OnDamageSetRequested(float value) {
				
			}
		}
	}
}