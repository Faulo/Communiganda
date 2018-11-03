using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float speechAttackRadius = 3f;

    void Start()
    {

    }

    void Update()
    {
        HandlePlayerInput();
    }

    private void HandlePlayerInput()
    {
        var input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        transform.Translate(input * moveSpeed * Time.deltaTime);
    }

    private void SpeechAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, speechAttackRadius);
        for (int i = 0; i < colliders.Length; i++)
        {
           // colliders[i].GetComponent<XY>
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, speechAttackRadius);
    }
}
