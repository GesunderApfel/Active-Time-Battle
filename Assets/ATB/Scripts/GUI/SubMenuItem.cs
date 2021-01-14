using UnityEngine;

/// <summary>
/// Element for the sub menu (either magic, bestia or skill)
/// </summary>
public class SubMenuItem : ScriptableObject
{
    public string subMenuItemName;
    public SubMenu subMenuType;
    public MagicType magicType;
}

