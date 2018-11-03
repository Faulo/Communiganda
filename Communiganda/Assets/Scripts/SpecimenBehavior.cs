using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NesScripts.Controls.PathFind;



public class SpecimenBehavior : MonoBehaviour
{
    enum State { Idle, Walking, Busy, Trapped };

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
            Debug.Log(thought.GetSprite());
        }
        get { return _thought; }
    }

    private State state = State.Idle;
    private SpecimenBehavior lastSpecimen;
    private GameObject face;
    private GameObject thoughtBubble;
    private SpriteRenderer thoughtSymbolSpriteRenderer;

    void Start ()
    {
        face = transform.Find("Sprites").Find("Face").gameObject;
        thoughtBubble = transform.Find("Sprites").Find("ToughtBubble").gameObject;
        thoughtSymbolSpriteRenderer = thoughtBubble.transform.Find("Symbol").GetComponent<SpriteRenderer>();
        thought = Thought.Love;
    }
	
	void Update () {
        if (CanWalk())
        {
            StopAllCoroutines();
            StartCoroutine(MovementRoutine());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;
        SpecimenBehavior npc = other.GetComponent<SpecimenBehavior>();
        if (npc != null) {
            if (CanEncounter() && npc.CanEncounter() && npc != lastSpecimen)
            {
                this.lastSpecimen = npc;
                npc.lastSpecimen = this;

                this.StopAllCoroutines();
                npc.StopAllCoroutines();
                StartCoroutine(EncounterRoutine(npc));
            }
        }
    }

    IEnumerator MoveToPositionRoutine(Vector2 to)
    {
        Vector2 from = new Vector2(transform.position.x, transform.position.y);
        float distance = Vector2.Distance(from, to);
        return Utility.instance.MoveToWaypointsRoutine(transform, moveDuration * distance, null, new[] { from, to });
    }

    IEnumerator MovementRoutine()
    {
        state = State.Walking;

        Point _from = pathfindingGrid.ConvertPositionToPoint(new Vector2(transform.position.x, transform.position.y));
        Point _to = pathfindingGrid.GenerateRandomTargetPointInsideGrid();

        IEnumerator routine = Utility.instance.MoveToWaypointsRoutine(transform, moveDuration, null, pathfindingGrid.GetWaypoints(_from, _to));
        yield return StartCoroutine(routine);

        yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 2.0f));

        state = State.Idle;
    }

    IEnumerator EncounterRoutine(SpecimenBehavior npc)
    {
        this.state = State.Busy;
        npc.state = State.Busy;

        var distance = 1.5f;
        var x = transform.position.x + distance;
        var y = transform.position.y;

        yield return StartCoroutine(npc.MoveToPositionRoutine(new Vector2(x, y)));

        this.LookAtTransform(npc.transform);
        npc.LookAtTransform(this.transform);




        yield return new WaitForSeconds(1f);

        this.state = State.Idle;
        this.LookAtNothing();

        npc.state = State.Idle;
        npc.LookAtNothing();
    }

    void LookAtNothing()
    {
        LookAtTransform(transform);
    }
    void LookAtTransform(Transform target)
    {
        var x = target.position.x - transform.position.x;
        x = Mathf.Clamp(x, -0.25f, 0.25f);

        var y = target.position.y - transform.position.y;
        y = Mathf.Clamp(y, -0.25f, 0.25f);

        face.transform.localPosition = new Vector2(x, y);
    }

    bool CanWalk()
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

    bool CanEncounter()
    {
        switch (state)
        {
            case State.Idle:
            case State.Walking:
                return true;
            case State.Busy:
            case State.Trapped:
                return false;
            default:
                throw new ArgumentOutOfRangeException("Unknown state: " + state);
        }
    }
}
