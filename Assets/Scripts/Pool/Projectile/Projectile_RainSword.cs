using Cysharp.Threading.Tasks;
using UnityEngine;

public class Projectile_RainSword : Pooling_Projectile
{
    protected override async UniTask ThrowProjectileAsync(Vector2 targetPos)
    {
        await base.ThrowProjectileAsync(targetPos);

        var dir = targetPos - rigidBody.position;
        var euler = Vector3.forward * Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(euler);
        var timeCount = 0f;
        while (timeCount <= 5f)
        {
            timeCount += Time.deltaTime;
            rigidBody.position += dir.normalized * projectileData.speed * Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: throwCts.Token);
        }

        Release();
    }
}
