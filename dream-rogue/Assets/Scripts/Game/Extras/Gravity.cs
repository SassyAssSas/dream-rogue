using UnityEngine;

namespace SecretHostel.DreamRogue {
   [RequireComponent(typeof(Rigidbody))]
   public class CustomGravity : MonoBehaviour {
      [field: SerializeField] public float GravityScale { get; private set; }

      private const float GlobalGravity = -9.81f;

      private Rigidbody _rb;

      private void Reset() {
         GravityScale = 1f;
      }

      private void Awake() {
         _rb = GetComponent<Rigidbody>();
         _rb.useGravity = false;
      }

      private void FixedUpdate() {
         Vector3 gravity = GlobalGravity * GravityScale * Vector3.up;
         _rb.AddForce(gravity, ForceMode.Acceleration);
      }
   }
}
