using Cysharp.Threading.Tasks;
using UnityEngine;

public class Skill_Heal : Pooling_Skill
{
    public override async UniTask UseSkill(Unit useUnit)
    {
        await base.UseSkill(useUnit);
        useUnit.GetDamage(skillData.incValue * useUnit.data.unitData.atk, Define.DamageType.Heal);
        var effect = ObjectPool.instance.GetPoolingItem("HealEffect") as Pooling_Effect;
        effect.Init(new PoolingItemData()
        {
            name = "HealEffect"
        });
        effect.transform.position = useUnit.transform.position + Vector3.up * 0.5f;
        effect.Play();
        Release();
    }
}
