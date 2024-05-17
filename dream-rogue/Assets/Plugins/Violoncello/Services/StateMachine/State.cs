using System;

namespace Violoncello.Services {
   public abstract class State {
      internal string UniqueIdentifier { get; set; }
      
      internal event Action<State, int> StateFinished;

      public virtual void EnterState() {
      
      }

      public virtual void QuitState() {
      
      }

      public virtual void Tick(float deltaTime) {
         
      }

      public virtual void FixedTick(float deltaTime) {

      }

      protected void FinishState(int exitCode) {
         StateFinished?.Invoke(this, exitCode);
      }
   }
}
