using UnityEngine;

public class PoolingProjectileData : PoolingItemData
{
    public Define.UnitType unitType;
    public int damage;
    public float speed;
}

public class Pooling_Projectile : PoolingItem
{
    [HideInInspector] public Rigidbody2D rigidBody;
    protected PoolingProjectileData projectileData;

    protected override void Awake()
    {
        base.Awake();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    public virtual void ThrowProjectile(Vector2 targetPos)
    {

    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Unit>(out var unit))
        {
            if (unit.GetUnitType() != projectileData.unitType)
            {
                Debug.Log($"{collision.gameObject.name}ø° {projectileData.damage} ¿‘»˚");
                unit.GetDamage(projectileData.damage);
                Release();
            }
        }
    }
}
