using Cysharp.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class PoolingSkillData : PoolingItemData
{
    public float incValue;
    public float coolTime;
    public int attackCount;
    public Define.SkillType skillType;
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

    public async UniTask UseSkill(Unit useUnit)
    {

    }
}
