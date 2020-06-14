using System.Collections;
using UnityEngine;

public class SpecimenManager : MonoBehaviour {
    [SerializeField] GameObject[] specimenPrefabs;

    void Start() {
        StartCoroutine(SpawnSpecimenRoutine());
    }

    void Update() {
    }

    public void SpawnSpecimen(Vector3 position, Vector3 scale) {
        var specimen = Instantiate(specimenPrefabs.RandomElement(), position, Quaternion.identity);
        specimen.transform.position = position;
        specimen.transform.parent = transform;
        specimen.transform.localScale = 0.5f * scale;
    }

    IEnumerator SpawnSpecimenRoutine() {
        while (true) {
            SpawnSpecimen(Vector3.zero, Vector3.zero);
            yield return new WaitForSeconds(5);
        }
    }
}
