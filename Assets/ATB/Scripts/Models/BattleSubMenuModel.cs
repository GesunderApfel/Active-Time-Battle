using System;
using UnityEngine;

public class BattleSubMenuModel
{
    private GridMenuNavigation _subMenuNavigation;
    private SubMenu _subMenu;
    private BattleActionProcessor _battleActionProcessor;

    private Vector3 _menuCursorPosition;
    private Vector3 _battleFieldCursorOffscreenPosition = new Vector3(0, -200, 0);

    #region Controller Event Members
    // region contains all members relevant for the Controller / UI

    public string BattleInfoText { get; private set; }
    public Vector3 TargetPosition { get; private set; }
    #endregion

    #region Controller Events
    // region contains events relevant for the Controller / UI and the invoking methods

    public event EventHandler onBackToRoot;
    public event EventHandler onTargetSelectionCancel;
    public event EventHandler onTargetSelection;
    public event EventHandler onEndTurn;

    private void BackToRoot()
    {
        onBackToRoot?.Invoke(this, EventArgs.Empty);
    }

    private void CancelTargetSelection()
    {
        onTargetSelectionCancel?.Invoke(this, EventArgs.Empty);
    }

    private void TargetSelection()
    {
        onTargetSelection?.Invoke(this, EventArgs.Empty);
    }

    private void EndTurn()
    {
        CancelTargetSelection();
        _battleActionProcessor.EndActiveFighterTurn();
        onEndTurn?.Invoke(this, EventArgs.Empty);
        BackToRoot();

        BattleSoundManager.Instance.PlayConfirm();
    }
    #endregion

    #region BattleField
    private int _targetSelectionIndex;
    private bool _isSelectingTarget;
    #endregion


    public BattleSubMenuModel(BattleActionProcessor battleActionModel, GridMenuNavigation navigation)
    {
        _subMenuNavigation = navigation;
        _battleActionProcessor = battleActionModel;
        SetSubMenu(SubMenu.Attack);
        _targetSelectionIndex = GetFirstEnemyFighterIndex();
    }

    public void SetSubMenu(SubMenu subMenu)
    {
        _subMenu = subMenu;
    }

    /// <summary>
    /// Returns the MenuCursorPosition and navigates.
    /// The BattleMenuController obtains the BattleFieldCursor position by event and associated member variable.
    /// event => onTargetSelection
    /// variable => TargetPosition
    /// </summary>
    public Vector3 Navigate()
    {
        switch (_subMenu)
        {
            case SubMenu.Attack:
                TargetSelectionSubMenu();
                return _battleFieldCursorOffscreenPosition;
            case SubMenu.Magic:
                NavigateSubMenuLoop();
                break;
            case SubMenu.Skills:
                BackToRoot();
                break;
            case SubMenu.Bestia:
                NavigateSubMenuLoop();
                break;
            case SubMenu.Inventory:
                BackToRoot();
                break;
            case SubMenu.Flight:
                BackToRoot();
                break;
        }

        return _menuCursorPosition;
    }

    /// <summary>
    /// Nagivates through the fighters of the battlefield and returns the cursor position
    /// </summary>
    private void TargetSelectionSubMenu()
    {
        Fighter enemy = NavigateTargets();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _battleActionProcessor.AddAttack(_battleActionProcessor.ActiveFighter, enemy);
            EndTurn();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            BattleSoundManager.Instance.PlayCancel();
            CancelTargetSelection();
            BackToRoot();
        }
    }

    /// <summary>
    /// Navigates through different options of the submenu and returns cursor position for the view
    /// </summary>
    private void NavigateSubMenuLoop()
    {
        if (!_isSelectingTarget)
        {
            // navigates through the sub menu options until the player return to root menu or confirms an action
            _menuCursorPosition = _subMenuNavigation.Navigate();
            SetBattleInfoTextBySubMenuElement();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SetBattleFieldCursorDependingOnMagicType();
                _isSelectingTarget = true;
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                BackToRoot();
            }

        }
        else
        {
            // after the player confirms an action, he has to choose which target he attacks.
            Fighter target = NavigateTargets();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // after confirming the target, the action will be added to the battleActionProcessor
                // and the turn of the active player ends
                _isSelectingTarget = false;
                var attacker = _battleActionProcessor.ActiveFighter;
                switch (_subMenu)
                {
                    case SubMenu.Magic:
                        var spell = attacker.spells[_subMenuNavigation.index];
                        _battleActionProcessor.AddMagic(attacker, target, spell);
                        break;
                    case SubMenu.Skills:
                        BackToRoot();
                        break;
                    case SubMenu.Bestia:
                        var bestia = attacker.bestias[_subMenuNavigation.index];
                        _battleActionProcessor.AddBestia(attacker, target, bestia);
                        break;
                    case SubMenu.Inventory:
                        BackToRoot();
                        break;
                    case SubMenu.Flight:
                        BackToRoot();
                        break;
                }

                EndTurn();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                BattleSoundManager.Instance.PlayCancel();
                CancelTargetSelection();
                _isSelectingTarget = false;
            }
        }
    }

    private void SetBattleInfoTextBySubMenuElement()
    {
        switch (_subMenu)
        {
            case SubMenu.Attack:
                break;
            case SubMenu.Magic:
                BattleInfoText = _battleActionProcessor.ActiveFighter.spells[_subMenuNavigation.index].infoText;
                break;
            case SubMenu.Skills:
                break;
            case SubMenu.Bestia:
                BattleInfoText = _battleActionProcessor.ActiveFighter.bestias[_subMenuNavigation.index].infoText;
                break;
            case SubMenu.Inventory:
                break;
            case SubMenu.Flight:
                break;
        }
    }

    private Fighter NavigateTargets()
    {
        var fighters = _battleActionProcessor.battleParties.allFighters;
        bool playerDidInput = false;

        int oldIndex = _targetSelectionIndex;

        if (Input.GetKeyDown(KeyCode.W))
        {
            _targetSelectionIndex--;
            playerDidInput = true;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            _targetSelectionIndex++;
            playerDidInput = true;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            _targetSelectionIndex--;
            playerDidInput = true;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            _targetSelectionIndex++;
            playerDidInput = true;
        }

        if (_targetSelectionIndex < 0)
        {
            _targetSelectionIndex = fighters.Count - 1;
        }
        else if (_targetSelectionIndex >= fighters.Count)
        {
            // what is playerDidInput for?
            // maybe during the selection, the current target dies, in that case, the next appropriate target should be selected
            // if we dont catch that case, the selection could change "teams"
            // Note: Currently just works for enemies
            if (playerDidInput)
                _targetSelectionIndex = 0;
            else
                _targetSelectionIndex -= 1;
        }

        if (oldIndex != _targetSelectionIndex)
            BattleSoundManager.Instance.PlayNavigate();

        Fighter target = fighters[_targetSelectionIndex];
        BattleInfoText = string.Format("{0}: {1}/{2}", target.name, target.health, target.maxHealth);
        TargetPosition = _battleActionProcessor.battleParties.fighterTransformDict[target].position;
        TargetSelection();

        return target;
    }

    /// <summary>
    /// If magic is Buff => select player team member, Debuff => select enemy
    /// </summary>
    private void SetBattleFieldCursorDependingOnMagicType()
    {
        switch (_subMenu)
        {
            case SubMenu.Magic:
                if (_battleActionProcessor.ActiveFighter.spells[_subMenuNavigation.index].magicType == MagicType.Debuff)
                    _targetSelectionIndex = GetFirstEnemyFighterIndex();
                else
                    _targetSelectionIndex = GetActivePlayerFighterIndex();
                break;
            case SubMenu.Bestia:
                if (_battleActionProcessor.ActiveFighter.bestias[_subMenuNavigation.index].magicType == MagicType.Debuff)
                    _targetSelectionIndex = GetFirstEnemyFighterIndex();
                else
                    _targetSelectionIndex = GetActivePlayerFighterIndex();
                break;
        }
    }

    private int GetActivePlayerFighterIndex()
    {
        return _battleActionProcessor.ActiveFighterIndex;
    }

    private int GetFirstEnemyFighterIndex()
    {
        return _battleActionProcessor.battleParties.PlayerParty.Fighters.Count;
    }
}
