using UnityEngine;

namespace Violoncello.Services {
   public interface IAssetsLoader {
      public T Load<T>(string collectionName, string assetName) where T : Object;
      public bool TryLoad<T>(string collectionName, string assetName, out T asset) where T : Object;
   }
}
