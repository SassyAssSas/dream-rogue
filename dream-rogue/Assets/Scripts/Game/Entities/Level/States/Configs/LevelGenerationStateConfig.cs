using UnityEngine;

namespace SecretHostel.DreamRogue {
	[CreateAssetMenu(fileName = "LevelGenerationStateConfig", menuName = "Project Configuration/Entities/Level/States/Generation")]
	public class LevelGenerationStateConfig : ScriptableObject {
		[field: SerializeField] public int MaxWidth { get; private set; }
		[field: SerializeField] public int MaxHeight { get; private set; }

      [field: SerializeField] public int ChunkSize { get; private set; }

      public void OnValidate() {
         if (ChunkSize % 2 != 0) {
            ChunkSize++;
            Debug.LogWarning("ChunkSize must be even.");
         }
      }
   } 
}