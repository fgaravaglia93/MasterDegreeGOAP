using System;
using System.Linq;

namespace DialogueSystem.DataContainers
{
    [Serializable]
    public class NodeLinkData
    {
        public string BaseNodeGUID;
        public string PortName;
        public string TargetNodeGUID;
        public string changeMoodTo;
        public Trait trait;

        /*public void SetChangeToMood(string message)
        {
            if (message == "Joy")
                changeMoodTo = MoodType.Joy;
        }*/
    }
}