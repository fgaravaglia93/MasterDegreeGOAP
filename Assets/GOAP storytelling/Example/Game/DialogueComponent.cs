using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem.DataContainers;
using DialogueSystem.Runtime;

public class DialogueComponent : MonoBehaviour
{
    public DialogueContainer narrativeSequence;
 
    public bool interact;
   
    public  bool firstInteract;

    void Update()
    {
        if(interact && firstInteract)
        {
            firstInteract = false;
            DialogueParser.instance.narrativeSequence = narrativeSequence;
            DialogueParser.instance.dialogueNPC = this;
            DialogueParser.instance.interactable = true;
        }

    }
}
