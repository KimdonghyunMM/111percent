using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

public class Skill_RainArrow : Pooling_Skill
{
    public override async UniTask UseSkill(Unit useUnit)
    {
        await base.UseSkill(useUnit);

        var targetPos = useUnit.GetTargetPos();
        for (var i = 0; i < skillData.attackCount; i++)
        {
            var startPos = targetPos + Vector2.up * 10 + Vector2.right * Random.Range(-1f, 1f) * 5f;
            var proj = COMMON.GetProjectile("RainSword");
            proj.transform.position = startPos;
            proj.rigidBody.position = startPos;
            proj.Init(SetProjectileData);
            proj.ThrowProjectile(targetPos);
            await UniTask.Delay(100);
        }

        Release();
    }

    protected override PoolingProjectileData SetProjectileData()
    {
        var projData = base.SetProjectileData();

        projData.name = "RainSword";
        projData.speed = 10f;

        return projData;
    }
}
