using System;
using UnityEngine;

namespace Communiganda.Model {
    public enum Channel {
        SpokenWords,
        ShoutedWords,
        WrittenSymbols,
        SignLanguage,
        Tweets
    }

    static class ChannelExtensions {
        public static Color GetColor(this Channel channel) {
            switch (channel) {
                case Channel.SpokenWords:
                    return Color.white;
                case Channel.ShoutedWords:
                    return Color.red;
                case Channel.WrittenSymbols:
                    return Color.green;
                case Channel.SignLanguage:
                    return Color.yellow;
                case Channel.Tweets:
                    return Color.blue;
                default:
                    throw new ArgumentOutOfRangeException("channel");
            }
        }
    }
}