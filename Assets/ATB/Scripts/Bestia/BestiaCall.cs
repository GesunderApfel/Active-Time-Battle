using UnityEngine;

/// <summary>
/// The start time defines the time after which the bestia animation starts, NOT the summoning
/// e.g. before the titan starts its animation (beginning with rising from the ground),
/// it has to wait a specific time for the particles (purple rings) to appear.
/// </summary>
public class BestiaCall : MonoBehaviour
{
    protected Animator _animator;
    public Transform bestiaTransform;
    public float startTime;
}

