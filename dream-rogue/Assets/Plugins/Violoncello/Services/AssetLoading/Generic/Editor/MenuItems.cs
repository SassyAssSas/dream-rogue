using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Violoncello.Services.Generic.Editor {
   public static class MenuItems {
      [MenuItem("AssetLoaderLinks", menuItem = "Assets/Create/Violoncello/Assets Management/Asset Loader Links")]
      public static void CreateAssetsList() {
         var asset = ScriptableObject.CreateInstance<AssetsLoaderLinks>();

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
               Debug.LogWarning("AssetCollection must be placed directly inside Resources folder.");
               return;
            }
         }

         var fullPath = $"{folderPath}/AssetsCollection.asset";

         AssetDatabase.CreateAsset(asset, fullPath);
      }
   }
}
