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
    public static string GetText(this Thought thought)
    {
        switch (thought)
        {
            case Thought.Nothing: return "";
            case Thought.Danger: return "🕳";
            case Thought.Money: return "💰";
            case Thought.Food: return "🍽";
            case Thought.Love: return "❤";
            default: throw new ArgumentOutOfRangeException("thought");
        }
    }
    public static Sprite GetSprite(this Thought thought)
    {
        switch (thought)
        {
            case Thought.Nothing: return null;
            case Thought.Danger: return Resources.Load<Sprite>("Thoughts/symbol_hole");
            case Thought.Money: return Resources.Load<Sprite>("Thoughts/symbol_money");
            case Thought.Food: return Resources.Load<Sprite>("Thoughts/symbol_hunger");
            case Thought.Love: return Resources.Load<Sprite>("Thoughts/symbol_heart");
            default: throw new ArgumentOutOfRangeException("thought");
        }
    }
}