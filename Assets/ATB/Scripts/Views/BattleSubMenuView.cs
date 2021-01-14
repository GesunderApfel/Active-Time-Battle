using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Holds dynamic children UI elements which depend on the active fighter and the choice of the player
/// e.g. the player chooses "Magic" and the fighter has two spells => Two children (spell names)
/// </summary>
public class BattleSubMenuView : MonoBehaviour
{
    private RectTransform _subMenuRectTransform;
    [SerializeField] private GameObject gridTextPrefab;
    public GridMenuNavigation gridNavigationMenu;

    private void Awake()
    {
        _subMenuRectTransform = GetComponent<RectTransform>();
    }

    public void UpdateView(Fighter activeFighter)
    {
        if (activeFighter == null)
        {
            Hide();
            return;
        }

        Show();
    }

    public void SetSubMenuPosition(Vector3 position, List<SubMenuItem> items)
    {

        // 0.25 is magic number
        var newPos = position + Vector3.right * _subMenuRectTransform.rect.width * 0.25f;
        _subMenuRectTransform.position = newPos;

        foreach (SubMenuItem item in items)
        {
            TMP_Text textMesh = Instantiate(gridTextPrefab, _subMenuRectTransform).GetComponent<TMP_Text>();
            textMesh.text = item.subMenuItemName;
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Destroys all children of the submenu
    /// </summary>
    public void CleanUp()
    {
        var children = transform.GetComponentInChildren<Transform>();
        foreach (Transform child in children)
        {
            Destroy(child.gameObject);
        }
    }
}

