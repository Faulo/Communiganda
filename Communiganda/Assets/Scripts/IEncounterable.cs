using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEncounterable {
    Transform GetTransform();

    bool HasThought();
    Thought GetThought();
    void SetThought(Thought thought);

    void AbortAction();
    void PrepareEncounter(IEncounterable npc);
    void FinishEncounter();

    Package CreatePackage();
    bool ReceivePackage(Package package);
}
