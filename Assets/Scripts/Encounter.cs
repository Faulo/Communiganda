using System.Collections;
using UnityEngine;

namespace Communiganda {
    public static class Encounter {
        public static IEnumerator Create(IEncounterable sender, SpecimenBehavior receiver) {
            sender.PrepareEncounter(receiver);
            receiver.PrepareEncounter(sender);

            float distance = 2f;
            float x = sender.GetTransform().position.x + distance;
            float y = sender.GetTransform().position.y;

            var list = receiver.MoveToPositionRoutine(new Vector2(x, y));
            while (list.MoveNext()) {
                yield return list.Current;
            }

            yield return new WaitForSeconds(0.5f);

            var package = sender.CreatePackage();
            yield return new WaitForSeconds(0.8f);

            bool communicationSuccessful = receiver.ReceivePackage(package);

            yield return new WaitForSeconds(0.8f);

            if (communicationSuccessful) {
                var thoughts = sender.ApplyThoughtRoutine(package.thought, receiver);
                while (thoughts.MoveNext()) {
                    yield return thoughts.Current;
                }
            }

            sender.FinishEncounter();
            receiver.FinishEncounter();
        }
    }
}