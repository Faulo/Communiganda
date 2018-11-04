using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecimenManager : MonoBehaviour {
    [SerializeField] private GameObject[] specimenPrefabs;

	void Start ()
    {
        StartCoroutine(SpawnSpecimenRoutine());
    }
	
	void Update () {
	}

    public void SpawnSpecimen(Vector3 position, Vector3 scale)
    {
        GameObject specimen = Instantiate(specimenPrefabs.RandomElement(), position, Quaternion.identity);
        specimen.transform.position = position;
        specimen.transform.parent = transform;
        specimen.transform.localScale = 0.5f * scale;
    }

    private IEnumerator SpawnSpecimenRoutine()
    {
        while (true)
        {
            SpawnSpecimen(Vector3.zero, Vector3.zero);
            yield return new WaitForSeconds(5);
        }
    }
}
