using UnityEngine;

/// <summary>
/// Script for the parent object of a magic spells particles
/// Holds an array of timestamps, which the target gets hit (for dmg display and hit animation)
/// </summary>
public class MagicTimer : MonoBehaviour
{
    public float[] durationUntilHit;
    public float endTime;
}
