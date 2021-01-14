using UnityEngine;

/// <summary>
/// Holds magic information as well as the particle effect
/// </summary>
[SerializeField, CreateAssetMenu(fileName = "Magic", menuName = "Final Fantasy Battle/Add Magic Attack")]
public class Magic : SubMenuItem
{
    public int damage;
    public int manaCost;
    public GameObject effect;
    public string infoText;

}

/// <summary>
/// Debuff = select first enemy -> debuff equals Damage Dealing Magic
/// Buff = select first party member
/// </summary>
public enum MagicType
{
    Debuff = 0,
    Buff = 1,
}
