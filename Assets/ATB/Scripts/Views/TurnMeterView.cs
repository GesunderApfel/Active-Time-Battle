using UnityEngine;

public class TurnMeterView : MonoBehaviour
{
    [SerializeField] private RectTransform turnMeterPercentageBar;

    private void Awake()
    {
        turnMeterPercentageBar.localScale = new Vector3(0f, 1f, 1f);
    }

    public void UpdateTurnMeter(float percentage)
    {
        turnMeterPercentageBar.localScale = new Vector3(percentage * 0.01f, 1f, 1f);
    }
}

