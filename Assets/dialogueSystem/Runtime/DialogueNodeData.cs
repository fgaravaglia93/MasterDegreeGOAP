using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.DataContainers
{
    [Serializable]
    public class DialogueNodeData
    {
        public string NodeGUID;
        public string title;
        public string DialogueText;
        public Sprite face;
        public MoodType mood;
        public Vector2 Position;
    }
}