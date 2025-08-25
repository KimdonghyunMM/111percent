using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class COMMON
{
    private static SkillSO skillSO;
    private static Dictionary<string, PoolingSkillData> dataDict = new Dictionary<string, PoolingSkillData>();

    private static void InitSkill()
    {
        skillSO = Resources.Load<SkillSO>($"{Define.Folder.SCRIPTABLE_OBJECT}/SkillSO");
        dataDict = skillSO.skillList.ToDictionary(x => x.name, x => x);
    }

    public static PoolingSkillData GetSkillData(string name) => dataDict[name];

    public static Pooling_Skill GetSkill(string name, Transform parTrans = null)
        => ObjectPool.instance.GetPoolingItem(name, parTrans) as Pooling_Skill;
}
