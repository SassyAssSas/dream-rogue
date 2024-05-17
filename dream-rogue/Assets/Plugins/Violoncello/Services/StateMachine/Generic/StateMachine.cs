using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Violoncello.Utilities;

namespace Violoncello.Services.Generic {
   public class StateMachine : StateMachine<State>, IDisposable {

   }

   public class StateMachine<TState> : IDisposable, IStateMachine<TState> where TState : State {
      public bool IsDisposed { get; private set; }
      public TState CurrentState { get; private set; }

      private Dictionary<string, TState> states;
      private Dictionary<string, List<ExitCodeTargetScenePair>> transitions;

      public StateMachine() {
         states = new();
         transitions = new();
      }

      public void AddState(string key, TState state) {
         Assert.That(!string.IsNullOrEmpty(key))
               .Throws("Key was null or empty.");

         Assert.That(!states.ContainsKey(key))
               .Throws($"Key {key} was already taken by another state.");

         Assert.That(string.IsNullOrEmpty(state.UniqueIdentifier))
               .Throws("You can't bind the same state object to several state machines.");

         state.UniqueIdentifier = key;
         state.StateFinished += OnStateFinished;

         states.Add(key, state);
      }

      public void AddTransition(string from, string to, int stateExitCode) {
         if (transitions.TryGetValue(from, out List<ExitCodeTargetScenePair> pairs)) {
            Assert.That(!pairs.Any(transition => transition.ExitCode == stateExitCode))
                  .Throws("Attempted to add 2 transitions with the same exit code from one state.");
         }
         else {
            transitions.Add(from, new());
         }

         transitions[from].Add(new(to, stateExitCode));
      }

      private void OnStateFinished(State state, int exitCode) {
         state.QuitState();

         CurrentState = null;

         if (transitions.TryGetValue(state.UniqueIdentifier, out List<ExitCodeTargetScenePair> pairs)) {
            var pair = pairs.FirstOrDefault(pair => pair.ExitCode == exitCode);

            if (pair != null) {
               CurrentState = states[pair.To];

               CurrentState.EnterState();
            }
         }
      }

      public void SetState(string key) {
         Assert.That(states.TryGetValue(key, out TState newState))
               .Throws($"Couldn't find state with the {key} key.");

         CurrentState?.QuitState();

         CurrentState = newState;

         CurrentState.EnterState();
      }

      public void Dispose() {
         Assert.That(!IsDisposed)
               .LogError("Tried to dispose an object twice.");

         foreach (var state in states.Values) {
            state.StateFinished -= OnStateFinished;
         }

         states.Clear();

         IsDisposed = true;
      }

      private class ExitCodeTargetScenePair {
         public string To { get; }
         public int ExitCode { get; }

         public ExitCodeTargetScenePair(string to, int exitCode) {
            To = to;
            ExitCode = exitCode;
         }
      }
   }
}
