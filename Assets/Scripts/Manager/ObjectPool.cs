using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    private Dictionary<string, Queue<PoolingItem>> pool = new Dictionary<string, Queue<PoolingItem>>();
    private Dictionary<string, PoolingItem> prefabDict = new Dictionary<string, PoolingItem>();
    private void Awake()
    {
        //╫л╠шео
        if (instance == null)
            instance = this;

        prefabDict = Resources.LoadAll<PoolingItem>(Define.Folder.PREFAB).ToDictionary(x => x.name);
        foreach (var item in prefabDict)
            pool.Add(item.Key, new Queue<PoolingItem>());
    }

    public PoolingItem GetPoolingItem(string name, Transform parTrans = null)
    {
        parTrans = parTrans == null ? transform : parTrans;
        if (pool[name].TryDequeue(out var item))
        {
            item.transform.SetParent(parTrans);
        }
        else
        {
            item = Instantiate(prefabDict[name], parTrans);
        }
        return item;
    }

    public void Release(PoolingItem item)
    {
        item.gameObject.SetActive(false);
        pool[item.key].Enqueue(item);
    }
}
