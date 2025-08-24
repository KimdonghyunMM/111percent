using System;
using UnityEngine;

public class PoolingItemData
{
    public string name;
}

public class PoolingItem : MonoBehaviour
{
    public string key;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {

    }

    public virtual void Init(PoolingItemData itemData)
    {
        key = itemData.name;
        gameObject.SetActive(true);
    }

    public void Init(Func<PoolingItemData> itemData)
    {
        Init(itemData.Invoke());
    }

    public virtual void Release() => ObjectPool.instance.Release(this);
}
