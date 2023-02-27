using System;
using System.Collections;
using Communiganda.Model;
using NesScripts.Controls.PathFind;
using UnityEngine;

namespace Communiganda {
    public class SpecimenBehavior : MonoBehaviour, IEncounterable {
        enum State { Idle, Walking, Talking, Trapped };

        static readonly System.Random random = new System.Random();

        static PathfindingGrid pathfindingGrid {
            get {
                return PathfindingGrid.Instance;
            }
        }

        SpecimenManager manager {
            get {
                return GetComponentInParent<SpecimenManager>();
            }
        }

        [SerializeField] Specimen id;
        [SerializeField] Specimen[] engagesWith;
        [SerializeField] Channel[] sendsVia;
        [SerializeField] Channel[] receivesVia;
        [SerializeField] float moveDuration = 0.1f;

        [SerializeField] AnimationCurve encounterAnimationCurve;

        Thought _thought;
        public Thought thought {
            set {
                _thought = value;
                if (thought == Thought.Nothing) {
                    thoughtBubble.SetActive(false);
                } else {
                    thoughtBubble.SetActive(true);
                }
                thoughtSymbolSpriteRenderer.sprite = thought.GetSprite();
            }
            get { return _thought; }
        }

        State state = State.Idle;
        bool isDying = false;

        IEncounterable lastEncounter;
        GameObject face;
        GameObject thoughtBubble;
        SpriteRenderer thoughtSymbolSpriteRenderer;
        GameObject sendBubble;
        SpriteRenderer sendSymbolSpriteRenderer;
        GameObject receiveBubble;
        SpriteRenderer receiveSymbolSpriteRenderer;

        Transform lookingTarget;
        Transform walkingTarget;

        Transform bodyTransform;
        SpriteRenderer leftFootSpriteRenderer;
        SpriteRenderer rightFootSpriteRenderer;


        void Start() {
            face = transform.Find("Sprites").Find("Face").gameObject;
            thoughtBubble = transform.Find("Sprites").Find("ToughtBubble").gameObject;
            thoughtSymbolSpriteRenderer = thoughtBubble.transform.Find("Symbol").GetComponent<SpriteRenderer>();

            sendBubble = transform.Find("Sprites").Find("ReceiveBubble").gameObject;
            sendSymbolSpriteRenderer = sendBubble.transform.Find("Symbol").GetComponent<SpriteRenderer>();

            receiveBubble = transform.Find("Sprites").Find("SendBubble").gameObject;
            receiveSymbolSpriteRenderer = receiveBubble.transform.Find("Symbol").GetComponent<SpriteRenderer>();

            bodyTransform = transform.Find("Sprites").Find("Body");
            leftFootSpriteRenderer = transform.Find("Sprites").Find("LeftFoot").GetComponent<SpriteRenderer>();
            rightFootSpriteRenderer = transform.Find("Sprites").Find("RightFoot").GetComponent<SpriteRenderer>();

            //thought = new[] { Thought.Nothing, Thought.Nothing, Thought.Nothing, Thought.Love, Thought.Money, Thought.Food }.RandomElement();
            //thought = Thought.Food;

            ClearLookingTarget();
            StartCoroutine(WalkingAnimation());
        }

        void Update() {
            if (CanDie()) {
                Destroy(gameObject);
            }
            UpdateFace();
            UpdateScale();
            if (CanWalk()) {
                AbortAction();
                StartCoroutine(MovementRoutine());
            }
        }
        void UpdateFace() {
            float x = lookingTarget.position.x - transform.position.x;
            x = Mathf.Clamp(x, -0.25f, 0.25f);

            float y = lookingTarget.position.y - transform.position.y;
            y = Mathf.Clamp(y, -0.25f, 0.25f);

            face.transform.localPosition = new Vector2(x, y);
        }
        void UpdateScale() {
            if (transform.localScale.z < 1.0f) {
                transform.localScale += Time.deltaTime * Vector3.one * 0.1f;
            }
        }

        void OnTriggerEnter2D(Collider2D collision) {
            var other = collision.gameObject;
            var npc = other.GetComponent<SpecimenBehavior>();
            if (npc != null) {
                if (CanEncounter(npc) && npc.CanEncounter(this) && engagesWith.Contains(npc.id)) {
                    AbortAction();
                    npc.AbortAction();
                    StartCoroutine(Encounter.Create(this, npc));
                }
            }
        }
        public void AbortAction() {
            StopAllCoroutines();
            if (state == State.Talking) {
                FinishEncounter();
                lastEncounter.AbortAction();
            }
            if (state != State.Trapped) {
                state = State.Idle;
            }
            StartCoroutine(WalkingAnimation());

        }

        public IEnumerator MoveToPositionRoutine(Vector2 to) {
            var from = new Vector2(transform.position.x, transform.position.y);
            float distance = Vector2.Distance(from, to);
            return Utility.instance.MoveToWaypointsRoutine(transform, moveDuration * distance, null, new[] { from, to });
        }

        IEnumerator MovementRoutine() {
            state = State.Walking;
            var _from = pathfindingGrid.ConvertPositionToPoint(new Vector2(transform.position.x, transform.position.y));
            Point _to;
            if (walkingTarget == null) {
                _to = pathfindingGrid.GenerateRandomTargetPointInsideGrid();
            } else {
                _to = pathfindingGrid.ConvertPositionToPoint(new Vector2(walkingTarget.position.x, walkingTarget.position.y));
                ;
                walkingTarget = null;
            }

            var routine = Utility.instance.MoveToWaypointsRoutine(transform, moveDuration, null, pathfindingGrid.GetWaypoints(_from, _to));
            yield return StartCoroutine(routine);

            yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 2.0f) * Time.timeScale);

            state = State.Idle;
        }

        public Package CreatePackage() {
            var ret = new Package {
                channel = sendsVia.RandomElement(),
                thought = thought
            };

            sendBubble.SetActive(true);
            sendBubble.GetComponent<SpriteRenderer>().color = ret.channel.GetColor();
            sendSymbolSpriteRenderer.sprite = ret.thought.GetSprite();

            AudioManager.instance.PlayRandomSound("Mumble", 0.5f, 0.2f);

            return ret;
        }

        public bool ReceivePackage(Package package) {
            var channel = receivesVia.Contains(package.channel) ? package.channel : receivesVia.RandomElement();
            receiveBubble.SetActive(true);
            receiveBubble.GetComponent<SpriteRenderer>().color = channel.GetColor();
            receiveSymbolSpriteRenderer.sprite = thought.GetSprite();

            AudioManager.instance.PlayRandomSound("Mumble", 0.5f, 0.2f);

            return receivesVia.Contains(package.channel);
        }

        void ClearLookingTarget() {
            SetLookingTarget(transform);
        }
        void SetLookingTarget(Transform target) {
            lookingTarget = target;
        }
        public void SetWalkingTarget(Transform target) {
            walkingTarget = target;
            SetLookingTarget(target);
        }

        public bool CanWalk() {
            switch (state) {
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

        public bool CanDie() {
            return isDying && state == State.Idle;
        }

        bool CanEncounter(SpecimenBehavior npc) {
            switch (state) {
                case State.Idle:
                case State.Walking:
                    return !npc.Equals(lastEncounter);
                case State.Talking:
                case State.Trapped:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException("Unknown state: " + state);
            }
        }
        public void PrepareEncounter(IEncounterable npc) {
            lastEncounter = npc;
            state = State.Talking;
            SetLookingTarget(npc.GetTransform());
            Utility.instance.ScaleGameObject(transform, new Vector3(2, 2, 2), .3f, encounterAnimationCurve);
            AudioManager.instance.PlaySound("EncounterTrigger");
        }
        public void FinishEncounter() {
            state = State.Idle;
            ClearLookingTarget();
            sendBubble.SetActive(false);
            receiveBubble.SetActive(false);
        }

        public void TrapIn(Transform target) {
            state = State.Trapped;
            thought = Thought.Danger;
            transform.position = target.position + new Vector3(UnityEngine.Random.Range(-0.4f, 0.4f), UnityEngine.Random.Range(-0.1f, 0.5f));
            AudioManager.instance.PlaySound("FallingIntoHole");
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


        IEnumerator WalkingAnimation() {
            while (true) {
                if (state == State.Walking) {
                    leftFootSpriteRenderer.flipY = !leftFootSpriteRenderer.flipY;
                    rightFootSpriteRenderer.flipY = !rightFootSpriteRenderer.flipY;
                    yield return new WaitForSeconds(.1f);
                }

                yield return null;
            }
        }


        public IEnumerator ApplyThoughtRoutine(Thought senderThought, SpecimenBehavior receiver) {
            var receiverThought = receiver.GetThought();

            if (senderThought == receiverThought) {
                switch (senderThought) {
                    case Thought.Danger:
                        var danger = FindObjectsOfType<DangerBehavior>().RandomElement().gameObject.transform;
                        SetWalkingTarget(danger);
                        receiver.SetWalkingTarget(danger);
                        yield return new WaitForSeconds(0.25f);
                        break;
                    case Thought.Food:
                        var foodSteps = receiver.MoveToPositionRoutine(new Vector2(transform.position.x + 0.5f, transform.position.y));
                        while (foodSteps.MoveNext()) {
                            yield return foodSteps.Current;
                        }
                        AudioManager.instance.PlaySound("NPCEatingNPC");
                        while (transform.localScale.z > 0) {
                            transform.localScale -= 0.1f * Vector3.one;
                            yield return new WaitForSeconds(0.1f);
                        }
                        transform.Find("Sprites").gameObject.SetActive(false);
                        isDying = true;
                        receiver.SetThought(Thought.Nothing);
                        yield return new WaitForSeconds(0.25f);
                        break;
                    case Thought.Love:
                        var loveSteps = receiver.MoveToPositionRoutine(new Vector2(transform.position.x + 1f, transform.position.y));
                        while (loveSteps.MoveNext()) {
                            yield return loveSteps.Current;
                        }
                        //@TODO play love-making sound here
                        yield return new WaitForSeconds(1.0f);
                        AudioManager.instance.PlaySound("SpawnNewNPC");
                        manager.SpawnSpecimen(transform.position + new Vector3(0, 0.5f, 0), transform.localScale);
                        SetThought(Thought.Nothing);
                        receiver.SetThought(Thought.Nothing);
                        yield return new WaitForSeconds(0.5f);
                        break;
                    case Thought.Money:
                        //@TODO ?
                        SetThought(Thought.Nothing);
                        receiver.SetThought(Thought.Nothing);
                        break;
                    case Thought.Nothing:
                        new[] { this, receiver }.RandomElement().SetThought(new[] { Thought.Love, Thought.Money, Thought.Food }.RandomElement());
                        yield return new WaitForSeconds(0.25f);
                        break;
                }
            } else {
                if (HasThought()) {
                    if (receiver.HasThought()) {
                        var thoughts = new[] { receiverThought, senderThought };
                        SetThought(thoughts.RandomElement());
                        receiver.SetThought(thoughts.RandomElement());
                    } else {
                        receiver.SetThought(senderThought);
                    }
                } else {
                    SetThought(receiverThought);
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}