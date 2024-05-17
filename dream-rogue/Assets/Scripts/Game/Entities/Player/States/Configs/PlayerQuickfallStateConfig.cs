using UnityEngine;

namespace SecretHostel.DreamRogue {
   [CreateAssetMenu(fileName = "PlayerQuickfallStateConfig", menuName = "Project Configuration/Entities/Player/States/Quickfall")]
   public class PlayerQuickfallStateConfig : ScriptableObject {
      [field: SerializeField, Min(0f)] public float FallVelocity { get; private set; }

      public void Reset() {
         FallVelocity = 20f;
      }
   }
}
