using UnityEngine;

namespace SecretHostel.DreamRogue {
	[CreateAssetMenu(fileName = "BulletActiveStateConfig", menuName = "Project Configuration/Entities/Bullet/States/Active")]
	public class BulletActiveStateConfig : ScriptableObject {
		[field: SerializeField] public float MovementSpeed { get; set; }
      [field: SerializeField] public float MaxLength { get; set; }

      public void Reset() {
         MovementSpeed = 10f;
         MaxLength = 0.5f;
      }
   } 
}