﻿using System;
using System.Linq;

namespace DialogueSystem.DataContainers
{
    [Serializable]
    public class NodeLinkData
    {
        public string BaseNodeGUID;
        public string PortName;
        public string TargetNodeGUID;
        public MoodType changeMoodTo;
        public Trait trait;

        
    }
}