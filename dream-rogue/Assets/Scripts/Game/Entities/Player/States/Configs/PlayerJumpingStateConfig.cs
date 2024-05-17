using UnityEngine;

namespace SecretHostel.DreamRogue {
   [CreateAssetMenu(fileName = "PlayerJumpingStateConfig", menuName = "Project Configuration/Entities/Player/States/Jumping")]
   public class PlayerJumpingStateConfig : ScriptableObject {
      [field: SerializeField, Min(0)] public float HorizontalMovementSpeed { get; private set; }
      [field: SerializeField, Min(0)] public float RotationSpeed { get; private set; }
      [field: SerializeField, Min(0)] public float JumpForce { get; private set; }
   }
}
