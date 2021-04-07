using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueSystem.Editor
{
    public class MoodPort : GraphElement
    {
        //used for detect mood to change in the next dialog
        public MoodType changeMoodTo;
        public Port port;

        public MoodPort(Port port, MoodType mood)
        {
            this.port = port;
            changeMoodTo = mood;
        }

    }
}