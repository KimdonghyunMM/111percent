using DG.Tweening;
using TMPro;
using UnityEngine;

public class PoolingTextData : PoolingItemData
{
    public Define.DamageType damageType;
    public int value;
}

public class Pooling_Text : PoolingItem
{
    [SerializeField] private TextMeshPro tmp;
    private PoolingTextData textData;

    protected override void Awake()
    {
        base.Awake();
        tmp = GetComponent<TextMeshPro>();
    }

    public override void Init(PoolingItemData itemData)
    {
        base.Init(itemData);
        textData = itemData as PoolingTextData;
        switch (textData.damageType)
        {
            case Define.DamageType.Damage:
                tmp.color = Color.red;
                tmp.text = "-";
                break;
            case Define.DamageType.Heal:
                tmp.color = Color.green;
                tmp.text = "+";
                break;
        }
        tmp.text += $"{textData.value}";
    }

    public void Play()
    {
        var isPlus = Random.Range(0, 2) == 1;
        var targetPosX = isPlus ? 1f : -1f;
        var targetPos = transform.position + Vector3.right * targetPosX;

        transform.localScale = Vector3.one;
        transform.DOScale(Vector3.one * 0.75f, 0.5f).SetEase(Ease.Linear);
        transform.DOJump(targetPos, 1f, 1, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            Release();
        });
    }
}
