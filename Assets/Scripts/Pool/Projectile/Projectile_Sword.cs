using Cysharp.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Projectile_Sword : Pooling_Projectile
{
    [SerializeField] private float targetHeight;

    protected override async UniTask ThrowProjectileAsync(Vector2 targetPos)
    {
        await base.ThrowProjectileAsync(targetPos);

        var startX = startPos.x;
        var targetX = targetPos.x;
        var mag = targetX - startX;
        var nextPos = Vector2.zero;
        while (nextPos != targetPos)
        {
            var nextX = Mathf.MoveTowards(rigidBody.position.x, targetX, projectileData.speed * Time.deltaTime);
            var height = Mathf.Lerp(startPos.y, targetPos.y, (nextX - startX) / mag);
            var arc = targetHeight * (nextX - startX) * (nextX - targetX) / (-0.25f * Mathf.Pow(mag, 2));
            nextPos = new Vector2(nextX, height + arc);

            transform.rotation = LookAt(nextPos - rigidBody.position);
            rigidBody.position = nextPos;

            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: throwCts.Token);
        }

        Release();
    }

    private Quaternion LookAt(Vector2 forward)
    {
        var vec = Vector3.forward * Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(vec);
    }
}
