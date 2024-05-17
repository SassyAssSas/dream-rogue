namespace Violoncello.Services {
   public interface ITransitionService {
      public void Transition(string targetSceneName, TransitionOptions options = null);
      public T GetExtra<T>(string key) where T : class;
   }
}