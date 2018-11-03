using UnityEngine;
using UnityEditor;

public class Encounter
{
    public GameObject sender;
    public GameObject receiver;

    public Encounter(GameObject sender, GameObject receiver)
    {
        this.sender = sender;
        this.receiver = receiver;
    }
}