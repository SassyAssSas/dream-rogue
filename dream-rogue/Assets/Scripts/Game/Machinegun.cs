using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace SecretHostel.DreamRogue {
   public class Machinegun : MonoBehaviour {
      [SerializeField, Min(1)] private int shootsPerSecond;

      private BulletViewModel.Pool _bulletPool;

      private Transform _player;

      [Inject]
      public void Construct(BulletViewModel.Pool bulletPool) {
         _bulletPool = bulletPool;
      }

      private void Awake() {
         Shoot().Forget();

         _player = FindObjectOfType<PlayerViewModel>().transform;
      }

      private async UniTaskVoid Shoot() {
         var delay = 1f / shootsPerSecond;
         const float bias = 0.4f;

         while (true) {
            transform.LookAt(_player);

            var direction = Quaternion.Euler(Random.Range(-bias, bias), Random.Range(-bias, bias), 0f) * transform.forward;

            _bulletPool.SpawnAndInitialize(transform.position + transform.forward / 2f, direction);

            await UniTask.WaitForSeconds(delay);
         }
      }
   }
}
