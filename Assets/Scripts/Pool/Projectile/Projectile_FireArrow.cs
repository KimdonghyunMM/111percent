using Cysharp.Threading.Tasks;
using UnityEngine;

public class Projectile_FireArrow : Pooling_Projectile
{
    protected override async UniTask ThrowProjectileAsync(Vector2 targetPos)
    {
        await base.ThrowProjectileAsync(targetPos);

        var dir = projectileData.unitType == Define.UnitType.Player ? Vector2.right : Vector2.left;
        transform.localScale = projectileData.unitType == Define.UnitType.Player ? new Vector3(1.5f, 1f, 0.5f) : new Vector3(-1.5f, 1f, 0.5f);
        var timeCount = 0f;
        while (timeCount <= 5f)
        {
            timeCount += Time.deltaTime;
            rigidBody.position += dir * projectileData.speed * Time.deltaTime;
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: throwCts.Token);
        }

        Release();
    }
}
