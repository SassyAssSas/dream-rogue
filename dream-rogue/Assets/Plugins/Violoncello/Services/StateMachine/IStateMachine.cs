namespace Violoncello.Services {
   public interface IStateMachine<TState> where TState : State {
      public TState CurrentState { get; }

      public void AddState(string key, TState state);
      public void AddTransition(string from, string to, int stateExitCode);
      public void SetState(string key);
   }
}
