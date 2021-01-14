using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A script for navigating UI grids
/// </summary>
[RequireComponent(typeof(GridLayoutGroup))]
public class GridMenuNavigation : MonoBehaviour
{
    private RectTransform rectTransform;
    public GridLayoutGroup Grid { get; private set; }
    public int index;
    /// <summary>
    /// Should be obsolete if an actual scrollview is used
    /// </summary>
    [Tooltip("Will be obsolete if an actual ScrollView is used"), Range(1, 4)] public int maxRows = 1;

    private int childCount;

    /// <summary>
    /// A dictionary for methods which will be invoked when confirmed
    /// </summary>
    public Dictionary<int, Action> actionDict = new Dictionary<int, Action>();

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Grid = GetComponent<GridLayoutGroup>();
        childCount = transform.childCount;
    }

    /// <summary>
    /// Navigates through grid and returns the position left to the selected cell
    /// This allows direct positioning of a cursor.
    /// </summary>
    /// <returns></returns>
    public Vector3 Navigate()
    {
        childCount = transform.childCount;

        int oldIndex = index;

        if (Input.GetKeyDown(KeyCode.W))
        {
            index--;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            index++;
        }

        if (Input.GetKeyDown(KeyCode.A))
            index -= maxRows;
        else if (Input.GetKeyDown(KeyCode.D))
            index += maxRows;

        if (index < 0)
            index += childCount;
        else if (index >= childCount)
            index -= childCount;

        if (oldIndex != index)
            BattleSoundManager.Instance.PlayNavigate();

        RectTransform selection = transform.GetChild(index).GetComponent<RectTransform>();
        // 0.3f is magic number
        return selection.position - Vector3.right * Grid.cellSize.x * 0.3f;
    }


    /// <summary>
    /// returns position of the selected UI grid child element.
    /// </summary>
    public Vector3 SelectedChildPosition()
    {
        RectTransform selection = transform.GetChild(index).GetComponent<RectTransform>();
        return selection.position;
    }

    public void Confirm()
    {
        BattleSoundManager.Instance.PlayConfirm();
        actionDict[index].Invoke();
    }
}
