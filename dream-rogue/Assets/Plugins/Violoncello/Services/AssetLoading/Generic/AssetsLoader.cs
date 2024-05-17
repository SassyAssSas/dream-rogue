using System.Linq;
using UnityEngine;
using Violoncello.Utilities;

namespace Violoncello.Services.Generic {
   public class AssetsLoader : IAssetsLoader {    
      private AssetsLoaderLinks _assetLinks;

      public AssetsLoader() {
         _assetLinks = Resources.Load<AssetsLoaderLinks>("AssetsLoaderLinks");

         Assert.IsNotNull(_assetLinks)
               .Throws("Couldn't find AssetCollection in the resources folder.");
      }

      public T Load<T>(string collectionName, string name) where T : Object {
         Assert.That(TryGetCollection(collectionName, out AssetCollection collection))
               .Throws($"Couldn't find assets collection named {collectionName}");

         var link = collection.AssetLinks.FirstOrDefault(asset => asset.Name == name);

         Assert.That(link.Object is T)
               .Throws($"Couldn't find asset with a {name} name and {typeof(T)} type.");

         return link.Object as T;
      }

      public bool TryLoad<T>(string collectionName, string name, out T asset) where T : Object {
         asset = null;

         if (!TryGetCollection(collectionName, out AssetCollection collection)) {
            var link = collection.AssetLinks.FirstOrDefault(asset => asset.Name == name);

            asset = link.Object as T;
         }

         return asset != null;
      }

      private bool TryGetCollection(string name, out AssetCollection collection) {
         collection = _assetLinks.AssetCollections.FirstOrDefault(collection => collection.Name == name);

         return collection != null;
      }
   }
}
