using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SpecimenBehavior : MonoBehaviour {
    [SerializeField] private Specimen id;
    [SerializeField] private Specimen[] engagesWith;
    [SerializeField] private Channel[] sendsVia;
    [SerializeField] private Channel[] receivesVia;

    public bool isBusy;

    private NPC_Pathfinding pathFinding;

    // Use this for initialization
    void Start ()
    {
        pathFinding = GetComponent<NPC_Pathfinding>();
    }
	
	// Update is called once per frame
	void Update () {

	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;
        SpecimenBehavior npc = other.GetComponent<SpecimenBehavior>();
        if (npc != null) {
            if (isBusy == false && npc.isBusy == false)
            {
                StartCoroutine(Encounter(npc));
            }
        }
    }

    IEnumerator Encounter(SpecimenBehavior npc)
    {
        this.isBusy = true;
        npc.isBusy = true;

        this.pathFinding.StopMoving();
        npc.pathFinding.StopMoving();

        var x = (transform.position.x + npc.transform.position.x) / 2;
        var y = (transform.position.y + npc.transform.position.y) / 2;
        var distance = 1f;

        this.transform.position = new Vector3(x - distance / 2, y, this.transform.position.z);
        npc.transform.position = new Vector3(x + distance / 2, y, npc.transform.position.z);

        Debug.Log("this + npc");

        yield return new WaitForSeconds(1f);

        isBusy = false;
        npc.isBusy = false;
    }
}
