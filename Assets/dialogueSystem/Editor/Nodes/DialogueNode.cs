using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor
{
    public class DialogueNode : Node
    {
        public string DialogueText;
        public Sprite face;
        public string GUID;
        public MoodType mood;
        public bool EntyPoint = false;
        
    }
}

