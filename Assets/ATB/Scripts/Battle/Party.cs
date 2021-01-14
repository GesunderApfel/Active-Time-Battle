using System.Collections.Generic;
using UnityEngine;

[SerializeField, CreateAssetMenu(fileName = "Fight Party", menuName = "Final Fantasy Battle/Add Party")]
public class Party : ScriptableObject
{
    [SerializeField] private List<FighterBlueprint> fighterBlueprints;
    private List<Fighter> _fighters;
    public List<Fighter> Fighters { get { return _fighters; } private set { _fighters = value; } }
    [Tooltip("Deactivates at the start of a fight")]
    public bool isPartyMemberActive;

    public void Init()
    {
        isPartyMemberActive = false;
        Fighters = new List<Fighter>();
        foreach (FighterBlueprint bp in fighterBlueprints)
        {
            Fighters.Add(new Fighter(bp));
        }
    }


}
