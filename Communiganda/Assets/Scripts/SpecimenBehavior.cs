using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NesScripts.Controls.PathFind;



public class SpecimenBehavior : MonoBehaviour
{
    enum State { Idle, Walking, Busy, Trapped };

    private static System.Random random = new System.Random();

    private static PathfindingGrid pathfindingGrid { get { return PathfindingGrid.Instance; } }

    [SerializeField] private Specimen id;
    [SerializeField] private Specimen[] engagesWith;
    [SerializeField] private Channel[] sendsVia;
    [SerializeField] private Channel[] receivesVia;
    [SerializeField] private float moveDuration = 0.1f;
    
    private Thought _thought;
    private Thought thought {
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
    private SpecimenBehavior lastSpecimen;
    private GameObject face;
    private GameObject thoughtBubble;
    private SpriteRenderer thoughtSymbolSpriteRenderer;
    private GameObject sendBubble;
    private SpriteRenderer sendSymbolSpriteRenderer;
    private GameObject receiveBubble;
    private SpriteRenderer receiveSymbolSpriteRenderer;
    private Transform lookingTarget;

    private void Start ()
    {
        face = transform.Find("Sprites").Find("Face").gameObject;
        thoughtBubble = transform.Find("Sprites").Find("ToughtBubble").gameObject;
        thoughtSymbolSpriteRenderer = thoughtBubble.transform.Find("Symbol").GetComponent<SpriteRenderer>();

        sendBubble = transform.Find("Sprites").Find("SendBubble").gameObject;
        sendSymbolSpriteRenderer = sendBubble.transform.Find("Symbol").GetComponent<SpriteRenderer>();

        receiveBubble = transform.Find("Sprites").Find("ReceiveBubble").gameObject;
        receiveSymbolSpriteRenderer = receiveBubble.transform.Find("Symbol").GetComponent<SpriteRenderer>();

        thought = new[] { Thought.Nothing, Thought.Love, Thought.Money , Thought.Food }.RandomElement();

        ClearLookingTarget();
    }

    private void Update () {
        UpdateFace();
        if (CanWalk())
        {
            StopAllCoroutines();
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
            if (this.CanEncounter(npc) && npc.CanEncounter(this))
            {
                this.StopAllCoroutines();
                npc.StopAllCoroutines();
                StartCoroutine(thought == Thought.Nothing ? Encounter.Create(npc, this) : Encounter.Create(this, npc));
            }
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

    public void ReceivePackage(Package package)
    {
        Channel channel = receivesVia.Contains(package.channel) ? package.channel : receivesVia.RandomElement();
        receiveBubble.SetActive(true);
        receiveBubble.GetComponent<SpriteRenderer>().color = channel.GetColor();
        receiveSymbolSpriteRenderer.sprite = thought.GetSprite();
    }

    public void ProcessPackage(Package package)
    {
        if (receivesVia.Contains(package.channel))
        {
            thought = package.thought;
        }
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
            case State.Busy:
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
                return npc != lastSpecimen && engagesWith.Contains(npc.id);
            case State.Busy:
            case State.Trapped:
                return false;
            default:
                throw new ArgumentOutOfRangeException("Unknown state: " + state);
        }
    }
    public void PrepareEncounter(SpecimenBehavior npc)
    {
        lastSpecimen = npc;
        state = State.Busy;
        SetLookingTarget(npc.transform);
    }
    public void FinishEncounter()
    {
        state = State.Idle;
        ClearLookingTarget();
        sendBubble.SetActive(false);
        receiveBubble.SetActive(false);
    }
}
