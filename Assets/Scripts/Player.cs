using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IEncounterable {
    [SerializeField] float moveSpeed = 10;
    [SerializeField] float speechAttackRadius = 3f;
    [SerializeField] AnimationCurve speechAttackAnimationCurve;

    bool moving = false;

    PathfindingGrid pathfindingGrid { get { return PathfindingGrid.Instance; } }
    Rigidbody2D body2d;

    Vector2 input;
    GameObject face;

    [SerializeField] SpriteRenderer leftFoodSpriteRend;
    [SerializeField] SpriteRenderer rightFootSpriteRend;
    [SerializeField] SpriteRenderer faceSpriteRend;
    [SerializeField] SpriteRenderer hatSpriteRend;
    [SerializeField] SpriteRenderer bodySpriteRend;

    [SerializeField] LayerMask npcLayer;

    Coroutine speechAttackRoutine;

    GameObject sendBubble;
    SpriteRenderer sendSymbolSpriteRenderer;

    public Thought thought;
    Transform lookingTarget;

    void Awake() {
        body2d = GetComponentInChildren<Rigidbody2D>();
    }

    void Start() {
        face = transform.Find("Sprites").Find("Face").gameObject;

        sendBubble = transform.Find("Sprites").Find("SpeechBubble").gameObject;
        sendSymbolSpriteRenderer = sendBubble.transform.Find("Symbol").GetComponent<SpriteRenderer>();

        StartCoroutine(AnimatePlayer());
        ClearLookingTarget();
    }

    void Update() {
        if (speechAttackRoutine == null) {
            HandlePlayerInput();
        }

        UpdateFace();
    }
    void UpdateFace() {
        var target = new Vector2(lookingTarget.position.x, lookingTarget.position.y) + body2d.velocity;

        float x = target.x - transform.position.x;
        x = Mathf.Clamp(x, -0.25f, 0.25f);

        float y = target.y - transform.position.y;
        y = Mathf.Clamp(y, -0.25f, 0.25f);

        face.transform.localPosition = new Vector2(x, y);
    }

    void HandlePlayerInput() {
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        moving = input.sqrMagnitude > .1f;
        body2d.velocity = input * moveSpeed * Time.deltaTime;
    }

    public void StartSpeechAttack() {
        if (speechAttackRoutine != null) {
            return;
        }

        AbortAction();

        var colliders = Physics2D.OverlapCircleAll(transform.position, speechAttackRadius, npcLayer);
        if (colliders.Length > 0) {
            Array.Sort(colliders, (c1, c2) => Vector2.Distance(c1.transform.position, transform.position).CompareTo(Vector2.Distance(c2.transform.position, transform.position)));
            var collision = colliders[0];
            var other = collision.gameObject;
            var npc = other.GetComponent<SpecimenBehavior>();

            AbortAction();
            npc.AbortAction();
            speechAttackRoutine = StartCoroutine(Encounter.Create(this, npc));
            Utility.instance.ScaleGameObject(transform, new Vector3(2, 2, 2), .3f, speechAttackAnimationCurve);
        }
    }

    IEnumerator AnimatePlayer() {
        Vector2 faceDefaultPos = faceSpriteRend.transform.localPosition;

        int hatRotationCounter = 0;
        while (true) {
            if (moving) {
                leftFoodSpriteRend.flipY = !leftFoodSpriteRend.flipY;
                rightFootSpriteRend.flipY = !rightFootSpriteRend.flipY;
                //faceSpriteRend.transform.localPosition = faceDefaultPos + (input * .1f);
                float angle = UnityEngine.Random.Range(-10f, 10f);
                var hatRotations = new Quaternion[] { Quaternion.Euler(0, 0, -9f), Quaternion.Euler(0, 0, 9f) };
                bodySpriteRend.transform.localRotation = hatRotations[hatRotationCounter % hatRotations.Length];
                hatRotationCounter++;
                yield return new WaitForSeconds(.1f);
            }
            yield return null;
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, speechAttackRadius);
    }

    void ClearLookingTarget() {
        SetLookingTarget(transform);
    }
    void SetLookingTarget(Transform target) {
        lookingTarget = target;
    }

    public Transform GetTransform() {
        return transform;
    }

    public bool HasThought() {
        return thought != Thought.Nothing;
    }

    public Thought GetThought() {
        return thought;
    }

    public void SetThought(Thought thought) {
        this.thought = thought;
    }

    public void AbortAction() {
        body2d.velocity = Vector2.zero;
        moving = false;
        FinishEncounter();
    }

    public void PrepareEncounter(IEncounterable npc) {
        SetLookingTarget(npc.GetTransform());
    }

    public void FinishEncounter() {
        ClearLookingTarget();
        sendBubble.SetActive(false);

        if (speechAttackRoutine != null) {
            StopCoroutine(speechAttackRoutine);
            speechAttackRoutine = null;
        }
    }

    public Package CreatePackage() {
        var ret = new Package {
            channel = Channel.SpokenWords,
            thought = thought
        };

        sendBubble.SetActive(true);
        sendBubble.GetComponent<SpriteRenderer>().color = ret.channel.GetColor();
        sendSymbolSpriteRenderer.sprite = ret.thought.GetSprite();

        AudioManager.instance.PlayRandomSound("PlayerMumble", 0.2f, 1.3f, 1.5f);

        return ret;
    }

    public bool ReceivePackage(Package package) {
        throw new System.NotImplementedException();
    }

    public IEnumerator ApplyThoughtRoutine(Thought senderThought, SpecimenBehavior receiver) {
        receiver.SetThought(senderThought);

        yield return null;
    }

    public void SetWalkingTarget(Transform target) {
        throw new NotImplementedException();
    }
}
