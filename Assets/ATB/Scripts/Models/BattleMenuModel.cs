using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleMenuModel
{
    [SerializeField] private BattleMenuView _view;
    public GridMenuNavigation rootMenuNavigation;

    public string BattleFieldInfoText { get; private set; }
    private float infoTextWaitTime = 2f;
    private float infoTextWaitStartTime;

    #region Controller Events
    public event EventHandler onAttackMenu;
    public event EventHandler onMagicMenu;
    public event EventHandler onSkillMenu;
    public event EventHandler onBestiaMenu;
    public event EventHandler onInventoryMenu;
    public event EventHandler onFlightMenu;
    #endregion

    public BattleMenuModel(List<PartyMemberView> partyMemberViews, GridMenuNavigation navigation)
    {
        rootMenuNavigation = navigation;

        rootMenuNavigation.actionDict.Add((int)SubMenu.Attack, AttackRootMenu);
        rootMenuNavigation.actionDict.Add((int)SubMenu.Magic, MagicRootMenu);
        rootMenuNavigation.actionDict.Add((int)SubMenu.Skills, SkillRootMenu);
        rootMenuNavigation.actionDict.Add((int)SubMenu.Bestia, BestiaRootMenu);
        rootMenuNavigation.actionDict.Add((int)SubMenu.Inventory, InventoryRootMenu);
        rootMenuNavigation.actionDict.Add((int)SubMenu.Flight, FlightRootMenu);
    }

    /// <summary>
    /// navigates through menu and return cursor position for the view
    /// </summary>
    public Vector3 Navigate()
    {
        // 4f is magic number
        Vector3 cursorPos = rootMenuNavigation.Navigate() + Vector3.right * 4f;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rootMenuNavigation.Confirm();
        }
        SetBattleFieldInfoText();
        return cursorPos;
    }

    private void SetBattleFieldInfoText()
    {
        if (rootMenuNavigation.index == 0)
            BattleFieldInfoText = "Attack a target.";
        else if (rootMenuNavigation.index == 1)
            BattleFieldInfoText = "Choose a spell.";
        else if (rootMenuNavigation.index == 2)
            BattleFieldInfoText = "Choose a skill.";
        else if (rootMenuNavigation.index == 3)
            BattleFieldInfoText = "Summon a Bestia.";
        else if (rootMenuNavigation.index == 4)
            BattleFieldInfoText = "Choose an item from the inventory.";
        else if (rootMenuNavigation.index == 5)
            BattleFieldInfoText = "Flee from the battle";
    }

    /// <summary>
    /// Some InfoText should stay (until the player gets to the sub menu at least)
    /// e.g. If a player isn't allowed to flee, he should get some time to read the info.
    /// </summary>
    public void WaitWithInfoTextChange()
    {
        infoTextWaitStartTime = Time.time;
    }

    public bool IsInfoTextBlocked()
    {
        // see comment above WaitWithInfoTextChange()
        if (Time.time < infoTextWaitStartTime + infoTextWaitTime)
            return true;
        return false;
    }


    private void AttackRootMenu()
    {
        onAttackMenu?.Invoke(this, EventArgs.Empty);
    }

    private void MagicRootMenu()
    {
        onMagicMenu?.Invoke(this, EventArgs.Empty);
    }

    private void SkillRootMenu()
    {
        onSkillMenu?.Invoke(this, EventArgs.Empty);
    }

    private void BestiaRootMenu()
    {
        onBestiaMenu?.Invoke(this, EventArgs.Empty);
    }

    private void InventoryRootMenu()
    {
        onInventoryMenu?.Invoke(this, EventArgs.Empty);
    }

    private void FlightRootMenu()
    {
        onFlightMenu?.Invoke(this, EventArgs.Empty);
    }

}