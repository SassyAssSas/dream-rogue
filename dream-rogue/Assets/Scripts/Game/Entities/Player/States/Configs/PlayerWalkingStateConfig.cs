using UnityEngine;

namespace SecretHostel.DreamRogue {
	[CreateAssetMenu(fileName = "PlayerMovementStateConfig", menuName = "Project Configuration/Entities/Player/States/Movement")]
	public class PlayerWalkingStateConfig : ScriptableObject {
		[field: SerializeField, Min(0)] public float MovementSpeed { get; private set; }
      [field: SerializeField, Min(0)] public float RotationSpeed { get; private set; }

      public void Reset() {
         MovementSpeed = 12f;
         RotationSpeed = 20f;
      }
   } 
}