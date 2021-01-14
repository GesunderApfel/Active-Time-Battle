using UnityEngine;
/// <summary>
/// Manages all the calculations and effects (like adding a Buff)
/// For now: just simple health point calculations, because balancing would break the scope of the prototype
/// </summary>
public class BattleRuleBook
{
    /// <summary>
    /// applys damage to the target and returns the damage for further calculation
    /// </summary>
    public int DoDamage(Fighter fighter, Fighter target)
    {
        int damage = Mathf.Clamp(fighter.attack - target.defense, 0, 9999);
        target.health -= damage;
        return damage;
    }


    public int DoSubMenuAction(Fighter attacker, Fighter target, SubMenuItem subMenuItem)
    {
        switch (subMenuItem.subMenuType)
        {
            case SubMenu.Magic:
                return DoMagic(attacker, target, (Magic)subMenuItem);
            case SubMenu.Skills:
                break;
            case SubMenu.Bestia:
                return DoBestia(attacker, target, (Bestia)subMenuItem);
        }

        Debug.LogErrorFormat("SubMenuType of asset {0} is not valid.", subMenuItem.subMenuItemName);
        return 0;
    }

    private int DoMagic(Fighter attacker, Fighter target, Magic magic)
    {
        switch (magic.magicType)
        {
            case MagicType.Debuff:
                return DoMagicDamage(attacker, target, magic);
            case MagicType.Buff:
                return DoHeal(attacker, target, magic);
        }

        Debug.LogErrorFormat("MagicType of {0} not valid.", magic.subMenuItemName);
        return 0;
    }

    private int DoMagicDamage(Fighter attacker, Fighter target, Magic magic)
    {
        int damage = Mathf.Clamp(magic.damage - target.defense, 0, 9999);
        target.health -= damage;
        return damage;
    }

    private int DoHeal(Fighter attacker, Fighter target, Magic magic)
    {
        int heal = magic.damage;
        target.health += heal;
        return heal;
    }

    private int DoBestia(Fighter attacker, Fighter target, Bestia bestia)
    {
        int damage = bestia.damage;
        target.health -= bestia.damage;
        return damage;
    }
}

