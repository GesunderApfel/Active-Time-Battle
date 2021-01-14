using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Architecture is following the MVC pattern, but just one controller (this) for all the menus.
/// This can change with scale and complexity growth.
/// 
/// The controller starts with Start() and not Awake(), so all the Views and other GameObjects can initiate themselves
/// before the controller wires / sets them up. It's a simple way to prevent dependency errors.
/// 
/// The communication between model and view is mainly integrated by events (see region "Model Events").
/// 
/// UI positioning wasn't the focus of this prototype, therefore many calculations (like cursor positions) invole magic numbers.
/// Magic numbers are "labeled" with comments.
/// 
/// For now, no enemy logic is implemented and the battle consists just of menu navigation and the player team actions.
/// 
/// Future milestones: 
/// - Camera movement and "cinematic effects" (like the camera concentrating on the action should add some juice to the "game")
/// - Enemy actions
/// </summary>
public class BattleMenuController : MonoBehaviour
{
    //public bool isDebugging;

    [SerializeField] private BattleMenuView _battleMenuView;
    [SerializeField] private BattleSubMenuView _battleSubMenuView;
    [SerializeField] private GameObject _battleOverCanvas;
    [SerializeField] private BattleField battleField;

    private BattleMenuModel _battleMenuModel;
    private BattleSubMenuModel _battleSubMenuModel;
    private BattleActionProcessor _battleActionProcessor;

    private BattleState _battleState;
    private MenuLevel _menuLevel;


    void Start()
    {
        _menuLevel = MenuLevel.Root;
        _battleState = BattleState.OnGoing;
        var battleParties = new BattleParties(battleField, _battleMenuView.GetPartyMemberViews());

        _battleMenuModel = new BattleMenuModel(_battleMenuView.GetPartyMemberViews(), _battleMenuView.gridNavigationMenu);
        _battleActionProcessor = new BattleActionProcessor(battleParties);
        _battleSubMenuModel = new BattleSubMenuModel(_battleActionProcessor, _battleSubMenuView.gridNavigationMenu);

        _battleMenuView.InitPartyMemberView(_battleActionProcessor);
        _battleActionProcessor.StartCoroutines(this);



        SetRootMenuEvents();
        SetSubMenuEvents();
        SetBattleActionProcessorEvents();
    }

    void Update()
    {
        if (_battleState == BattleState.Over)
            return;

        _battleMenuView.UpdateView(_battleActionProcessor);

        if (_battleActionProcessor.ActiveFighter == null)
            return;


        Vector3 cursorPos;
        switch (_menuLevel)
        {

            case MenuLevel.Root:
                cursorPos = _battleMenuModel.Navigate();
                _battleMenuView.SetMenuCursorPosition(cursorPos);
                if (!_battleMenuModel.IsInfoTextBlocked())
                    _battleMenuView.SetBattleInfoText(_battleMenuModel.BattleFieldInfoText);
                break;
            case MenuLevel.Sub:
                cursorPos = _battleSubMenuModel.Navigate();
                _battleMenuView.SetMenuCursorPosition(cursorPos);
                _battleMenuView.SetBattleInfoText(_battleSubMenuModel.BattleInfoText);
                break;
            case MenuLevel.Sub2:
                break;
        }
    }


    #region Model Events

    private void SetRootMenuEvents()
    {
        // onAttackMenu etc. is invoked when the player confirms a selection in the root menu
        _battleMenuModel.onAttackMenu += (sender, args) => Attack();
        _battleMenuModel.onMagicMenu += (sender, args) => Magic();
        _battleMenuModel.onBestiaMenu += (sender, args) => Bestia();
        _battleMenuModel.onSkillMenu += (sender, args) => Skills();
        _battleMenuModel.onInventoryMenu += (sender, args) => Inventory();
        _battleMenuModel.onFlightMenu += (sender, args) => Flight();
    }

    #region Root Menu
    private void Attack()
    {
        Debug.Log("Attack Sub Menu");
        _menuLevel = MenuLevel.Sub;
        _battleSubMenuModel.SetSubMenu(SubMenu.Attack);
    }

    private void Magic()
    {
        var activeFighter = _battleActionProcessor.ActiveFighter;
        if (activeFighter != null && activeFighter.spells.Count > 0)
        {
            _menuLevel = MenuLevel.Sub;
            _battleSubMenuModel.SetSubMenu(SubMenu.Magic);
            SetSubPanel(activeFighter.spells.ToList<SubMenuItem>());
            Debug.Log("Magic Sub Menu");
        }
        else
        {
            Debug.Log("Fighter has no spells");
        }
    }

    private void Bestia()
    {
        var activeFighter = _battleActionProcessor.ActiveFighter;
        if (activeFighter != null && activeFighter.bestias.Count > 0)
        {
            _menuLevel = MenuLevel.Sub;
            _battleSubMenuModel.SetSubMenu(SubMenu.Bestia);
            SetSubPanel(activeFighter.bestias.ToList<SubMenuItem>());
            Debug.Log("Bestia Sub Menu");
        }
        else
        {
            Debug.Log("Fighter has no bestias");
        }
    }

    private void Skills()
    {
        Debug.Log("Skills aren't implemented.");
    }

    private void Inventory()
    {
        Debug.Log("Inventory is not implemented");
    }

    private void Flight()
    {
        Debug.Log("Trying to flee... look at the Info Panel at the top.");
        _battleMenuModel.WaitWithInfoTextChange();
        _battleMenuView.SetBattleInfoText("Can't flee this battle.");
    }
    #endregion

    private void SetSubMenuEvents()
    {
        _battleSubMenuModel.onBackToRoot += (sender, args) => BackToRoot();
        _battleSubMenuModel.onTargetSelectionCancel += (sender, args) => TargetSelectionCancel();
        _battleSubMenuModel.onTargetSelection += (sender, args) => TargetSelection();
        _battleSubMenuModel.onEndTurn += (sender, args) => EndTurn();
    }

    #region SubMenu
    private void BackToRoot()
    {
        Debug.Log("Back to Root Level");
        _battleSubMenuView.CleanUp();
        _battleSubMenuView.Hide();
        _menuLevel = MenuLevel.Root;
    }

    private void TargetSelectionCancel()
    {
        Debug.Log("Cancel Target Selection");
        _battleMenuView.SetBattleFieldCursorActive(false);
    }

    private void TargetSelection()
    {
        _battleMenuView.SetBattleFieldCursorActive(true);
        _battleMenuView.SetBattleFieldCursorPosition(_battleSubMenuModel.TargetPosition);
    }

    private void EndTurn()
    {
        Debug.Log("End Turn");
    }
    #endregion

    private void SetBattleActionProcessorEvents()
    {
        _battleActionProcessor.onDamageDealt += (sender, args) => DamageDealt();
        _battleActionProcessor.onBattleWon += (sender, args) => BattleWon();
        _battleActionProcessor.onBattleLost += (sender, args) => BattleLost();
    }

    #region BattleActionProcessor
    private void DamageDealt()
    {
        Debug.LogFormat("Damage Dealt {0}", _battleActionProcessor.LastDamageDealt);
        _battleMenuView.SpawnDamageText(_battleActionProcessor.LastTargetTransform.position, _battleActionProcessor.LastDamageDealt);
    }

    private void BattleWon()
    {
        Debug.Log("You won the battle.");
        BattleSoundManager.Instance.PlayWinFanfare();
        StopBattle();
    }

    private void BattleLost()
    {
        Debug.Log("You lost the battle.");
        BattleSoundManager.Instance.PlayLoseFanfare();
        StopBattle();
    }

    private void StopBattle()
    {
        _battleState = BattleState.Over;
        Destroy(_battleMenuView.gameObject);
        Destroy(_battleSubMenuView.gameObject);
        _battleActionProcessor = null;
        _battleMenuModel = null;
        _battleSubMenuModel = null;
        _battleOverCanvas.SetActive(true);
    }
    #endregion

    #endregion

    private void SetSubPanel(List<SubMenuItem> items)
    {
        _battleSubMenuView.Show();
        var newPos = _battleMenuModel.rootMenuNavigation.SelectedChildPosition()
            + Vector3.right * _battleMenuModel.rootMenuNavigation.Grid.cellSize.x * 0.1f;
        // 0.1f is magic number


        _battleSubMenuView.SetSubMenuPosition(newPos, items);
    }
}

public enum MenuLevel
{
    Root,
    Sub,
    Sub2,
}

public enum SubMenu
{
    Attack = 0,
    Magic = 1,
    Skills = 2,
    Bestia = 3,
    Inventory = 4,
    Flight = 5,
}

public enum BattleState
{
    OnGoing,
    Over
}