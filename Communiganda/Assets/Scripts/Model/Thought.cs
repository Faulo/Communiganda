using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Thought {
    Nothing,
    Danger,
    Money,
    Food,
    Love
}

static class ThoughtExtensions
{
    private static Sprite[] sprites = new[] {
        null,
        Resources.Load<Sprite>("Thoughts/symbol_hole"),
        Resources.Load<Sprite>("Thoughts/symbol_money"),
        Resources.Load<Sprite>("Thoughts/symbol_hunger"),
        Resources.Load<Sprite>("Thoughts/symbol_heart")
    };
    public static Sprite GetSprite(this Thought thought)
    {
        return sprites[(int) thought];
    }
    public static void Battle(this Thought thought, SpecimenBehavior sender, SpecimenBehavior receiver)
    {
        receiver.thought = new[] { sender.thought, receiver.thought }.RandomElement();
        sender.thought = new[] { sender.thought, receiver.thought }.RandomElement();
    }
}