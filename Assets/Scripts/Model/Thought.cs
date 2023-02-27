using UnityEngine;

namespace Communiganda.Model {
    public enum Thought {
        Nothing,
        Danger,
        Money,
        Food,
        Love
    }

    static class ThoughtExtensions {
        static readonly Sprite[] sprites = new[] {
        null,
        Resources.Load<Sprite>("Thoughts/symbol_hole"),
        Resources.Load<Sprite>("Thoughts/symbol_money"),
        Resources.Load<Sprite>("Thoughts/symbol_hunger"),
        Resources.Load<Sprite>("Thoughts/symbol_heart")
    };
        public static Sprite GetSprite(this Thought thought) {
            return sprites[(int)thought];
        }
        public static void Battle(this Thought _, IEncounterable sender, IEncounterable receiver) {
            if (!receiver.HasThought()) {
                receiver.SetThought(sender.GetThought());
            }
            if (!sender.HasThought()) {
                sender.SetThought(receiver.GetThought());
            }

            receiver.SetThought(new[] { sender.GetThought(), receiver.GetThought() }.RandomElement());
            sender.SetThought(new[] { sender.GetThought(), receiver.GetThought() }.RandomElement());
        }
    }
}