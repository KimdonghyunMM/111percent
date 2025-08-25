using UnityEngine;

public partial class COMMON
{
    public static UnitsSO unitSo;
    private static void InitUserUnit()
    {
        unitSo = Resources.Load<UnitsSO>($"{Define.Folder.SCRIPTABLE_OBJECT}/UnitsSO");
    }

    [System.Serializable]
    public class UnitData
    {
        public int atk;
        public int hp;

        public UnitData(UnitData data)
        {
            atk = data.atk;
            hp = data.hp;
        }
    }

    [System.Serializable]
    public class UserData
    {
        public UnitData unitData;
        public Define.UnitType unitType;

        public UserData(Define.UnitType type)
        {
            unitType = type;
            switch (unitType)
            {
                case Define.UnitType.Player:
                    unitData = new UnitData(unitSo.playerData);
                    break;
                case Define.UnitType.Enemy:
                    unitData = new UnitData(unitSo.enemyData);
                    break;
            }
        }
    }
}
