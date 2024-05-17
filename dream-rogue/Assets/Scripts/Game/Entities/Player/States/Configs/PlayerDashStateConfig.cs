using UnityEngine;

namespace SecretHostel.DreamRogue {
   [CreateAssetMenu(fileName = "PlayerRocketStateConfig", menuName = "Project Configuration/Entities/Player/States/Rocket")]
   public class PlayerDashStateConfig : ScriptableObject {
      [field: SerializeField, Min(0)] public float DashTimeSeconds { get; private set; }
      [field: SerializeField, Min(0)] public float DashDistance { get; private set; }
   }
}
