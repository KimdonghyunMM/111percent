using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class Projectile_Sword : Pooling_Projectile
{
    [SerializeField] private float targetHeight;

    private Vector3 startPos;
    private CancellationTokenSource throwCts;

    public override void ThrowProjectile(Vector2 targetPos)
    {
        base.ThrowProjectile(targetPos);
        startPos = rigidBody.position;
        ThrowProjectileAsync(targetPos).Forget();
    }

    private async UniTask ThrowProjectileAsync(Vector2 targetPos)
    {
        throwCts = new CancellationTokenSource();
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
        return Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
    }

    public override void Init(PoolingItemData itemData)
    {
        base.Init(itemData);
        projectileData = itemData as PoolingProjectileData;
        transform.rotation = Quaternion.identity;
    }

    public override void Release()
    {
        base.Release();
        throwCts?.Cancel();
    }
}
