using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class PoolingProjectileData : PoolingItemData
{
    public Buff buff;
    public Define.UnitType unitType;
    public int damage;
    public float speed;
}

public class Pooling_Projectile : PoolingItem
{
    [HideInInspector] public Rigidbody2D rigidBody;
    [SerializeField] private string effectName;
    public PoolingProjectileData projectileData;

    protected Vector3 startPos;
    protected CancellationTokenSource throwCts;

    protected override void Awake()
    {
        base.Awake();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    public override void Init(PoolingItemData itemData)
    {
        base.Init(itemData);
        transform.rotation = Quaternion.identity;
        projectileData = itemData as PoolingProjectileData;
    }

    public virtual void ThrowProjectile(Vector2 targetPos = new Vector2())
    {
        startPos = rigidBody.position;
        throwCts?.Cancel();
        throwCts = new CancellationTokenSource();
        ThrowProjectileAsync(targetPos).Forget();
    }

    public override void Release()
    {
        base.Release();
        throwCts?.Cancel();
    }

    protected virtual async UniTask ThrowProjectileAsync(Vector2 targetPos)
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Unit>(out var unit))
        {
            if (unit.GetUnitType() != projectileData.unitType)
                CollisionAction(unit);
        }
        else if (collision.gameObject.CompareTag("Floor"))
            Release();
    }
    
    private void CollisionAction(Unit unit)
    {
        unit.GetDamage(projectileData.damage);
        if (projectileData.buff != null)
            unit.DOBuff(projectileData.buff).Forget();
        var effect = ObjectPool.instance.GetPoolingItem(effectName) as Pooling_Effect;
        effect.Init(new PoolingItemData()
        {
            name = effectName
        });
        effect.transform.position = unit.transform.position + Vector3.up * 0.5f;
        effect.Play();
        Release();
    }
}
