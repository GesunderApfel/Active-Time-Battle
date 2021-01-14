using TMPro;
using UnityEngine;

public class PartyMemberView : MonoBehaviour
{
    public TMP_Text fighterName;
    public TMP_Text health;
    public TMP_Text mana;
    public TurnMeterView turnMeterView;
    public GameObject ActivePanel;


    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

