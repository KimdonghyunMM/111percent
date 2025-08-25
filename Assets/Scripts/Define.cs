public class Define
{
    public readonly struct Folder
    {
        public static readonly string PREFAB = "Prefabs/ItemPool";
        public static readonly string SCRIPTABLE_OBJECT = "Prefabs/ScriptableObject";
    }


    public enum UnitType
    {
        Player,
        Enemy
    }

    public enum DamageType
    {
        Damage,
        Heal,
    }

    public enum BuffType
    {
        DotHeal,
        DotDamage,
        Buff,
        DeBuff
    }
}
