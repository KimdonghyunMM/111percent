using Cysharp.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class PoolingSkillData : PoolingItemData
{
    public float incValue;
    public float coolTime;
    public int attackCount;
    public Buff buff;
}

[System.Serializable]
public class Buff
{
    public float percent;
    public float incValue;
    public float term;
    public float duration;
    public Define.BuffType buffType;
}

public class Pooling_Skill : PoolingItem
{
    protected PoolingSkillData skillData;
    protected Unit useUnit;

    public override void Init(PoolingItemData itemData)
    {
        base.Init(itemData);
        skillData = itemData as PoolingSkillData;
    }

    public virtual async UniTask UseSkill(Unit useUnit) => this.useUnit = useUnit;
    protected virtual PoolingProjectileData SetProjectileData()
    {
        var projData = new PoolingProjectileData();
        projData.unitType = useUnit.GetUnitType();
        projData.damage = Mathf.RoundToInt(useUnit.data.unitData.atk * skillData.incValue);
        projData.unitType = useUnit.data.unitType;
        projData.buff = skillData.buff;
        return projData;
    }
}
