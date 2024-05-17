using System;
using UnityEngine;
using Violoncello.Services;
using Violoncello.Structurization;
using UnityEngine.EventSystems;
using TMPro;
using JetBrains.Annotations;
using Violoncello.Utilities;
using Violoncello.Services.Generic;

namespace SecretHostel.DreamRogue {
   public class ButtonViewModel : EntityViewModel<ButtonViewModel>, ISelectHandler, IDeselectHandler {
      private TextMeshProUGUI _tmp;

      public string CurrentText {
         get => _tmp.text;
         private set => _tmp.text = value;
      }

      public string RawText { get; private set; }

      private event Action<ButtonViewModel> OnSelected;
      private event Action<ButtonViewModel> OnDeselected;

      private void Awake() {
         _tmp = GetComponentInChildren<TextMeshProUGUI>();
         RawText = CurrentText;
      }

      public void OnSelect(BaseEventData eventData) {
         OnSelected?.Invoke(this);
      }

      public void OnDeselect(BaseEventData eventData) {
         OnDeselected?.Invoke(this);
      }

      public new abstract class Presenter<TState> : EntityViewModel<ButtonViewModel>.Presenter<TState> where TState : State {
         protected override void OnViewModelBound(ButtonViewModel viewModel, IStateMachine<TState> stateMachine) {
            base.OnViewModelBound(viewModel, stateMachine);

            viewModel.OnSelected += OnSelected;
            viewModel.OnDeselected += OnDeselected;
         }

         protected override void OnViewModelUnbound(ButtonViewModel viewModel) {
            base.OnViewModelUnbound(viewModel);

            viewModel.OnSelected -= OnSelected;
            viewModel.OnDeselected -= OnDeselected;
         }

         protected virtual void OnSelected(ButtonViewModel viewModel) {
            Assert.That(ViewModelStateMachinePairs.TryGetValue(viewModel, out IStateMachine<TState> stateMachine))
                  .Throws("Recieved an event call from unknown ButtonViewModel");

            stateMachine.CurrentState?.OnSelected();
         }

         protected virtual void OnDeselected(ButtonViewModel viewModel) {
            Assert.That(ViewModelStateMachinePairs.TryGetValue(viewModel, out IStateMachine<TState> stateMachine))
                  .Throws("Recieved an event call from unknown ButtonViewModel");

            stateMachine.CurrentState?.OnDeselected();
         }
      }

      public new abstract class Presenter : Presenter<State> {

      }

      public new abstract class State : EntityViewModel<ButtonViewModel>.State {
         protected string Text {
            get => ViewModel.CurrentText;
            set => ViewModel.CurrentText = value;
         }

         protected string RawText {
            get => ViewModel.RawText;
            set => ViewModel.RawText = value;
         }

         protected State(ButtonViewModel viewModel) : base(viewModel) {

         }

         public virtual void OnSelected() {

         }

         public virtual void OnDeselected() {

         }
      }
   }
}