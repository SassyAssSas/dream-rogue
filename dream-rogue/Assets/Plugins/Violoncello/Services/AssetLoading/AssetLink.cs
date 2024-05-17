using UnityEngine;

namespace Violoncello.Services {
   [System.Serializable]
   public struct AssetLink {
      [field: SerializeField] public string Name { get; private set; }
      [field: SerializeField] public Object Object { get; private set; }

      public AssetLink(string name, Object obj) {
         Name = name;
         Object = obj;
      }
   }
}
