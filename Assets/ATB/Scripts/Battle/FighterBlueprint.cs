using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The name "Blueprint" is just my convention for ScriptableObjects, which are used
/// for other classes (in this case "Fighter"). It serves as a simple DTO class for instantiation.
/// That way I don't have to worry changing the data of scriptable objects during runtime.
/// </summary>
[SerializeField, CreateAssetMenu(fileName = "Fighter", menuName = "Final Fantasy Battle/Add Fighter")]
public class FighterBlueprint : ScriptableObject
{
    public int maxHealth;
    public int maxMana;
    public int attackTurnMeter;
    public int speed;
    public int attack;
    public int defense;
    [Range(0, 1)] public float critChance;

    public GameObject fighterPrefab;
    public Vector3 fighterPrefabScale = Vector3.one;

    public List<Magic> spells;
    public List<Bestia> bestias;
}

