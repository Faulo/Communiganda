using System;
using System.Collections;
using System.Collections.Generic;
using NesScripts.Controls.PathFind;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IEncounterable
{
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float speechAttackRadius = 3f;
    [SerializeField] private AnimationCurve speechAttackAnimationCurve;

    private bool moving = false;

    private PathfindingGrid pathfindingGrid { get { return PathfindingGrid.Instance; } }
    private Rigidbody2D body2d;

    private Vector2 input;
    private GameObject face;

    [SerializeField] private SpriteRenderer leftFoodSpriteRend;
    [SerializeField] private SpriteRenderer rightFootSpriteRend;
    [SerializeField] private SpriteRenderer faceSpriteRend;
    [SerializeField] private SpriteRenderer hatSpriteRend;
    [SerializeField] private LayerMask npcLayer;

    private Coroutine speechAttackRoutine;

    private GameObject sendBubble;
    private SpriteRenderer sendSymbolSpriteRenderer;

    public Thought thought;
    private Transform lookingTarget;

    private void Awake()
    {
        body2d = GetComponentInChildren<Rigidbody2D>();
    }

    void Start()
    {
        face = transform.Find("Sprites").Find("Face").gameObject;

        sendBubble = transform.Find("Sprites").Find("SpeechBubble").gameObject;
        sendSymbolSpriteRenderer = sendBubble.transform.Find("Symbol").GetComponent<SpriteRenderer>();

        StartCoroutine(AnimatePlayer());
        ClearLookingTarget();
    }

    void Update()
    {
        if (speechAttackRoutine == null)
        {
            HandlePlayerInput();
        }

        UpdateFace();
    }
    private void UpdateFace()
    {
        Vector2 target = new Vector2(lookingTarget.position.x, lookingTarget.position.y) + body2d.velocity;

        var x = target.x - transform.position.x;
        x = Mathf.Clamp(x, -0.25f, 0.25f);

        var y = target.y - transform.position.y;
        y = Mathf.Clamp(y, -0.25f, 0.25f);

        face.transform.localPosition = new Vector2(x, y);
    }

    private void HandlePlayerInput()
    {
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        moving = input.sqrMagnitude > .1f;
        body2d.velocity = input * moveSpeed * Time.deltaTime;
    }

    public void StartSpeechAttack()
    {
        if (speechAttackRoutine != null)
        {
            return;
        }

        AbortAction();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, speechAttackRadius, npcLayer);
        if (colliders.Length > 0)
        {
            Array.Sort(colliders, (c1, c2) => Vector2.Distance(c1.transform.position, transform.position).CompareTo(Vector2.Distance(c2.transform.position, transform.position)));
            Collider2D collision = colliders[0];
            GameObject other = collision.gameObject;
            SpecimenBehavior npc = other.GetComponent<SpecimenBehavior>();

            this.AbortAction();
            npc.AbortAction();
            speechAttackRoutine = StartCoroutine(Encounter.Create(this, npc));
            Utility.instance.ScaleGameObject(transform, new Vector3(2, 2, 2), .3f, speechAttackAnimationCurve);
        }
    }

    private IEnumerator AnimatePlayer()
    {
        Vector2 faceDefaultPos = faceSpriteRend.transform.localPosition;

        int hatRotationCounter = 0;
        while (true)
        {
            if (moving)
            {
                leftFoodSpriteRend.flipY = !leftFoodSpriteRend.flipY;
                rightFootSpriteRend.flipY = !rightFootSpriteRend.flipY;
                //faceSpriteRend.transform.localPosition = faceDefaultPos + (input * .1f);
                float angle = UnityEngine.Random.Range(-10f, 10f);
                Quaternion[] hatRotations = new Quaternion[] { Quaternion.Euler(0, 0, angle), Quaternion.Euler(0, 0, -angle) };
                hatSpriteRend.transform.localRotation = hatRotations[hatRotationCounter % hatRotations.Length];
                hatRotationCounter++;
                yield return new WaitForSeconds(.1f);
            }
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, speechAttackRadius);
    }

    private void ClearLookingTarget()
    {
        SetLookingTarget(transform);
    }
    private void SetLookingTarget(Transform target)
    {
        lookingTarget = target;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public bool HasThought()
    {
        return thought != Thought.Nothing;
    }

    public Thought GetThought()
    {
        return thought;
    }

    public void SetThought(Thought thought)
    {
        this.thought = thought;
    }

    public void AbortAction()
    {
        body2d.velocity = Vector2.zero;
        moving = false;
        FinishEncounter();
    }

    public void PrepareEncounter(IEncounterable npc)
    {
        SetLookingTarget(npc.GetTransform());
    }

    public void FinishEncounter()
    {
        ClearLookingTarget();
        sendBubble.SetActive(false);

        if (speechAttackRoutine != null)
        {
            StopCoroutine(speechAttackRoutine);
            speechAttackRoutine = null;
        }
    }

    public Package CreatePackage()
    {
        var ret = new Package();
        ret.channel = Channel.SpokenWords;
        ret.thought = thought;

        sendBubble.SetActive(true);
        sendBubble.GetComponent<SpriteRenderer>().color = ret.channel.GetColor();
        sendSymbolSpriteRenderer.sprite = ret.thought.GetSprite();

        return ret;
    }

    public bool ReceivePackage(Package package)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator ApplyThoughtRoutine(Thought senderThought, SpecimenBehavior receiver)
    {
        receiver.SetThought(senderThought);

        yield return new WaitForSeconds(0.5f);
    }

    public void SetWalkingTarget(Transform target)
    {
        throw new NotImplementedException();
    }
}
