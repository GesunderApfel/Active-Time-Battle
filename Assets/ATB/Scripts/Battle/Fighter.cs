using System;
using System.Collections.Generic;
using UnityEngine;

public class Fighter
{
    public Guid Guid { get; private set; }

    public string name;
    public int level;
    public int maxHealth;
    public int maxMana;
    public int health;
    public int mana;
    public float attackTurnMeter;
    public int speed;
    public int attack;
    public int defense;
    public float critChance;

    public GameObject fighterPrefab;
    public Vector3 fighterPrefabScale;
    public bool isDoingAction;

    public List<Magic> spells;
    public List<Bestia> bestias;

    public Fighter(FighterBlueprint bp)
    {
        Guid = Guid.NewGuid();
        level = 1;

        name = bp.name;
        maxHealth = bp.maxHealth;
        maxMana = bp.maxMana;
        health = bp.maxHealth;
        mana = bp.maxMana;
        attackTurnMeter = bp.attackTurnMeter;
        speed = bp.speed;
        attack = bp.attack;
        defense = bp.defense;
        critChance = bp.critChance;

        fighterPrefab = bp.fighterPrefab;
        fighterPrefabScale = bp.fighterPrefabScale;

        spells = bp.spells;
        bestias = bp.bestias;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        return Guid == ((Fighter)obj).Guid;
    }

    public override int GetHashCode()
    {
        return -737073652 + EqualityComparer<Guid>.Default.GetHashCode(Guid);
    }

    public static bool operator ==(Fighter a, Fighter b)
    {
        if (object.Equals(a, null))
            if (object.Equals(b, null))
                return true;
            else
                return false;

        return a.Equals(b);
    }

    public static bool operator !=(Fighter a, Fighter b)
    {
        if (object.Equals(a, null))
            if (object.Equals(b, null))
                return false;
            else
                return true;
        return !a.Equals(b);
    }
}

