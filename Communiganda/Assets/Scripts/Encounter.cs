using UnityEngine;
using UnityEditor;
using System.Collections;

public static class Encounter 
{
    public static IEnumerator Create(SpecimenBehavior sender, SpecimenBehavior receiver)
    {
        sender.PrepareEncounter(receiver);
        receiver.PrepareEncounter(sender);

        var distance = 2f;
        var x = receiver.transform.position.x + distance;
        var y = receiver.transform.position.y;

        IEnumerator list = sender.MoveToPositionRoutine(new Vector2(x, y));
        while (list.MoveNext())
        {
            yield return list.Current;
        }

        yield return new WaitForSeconds(0.5f);

        var package = sender.CreatePackage();

        yield return new WaitForSeconds(0.5f);

        if (receiver.ReceivePackage(package)) {
            yield return new WaitForSeconds(0.5f);
            package.thought.Battle(sender, receiver);
        }

        yield return new WaitForSeconds(0.5f);

        sender.FinishEncounter();
        receiver.FinishEncounter();
    }
}