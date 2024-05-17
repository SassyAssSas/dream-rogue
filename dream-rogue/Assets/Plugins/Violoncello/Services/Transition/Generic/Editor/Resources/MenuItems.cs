using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Violoncello.Services.TransitionsEditor {
   public static class MenuItems {
      private const string MonoEntityViewModelResourcesTemplatePath = "Prefabs/TransitionService";

      [MenuItem("MonoTransitionService", menuItem = "Assets/Create/Violoncello/Generic Services/Transition/Prefab", priority = 0)]
      public static void CreateMonoTransitionServicePrefab() {
         var asset = Resources.Load<GameObject>(MonoEntityViewModelResourcesTemplatePath);

         var selection = Selection.GetFiltered<Object>(SelectionMode.Assets).FirstOrDefault();

         var pathName = selection == null
            ? "Assets/TransitionService.prefab"
            : $"{AssetDatabase.GetAssetPath(selection)}/TransitionService.prefab";

         PrefabUtility.SaveAsPrefabAsset(asset, pathName); 
      }
   }
}
