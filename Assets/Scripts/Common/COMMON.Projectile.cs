using UnityEngine;

public partial class COMMON
{
    private static void InitProjectile()
    {

    }

    public static Pooling_Projectile GetProjectile(string name, Transform partrans = null) =>
        ObjectPool.instance.GetPoolingItem(name, partrans) as Pooling_Projectile;
}
