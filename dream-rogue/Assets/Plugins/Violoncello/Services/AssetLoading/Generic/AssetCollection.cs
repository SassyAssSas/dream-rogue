using System.Collections.Generic;
using UnityEngine;

namespace Violoncello.Services.Generic {
   [CreateAssetMenu(fileName = "New AssetCollection", menuName = "Violoncello/Assets Management/Asset Collection")]
   public class AssetCollection : ScriptableObject {
      [field:SerializeField] public string Name { get; private set; }

      [SerializeField] private List<AssetLink> _assets;
      public IReadOnlyList<AssetLink> AssetLinks => _assets;
   }
}
