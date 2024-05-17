using UnityEngine;
using System.Collections.Generic;

namespace Violoncello.Services.Generic {
   public class AssetsLoaderLinks : ScriptableObject {
      [SerializeField] private List<AssetCollection> _assetCollections;
      public IReadOnlyList<AssetCollection> AssetCollections => _assetCollections;
   }
}
