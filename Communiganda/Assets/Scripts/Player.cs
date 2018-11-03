using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float speechAttackRadius = 3f;
    private bool moving = false;

    private PathfindingGrid pathfindingGrid { get { return PathfindingGrid.Instance; } }
    private Rigidbody2D body2d;

    private Vector2 input;

    [SerializeField] private SpriteRenderer leftFoodSpriteRend;
    [SerializeField] private SpriteRenderer rightFootSpriteRend;
    [SerializeField] private SpriteRenderer faceSpriteRend;
    [SerializeField] private SpriteRenderer hatSpriteRend;
    [SerializeField] private SpriteRenderer speechBubbleSpriteRend;
    [SerializeField] private SpriteRenderer speechBubbleSymbolSpriteRend;

    private Coroutine speechAttackRoutine;

    public Thought playerthought;
    [SerializeField] private Image playerThoughtImage;

    private void Awake()
    {
        body2d = GetComponentInChildren<Rigidbody2D>();
        speechBubbleSpriteRend.enabled = false;
        speechBubbleSymbolSpriteRend.enabled = false;
        SetThougtSprites();
    }

    void Start()
    {
        StartCoroutine(AnimatePlayer());
    }

    void Update()
    {
        HandlePlayerInput();
    }

    private void HandlePlayerInput()
    {
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        moving = input.sqrMagnitude > .1f;
        body2d.velocity = input * moveSpeed * Time.deltaTime;
        bool inSpeechAttack = speechAttackRoutine != null;
        if (Input.GetKeyDown(KeyCode.E) && inSpeechAttack == false)
        {
            speechAttackRoutine = StartCoroutine(SpeechAttack());
        }
        bool danger = Input.GetKeyDown(KeyCode.Alpha1);
        bool love = Input.GetKeyDown(KeyCode.Alpha2);
        bool newInput = danger | love;
        if (danger) playerthought = Thought.Danger;
        else if (love) playerthought = Thought.Love;
        if (newInput && inSpeechAttack == false)
        {
            SetThougtSprites();
        }
    }

    private IEnumerator SpeechAttack()
    {
        speechBubbleSpriteRend.enabled = true;
        speechBubbleSymbolSpriteRend.enabled = true;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, speechAttackRadius);
        for (int i = 0; i < colliders.Length; i++)
        {

        }
        yield return new WaitForSeconds(2f);
        speechBubbleSpriteRend.enabled = false; 
        speechBubbleSymbolSpriteRend.enabled = false;
        speechAttackRoutine = null;
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
                faceSpriteRend.transform.localPosition = faceDefaultPos + (input * .1f);
                float angle = Random.Range(-10f, 10f);
                Quaternion[] hatRotations = new Quaternion[] { Quaternion.Euler(0, 0, angle), Quaternion.Euler(0, 0, -angle) };
                hatSpriteRend.transform.localRotation = hatRotations[hatRotationCounter % hatRotations.Length];
                hatRotationCounter++;
                yield return new WaitForSeconds(.1f);
            }
            yield return null;
        }
    }

    public void SetThougtSprites()
    {
        speechBubbleSymbolSpriteRend.sprite = playerthought.GetSprite();
      //  playerThoughtImage.sprite = playerthought.GetSprite();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, speechAttackRadius);
    }
}
