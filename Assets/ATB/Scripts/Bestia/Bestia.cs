using UnityEngine;

[SerializeField, CreateAssetMenu(fileName = "Bestia", menuName = "Final Fantasy Battle/Add Bestia")]
public class Bestia : SubMenuItem
{
    public int damage;
    public int manaCost;
    public float duration;
    public GameObject bestiaCall;
    public string infoText;

    public Bestia()
    {
        subMenuType = SubMenu.Bestia;
    }
}

