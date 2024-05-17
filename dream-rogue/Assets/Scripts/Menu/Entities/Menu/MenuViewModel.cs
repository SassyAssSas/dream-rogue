using System;
using System.Collections.Generic;
using UnityEngine;
using Violoncello.Services;
using Violoncello.Utilities;
using Violoncello.Structurization;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace SecretHostel.DreamRogue {
   public class MenuViewModel : EntityViewModel<MenuViewModel> {
      [SerializeField] private Button _playButton;
      [SerializeField] private Button _optionsButton;
      [SerializeField] private Button _exitButton;

      protected override void Start() {
         base.Start();

         _playButton.Select();
      }

      public new abstract class Presenter<TState> : EntityViewModel<MenuViewModel>.Presenter<TState> where TState : State {
         protected override void OnViewModelBound(MenuViewModel viewModel, IStateMachine<TState> stateMachine) {
            base.OnViewModelBound(viewModel, stateMachine);

            viewModel._playButton.onClick.AddListener(OnPlayButtonPressed);
            viewModel._optionsButton.onClick.AddListener(OnOptionsButtonPressed);
            viewModel._exitButton.onClick.AddListener(OnExitButtonPressed);

            
         }

         protected override void OnViewModelUnbound(MenuViewModel viewModel) {
            base.OnViewModelUnbound(viewModel);

            viewModel._playButton.onClick.RemoveListener(OnPlayButtonPressed);
            viewModel._optionsButton.onClick.RemoveListener(OnOptionsButtonPressed);
            viewModel._exitButton.onClick.RemoveListener(OnExitButtonPressed);
         }

         protected virtual void OnPlayButtonPressed() {

         }

         protected virtual void OnOptionsButtonPressed() {

         }

         protected virtual void OnExitButtonPressed() {

         }
      }

      public new abstract class Presenter : Presenter<State> {

      }

      public new abstract class State : EntityViewModel<MenuViewModel>.State {


         protected State(MenuViewModel viewModel) : base(viewModel) {

         }
      }
   }
}