using UnityEngine;

public static class COMMON
{
    public static UnitsSO unitSo;
    public static void Init()
    {
        unitSo = Resources.Load<UnitsSO>($"{Define.Folder.SCRIPTABLE_OBJECT}/UnitsSO");
    }

    [System.Serializable]
    public class UnitData
    {
        public int atk;
        public int hp;
    }
}
