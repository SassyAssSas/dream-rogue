namespace Violoncello.Structurization {
   public abstract class EntityPresenter<TViewModel> where TViewModel : EntityViewModel<TViewModel> {
      public abstract void BindViewModel(TViewModel viewModel);
      public abstract void UnbindViewModel(TViewModel viewModel);
   }
}
