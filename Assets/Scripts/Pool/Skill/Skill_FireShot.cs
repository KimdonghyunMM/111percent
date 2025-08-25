using Cysharp.Threading.Tasks;
using UnityEngine;

public class Skill_FireShot : Pooling_Skill
{
    public override async UniTask UseSkill(Unit useUnit)
    {
        await base.UseSkill(useUnit);
        var proj = COMMON.GetProjectile("FireArrow");
        proj.Init(SetProjectileData);
        proj.rigidBody.position = useUnit.muzzle.position;
        proj.ThrowProjectile();
        await UniTask.Delay(5000);
        Release();
    }

    protected override PoolingProjectileData SetProjectileData()
    {
        var projData = base.SetProjectileData();

        projData.name = "FireArrow";
        projData.speed = 10f;

        return projData;
    }
}
