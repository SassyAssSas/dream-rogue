using System;
using System.Collections.Generic;
using UnityEngine;
using Violoncello.Services;
using Violoncello.Utilities;
using Violoncello.Structurization;

namespace SecretHostel.DreamRogue {
	public class CommandRunnerViewModel : EntityViewModel<CommandRunnerViewModel> {
		[SerializeField] private GameObject _inputField;

		public new abstract class Presenter<TState> : EntityViewModel<CommandRunnerViewModel>.Presenter<TState> where TState : State {
			protected override void OnViewModelBound(CommandRunnerViewModel viewModel, IStateMachine<TState> stateMachine) {
				base.OnViewModelBound(viewModel, stateMachine);
				

			}

			protected override void OnViewModelUnbound(CommandRunnerViewModel viewModel) {
				base.OnViewModelUnbound(viewModel);
				
	
			}


		}

		public new abstract class Presenter : Presenter<State> { 
		   
		}

		public new abstract class State : EntityViewModel<CommandRunnerViewModel>.State {
			protected GameObject InputField => ViewModel._inputField;

			protected State(CommandRunnerViewModel viewModel) : base(viewModel) {
				
			}
		}
	}
}