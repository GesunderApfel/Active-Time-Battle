using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains all parties (and therefore fighters), their Transforms and UI Elements.
/// Furthermore allows manipulation or access to party information through public methods.
/// </summary>
public class BattleParties
{
    public Party PlayerParty { get; private set; }
    public Party EnemyParty { get; private set; }

    /// <summary>
    /// Holds all fighters for easy navigation purposes. Beginning with the Player Party.
    /// e.g. maybe the player wants to spell a heal on the enemy or damage a party member of his own.
    /// </summary>
    public List<Fighter> allFighters;

    public Dictionary<PartyMemberView, Fighter> viewFighterDict;
    public Dictionary<Fighter, Transform> fighterTransformDict;
    public Dictionary<Fighter, TurnMeterView> fighterTurnMeterDict;

    public BattleParties(BattleField battleField, List<PartyMemberView> partyMemberViews)
    {
        PlayerParty = battleField.PlayerParty;
        EnemyParty = battleField.EnemyParty;
        PlayerParty.Init();
        EnemyParty.Init();

        allFighters = new List<Fighter>();
        viewFighterDict = new Dictionary<PartyMemberView, Fighter>();
        fighterTransformDict = new Dictionary<Fighter, Transform>();
        fighterTurnMeterDict = new Dictionary<Fighter, TurnMeterView>();

        InitAllFighters();
        InstantiatePlayerParty(partyMemberViews, battleField.PlayerPartyTransform);
        InstantiateEnemyParty(battleField.EnemyPartyTransform);
    }

    private void InitAllFighters()
    {
        allFighters.AddRange(PlayerParty.Fighters);
        allFighters.AddRange(EnemyParty.Fighters);
    }

    private void InstantiatePlayerParty(List<PartyMemberView> partyMemberViews, Transform playerPartyTransform)
    {
        for (int i = 0; i < PlayerParty.Fighters.Count; i++)
        {
            var fighter = PlayerParty.Fighters[i];
            var view = partyMemberViews[i];
            var fighterTransform = InstantiateFighterPrefab(fighter, playerPartyTransform, i, true);

            fighterTransformDict.Add(fighter, fighterTransform);
            viewFighterDict.Add(view, fighter);
            fighterTurnMeterDict.Add(fighter, view.turnMeterView);
        }
    }

    private void InstantiateEnemyParty(Transform enemyPartyTransform)
    {
        for (int i = 0; i < EnemyParty.Fighters.Count; i++)
        {
            Fighter fighter = EnemyParty.Fighters[i];
            Transform fighterTransform = InstantiateFighterPrefab(fighter, enemyPartyTransform, i, false);
            fighterTransformDict.Add(fighter, fighterTransform);
        }
    }

    private Transform InstantiateFighterPrefab(Fighter fighter, Transform partyTransform, int index, bool isPlayerParty)
    {
        Transform currentModel = partyTransform.GetChild(index);
        if (fighter.fighterPrefab == null)
        {
            Debug.LogWarningFormat("Fighter {0} of {1} has no Prefab attached to it",
                fighter.name,
                isPlayerParty ? "Player Party" : "EnemyParty");
            return currentModel;
        }


        Transform fighterPrefab = Object.Instantiate(fighter.fighterPrefab.transform, partyTransform);
        fighterPrefab.localScale = fighter.fighterPrefabScale;
        fighterPrefab.position = currentModel.position - Vector3.up;

        // assumes the model is facing the Z-direction
        if (isPlayerParty)
            fighterPrefab.Rotate(0f, 90f, 0f);
        else
            fighterPrefab.Rotate(0f, -90f, 0f);

        fighterPrefab.name = fighter.name;
        Object.Destroy(currentModel.gameObject);

        return fighterPrefab;
    }

    public void KillFighter(Fighter fighter)
    {
        GameObject.Destroy(fighterTransformDict[fighter].gameObject);
        fighterTransformDict.Remove(fighter);
        allFighters.Remove(fighter);
    }

    /// <summary>
    /// Fighters have a particular sequence, this helps getting the index of a fighter to access the correct fighter from "_allFighters"
    /// </summary>
    public int GetIndexOfFighterInPlayerParty(Fighter fighter)
    {
        return PlayerParty.Fighters.IndexOf(fighter);
    }

    /// <summary>
    /// Fighters have a particular sequence, this helps getting the index of a fighter to access the correct fighter from "_allFighters"
    /// </summary>
    public int GetIndexOfFighterInEnemyParty(Fighter fighter)
    {
        return EnemyParty.Fighters.IndexOf(fighter) + PlayerParty.Fighters.Count;
    }

    public bool IsFighterInPlayerParty(Fighter fighter)
    {
        if (PlayerParty.Fighters.Contains(fighter))
            return true;
        return false;
    }

}

