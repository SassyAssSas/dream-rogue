using UnityEngine;

[ExecuteInEditMode]
public class ShaderHandler : MonoBehaviour {
   [SerializeField] private Material _effectMaterial;

   private void OnRenderImage(RenderTexture source, RenderTexture destination) {
      Graphics.Blit(source, destination, _effectMaterial);
   }
}
