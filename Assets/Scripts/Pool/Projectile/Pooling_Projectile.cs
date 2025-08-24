using UnityEngine;

public class PoolingProjectileData : PoolingItemData
{
    public int damage;
    public float speed;
}

public class Pooling_Projectile : PoolingItem
{
    public PoolingProjectileData projectileData;

    public virtual void ThrowProjectile(Vector3 targetPos)
    {

    }
}
