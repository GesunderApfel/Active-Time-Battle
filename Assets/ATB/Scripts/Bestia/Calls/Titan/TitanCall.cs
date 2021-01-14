using DG.Tweening;
using System.Collections;
using UnityEngine;

/// <summary>
/// Titan Call animation sequence
/// </summary>
public class TitanCall : BestiaCall
{
    // Start is called before the first frame update
    void Start()
    {
        _animator = bestiaTransform.GetComponent<Animator>();
        StartCoroutine(TitanAttack());
    }

    IEnumerator TitanAttack()
    {
        // darken world
        Light[] lights = FindObjectsOfType<Light>();
        foreach (Light light in lights)
            if (light.type == LightType.Directional)
                light.intensity *= 0.5f;

        yield return new WaitForSeconds(startTime);

        Sequence sequence = DOTween.Sequence();

        Vector3 movePosition = new Vector3(bestiaTransform.position.x, transform.position.y, bestiaTransform.position.z);
        sequence.Append(bestiaTransform.transform.DOMove(movePosition, 2));
        yield return sequence.WaitForElapsedLoops(1);

        _animator.SetBool("Shout", true);
        yield return new WaitForSeconds(2);

        sequence = DOTween.Sequence();
        _animator.SetBool("Shout", false);
        _animator.SetBool("Walk", true);
        movePosition = transform.position - Vector3.right * bestiaTransform.localScale.x * 2;
        sequence.Append(bestiaTransform.transform.DOMove(movePosition, 2));
        yield return sequence.WaitForElapsedLoops(1);

        _animator.SetBool("Walk", false);
        _animator.SetBool("Attack", true);
        yield return new WaitForSeconds(4);

        _animator.SetBool("Attack", false);
        _animator.SetBool("Shout", true);
        yield return new WaitForSeconds(2);


        // brighten world
        foreach (Light light in lights)
            if (light.type == LightType.Directional)
                light.intensity *= 2f;

        Destroy(gameObject);
    }
}
