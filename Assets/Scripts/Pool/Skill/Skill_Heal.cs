using Cysharp.Threading.Tasks;
using UnityEngine;

public class Skill_Heal : Pooling_Skill
{
    public override async UniTask UseSkill(Unit useUnit)
    {
        await base.UseSkill(useUnit);
        useUnit.GetDamage(skillData.incValue * useUnit.data.unitData.atk, Define.DamageType.Heal);
        Release();
    }
}
