using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Violoncello.Structurization.Editor {
   public class EntityDefaultSettings : ScriptableObject {
      [field: SerializeField] public string Namespace { get; private set; }
      [field: SerializeField] public bool CreateInstaller { get; private set; }
      [field: SerializeField] public bool Poolable { get; private set; }

      public void Reset() {
         Namespace = string.Empty;
         CreateInstaller = true;
         Poolable = true;
      }

      [MenuItem("EntityDefaultSettings", menuItem = "Assets/Create/Violoncello/Entity Default Settings", priority = -1, secondaryPriority = 0)]
      public static void CreateAssetsList() {
         var asset = CreateInstance<EntityDefaultSettings>();

         var selection = Selection.GetFiltered<Object>(SelectionMode.Assets).FirstOrDefault();

         string folderPath;

         if (selection == null) {
            if (!AssetDatabase.IsValidFolder("Assets/Resources")) {
               AssetDatabase.CreateFolder("Assets", "Resources");
            }

            folderPath = "Assets/Resources";
         }
         else {
            folderPath = $"{AssetDatabase.GetAssetPath(selection)}";

            if (folderPath.Split('/')[^1] != "Resources") {
               Debug.LogWarning("Entity Default Settings must be placed directly inside the Resources folder.");
               return;
            }
         }

         var fullPath = $"{folderPath}/EntityDefaultSettings.asset";

         AssetDatabase.CreateAsset(asset, fullPath);
      }
   }
}
