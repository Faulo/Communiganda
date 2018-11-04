using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NesScripts.Controls.PathFind;



public class SpecimenBehavior : MonoBehaviour, IEncounterable
{
    enum State { Idle, Walking, Talking, Trapped };

    private static System.Random random = new System.Random();

    private static PathfindingGrid pathfindingGrid { get { return PathfindingGrid.Instance; } }

    [SerializeField] private Specimen id;
    [SerializeField] private Specimen[] engagesWith;
    [SerializeField] private Channel[] sendsVia;
    [SerializeField] private Channel[] receivesVia;
    [SerializeField] private float moveDuration = 0.1f;
    
    private Thought _thought;
    public Thought thought {
        set {
            _thought = value;
            if (thought == Thought.Nothing)
            {
                thoughtBubble.SetActive(false);
            } else
            {
                thoughtBubble.SetActive(true);
            }
            thoughtSymbolSpriteRenderer.sprite = thought.GetSprite();
        }
        get { return _thought; }
    }

    private State state = State.Idle;
    private IEncounterable lastEncounter;
    private GameObject face;
    private GameObject thoughtBubble;
    private SpriteRenderer thoughtSymbolSpriteRenderer;
    private GameObject sendBubble;
    private SpriteRenderer sendSymbolSpriteRenderer;
    private GameObject receiveBubble;
    private SpriteRenderer receiveSymbolSpriteRenderer;
    private Transform lookingTarget;

    void Start ()
    {
        face = transform.Find("Sprites").Find("Face").gameObject;
        thoughtBubble = transform.Find("Sprites").Find("ToughtBubble").gameObject;
        thoughtSymbolSpriteRenderer = thoughtBubble.transform.Find("Symbol").GetComponent<SpriteRenderer>();

        sendBubble = transform.Find("Sprites").Find("ReceiveBubble").gameObject;
        sendSymbolSpriteRenderer = sendBubble.transform.Find("Symbol").GetComponent<SpriteRenderer>();

        receiveBubble = transform.Find("Sprites").Find("SendBubble").gameObject;
        receiveSymbolSpriteRenderer = receiveBubble.transform.Find("Symbol").GetComponent<SpriteRenderer>();

        thought = new[] { Thought.Nothing, Thought.Nothing, Thought.Nothing, Thought.Love, Thought.Money , Thought.Food }.RandomElement();

        ClearLookingTarget();
    }

    void Update () {
        UpdateFace();
        if (CanWalk())
        {
            AbortAction();
            StartCoroutine(MovementRoutine());
        }
    }
    private void UpdateFace()
    {
        var x = lookingTarget.position.x - transform.position.x;
        x = Mathf.Clamp(x, -0.25f, 0.25f);

        var y = lookingTarget.position.y - transform.position.y;
        y = Mathf.Clamp(y, -0.25f, 0.25f);

        face.transform.localPosition = new Vector2(x, y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;
        SpecimenBehavior npc = other.GetComponent<SpecimenBehavior>();
        if (npc != null) {
            if (this.CanEncounter(npc) && npc.CanEncounter(this) && engagesWith.Contains(npc.id))
            {
                this.AbortAction();
                npc.AbortAction();
                StartCoroutine(Encounter.Create(this, npc));
            }
        }
    }
    public void AbortAction()
    {
        StopAllCoroutines();
        if (state == State.Talking)
        {
            FinishEncounter();
            lastEncounter.AbortAction();
        }
        if (state != State.Trapped) {
            state = State.Idle;
        }
    }

    public IEnumerator MoveToPositionRoutine(Vector2 to)
    {
        Vector2 from = new Vector2(transform.position.x, transform.position.y);
        float distance = Vector2.Distance(from, to);
        return Utility.instance.MoveToWaypointsRoutine(transform, moveDuration * distance, null, new[] { from, to });
    }

    private IEnumerator MovementRoutine()
    {
        state = State.Walking;

        Point _from = pathfindingGrid.ConvertPositionToPoint(new Vector2(transform.position.x, transform.position.y));
        Point _to = pathfindingGrid.GenerateRandomTargetPointInsideGrid();

        IEnumerator routine = Utility.instance.MoveToWaypointsRoutine(transform, moveDuration, null, pathfindingGrid.GetWaypoints(_from, _to));
        yield return StartCoroutine(routine);

        yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 2.0f) * Time.timeScale);

        state = State.Idle;
    }

    public Package CreatePackage()
    {
        var ret = new Package();
        ret.channel = sendsVia.RandomElement();
        ret.thought = thought;

        sendBubble.SetActive(true);
        sendBubble.GetComponent<SpriteRenderer>().color = ret.channel.GetColor();
        sendSymbolSpriteRenderer.sprite = ret.thought.GetSprite();

        return ret;
    }

    public bool ReceivePackage(Package package)
    {
        Channel channel = receivesVia.Contains(package.channel) ? package.channel : receivesVia.RandomElement();
        receiveBubble.SetActive(true);
        receiveBubble.GetComponent<SpriteRenderer>().color = channel.GetColor();
        receiveSymbolSpriteRenderer.sprite = thought.GetSprite();
        return receivesVia.Contains(package.channel);
    }

    private void ClearLookingTarget()
    {
        SetLookingTarget(transform);
    }
    private void SetLookingTarget(Transform target)
    {
        lookingTarget = target;
    }

    public bool CanWalk()
    {
        switch (state)
        {
            case State.Idle:
                return true;
            case State.Walking:
            case State.Talking:
            case State.Trapped:
                return false;
            default:
                throw new ArgumentOutOfRangeException("Unknown state: " + state);
        }
    }

    private bool CanEncounter(SpecimenBehavior npc)
    {
        switch (state)
        {
            case State.Idle:
            case State.Walking:
                return !npc.Equals(lastEncounter) && npc.thought != thought;
            case State.Talking:
            case State.Trapped:
                return false;
            default:
                throw new ArgumentOutOfRangeException("Unknown state: " + state);
        }
    }
    public void PrepareEncounter(IEncounterable npc)
    {
        lastEncounter = npc;
        state = State.Talking;
        SetLookingTarget(npc.GetTransform());
    }
    public void FinishEncounter()
    {
        state = State.Idle;
        ClearLookingTarget();
        sendBubble.SetActive(false);
        receiveBubble.SetActive(false);
    }

    public void TrapIn(Transform target)
    {
        state = State.Trapped;
        thought = Thought.Danger;
        transform.position = target.position;
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
}
