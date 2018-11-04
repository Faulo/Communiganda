using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerBehavior : MonoBehaviour
{

    [SerializeField] private float animationDuration = .5f;
    [SerializeField] private AnimationCurve animationCurve;

    [SerializeField] private Vector3 maxScale;

    private Coroutine animationRoutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;
        SpecimenBehavior npc = other.GetComponent<SpecimenBehavior>();
        if (npc != null)
        {
            npc.AbortAction();

            if (npc.thought != Thought.Danger)
            {
                AudioManager.instance.PlaySound("FallingIntoHole", .15f);
                npc.TrapIn(transform);
                if (animationRoutine != null)
                {
                    StopCoroutine(animationRoutine);
                    transform.localScale = Vector3.one;
                }
                animationRoutine = StartCoroutine(Utility.instance.ScaleGameObjectRoutine(transform, maxScale, animationDuration, animationCurve));
            }
        }
    }
}
