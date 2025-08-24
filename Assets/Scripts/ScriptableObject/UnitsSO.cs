using UnityEngine;
using static COMMON;

[CreateAssetMenu(fileName = "UnitsSO", menuName = "Scriptable Objects/UnitsSO")]
public class UnitsSO : ScriptableObject
{
    public UnitData playerData;
    public UnitData enemyData;
}
