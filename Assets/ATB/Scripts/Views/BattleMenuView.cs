using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Displays the main UI elements: root menu, info panel, party member panel, all cursors
/// </summary>
public class BattleMenuView : MonoBehaviour
{

    [SerializeField] private RectTransform worldSpaceGuiTransform;

    [SerializeField] GameObject gridTextPrefab;
    [SerializeField] private GameObject worldSpaceTextPrefab;
    public GridMenuNavigation gridNavigationMenu;

    [SerializeField] private TextMeshProUGUI battleInfo;

    [SerializeField] private GameObject rootMenu;
    [SerializeField] List<PartyMemberView> partyMemberViews;
    private List<GameObject> activeBlinks;

    #region Cursor Members
    [SerializeField] private GameObject menuCursor;
    private MenuCursorMover _menuCursorMover;
    private Vector3 _menuCursorOffset;

    [SerializeField] private GameObject battleFieldCursor;
    private MenuCursorMover _battleFieldCursorMover;
    private Vector3 _battleFieldCursorOffset;
    #endregion


    private void Awake()
    {
        _menuCursorMover = menuCursor.GetComponent<MenuCursorMover>();
        _battleFieldCursorMover = battleFieldCursor.GetComponent<MenuCursorMover>();

        _menuCursorOffset = -Vector3.right * menuCursor.GetComponent<RectTransform>().rect.width * 0.5f;
        _battleFieldCursorOffset = -Vector3.right;

        activeBlinks = new List<GameObject>();
    }

    public void UpdateView(BattleActionProcessor battleActionProcessor)
    {
        if (battleActionProcessor.ActiveFighter == null)
        {
            Hide();
            SetPartyMemberBlink(battleActionProcessor.LastActiveFighterIndex, false);
            return;
        }

        if (battleActionProcessor.ActiveFighter != battleActionProcessor.LastActiveFighter && battleActionProcessor.LastActiveFighter != null)
        {
            SetPartyMemberBlink(battleActionProcessor.LastActiveFighterIndex, false);
        }


        Show();
        SetPartyMemberBlink(battleActionProcessor.ActiveFighterIndex, true);
        foreach (var view in partyMemberViews)
        {
            Fighter fighter = battleActionProcessor.battleParties.viewFighterDict[view];
            view.health.text = fighter.health + "/" + fighter.maxHealth;
            view.mana.text = fighter.mana + "/" + fighter.maxMana;
        }
    }

    public void Show()
    {
        rootMenu.SetActive(true);
        menuCursor.SetActive(true);
    }

    public void Hide()
    {
        rootMenu.SetActive(false);
        menuCursor.SetActive(false);
    }

    public List<PartyMemberView> GetPartyMemberViews()
    {
        return partyMemberViews;
    }

    public void SetBattleInfoText(string text)
    {
        battleInfo.text = text;
    }


    public void InitPartyMemberView(BattleActionProcessor battleActionProcessor)
    {

        foreach (var view in partyMemberViews)
        {
            Fighter fighter = battleActionProcessor.battleParties.viewFighterDict[view];
            view.fighterName.text = fighter.name;
            view.health.text = fighter.health + "/" + fighter.maxHealth;
            view.mana.text = fighter.mana + "/" + fighter.maxMana;
            activeBlinks.Add(view.ActivePanel);
            view.Show();
        }
    }

    public void SetPartyMemberBlink(int index, bool active)
    {
        if (index >= 0)
            activeBlinks[index].SetActive(active);
    }

    public void SpawnDamageText(Vector3 position, float damage)
    {
        GameObject text = Instantiate(worldSpaceTextPrefab, Camera.main.WorldToScreenPoint(position + Vector3.up), Quaternion.identity, worldSpaceGuiTransform);
        text.GetComponent<TextMeshProUGUI>().text = damage.ToString();
        Destroy(text, 1f);
    }

    #region Cursors
    public void SetMenuCursorActive(bool active)
    {
        menuCursor.SetActive(active);
    }

    public void ToogleMenuCursor()
    {
        if (!menuCursor.activeSelf)
            menuCursor.SetActive(true);
    }

    public void SetMenuCursorPosition(Vector3 canvasPoint)
    {
        _menuCursorMover.SetCursorToNewPosition(canvasPoint);
    }

    public void SetBattleFieldCursorActive(bool active)
    {
        battleFieldCursor.SetActive(active);
    }

    public void ToggleBattleFieldCursor()
    {
        if (!battleFieldCursor.activeSelf)
            battleFieldCursor.SetActive(true);
    }

    public void SetBattleFieldCursorPosition(Vector3 worldPoint)
    {
        _battleFieldCursorMover.SetCursorToNewPosition(Camera.main.WorldToScreenPoint(worldPoint + _battleFieldCursorOffset));
    }
    #endregion

}

