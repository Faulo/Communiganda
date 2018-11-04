using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;
        SpecimenBehavior npc = other.GetComponent<SpecimenBehavior>();
        if (npc != null)
        {
            npc.AbortAction();

            if (npc.thought != Thought.Danger)
            {
                npc.TrapIn(transform);
            }
        }
    }
}
