using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Thought {
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
            case Thought.Danger: return "🕳";
            case Thought.Money: return "💰";
            case Thought.Food: return "🍽";
            case Thought.Love: return "❤";
            default: throw new ArgumentOutOfRangeException("thought");
        }
    }
}