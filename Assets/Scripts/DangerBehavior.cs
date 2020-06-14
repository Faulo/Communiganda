using UnityEngine;

public class DangerBehavior : MonoBehaviour {

    [SerializeField] float animationDuration = .5f;
    [SerializeField] AnimationCurve animationCurve;

    [SerializeField] Vector3 maxScale;

    Coroutine animationRoutine;

    void OnTriggerEnter2D(Collider2D collision) {
        var other = collision.gameObject;
        var npc = other.GetComponent<SpecimenBehavior>();
        if (npc != null) {
            npc.AbortAction();

            if (npc.thought != Thought.Danger) {
                npc.TrapIn(transform);
                if (animationRoutine != null) {
                    StopCoroutine(animationRoutine);
                    transform.localScale = Vector3.one;
                }
                animationRoutine = StartCoroutine(Utility.instance.ScaleGameObjectRoutine(transform, maxScale, animationDuration, animationCurve));
            }
        }
    }
}
