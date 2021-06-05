using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem.DataContainers;
using DialogueSystem.Runtime;

public class DialogueController : MonoBehaviour
{
    public DialogueContainer dialogue;
 
    public bool interact;
   
    public  bool firstInteract;

    public Sprite face;

    void Update()
    {
        if(interact && firstInteract)
        {
            firstInteract = false;
            DialogueParser.instance.narrativeSequence = dialogue;
            DialogueParser.instance.dialogueNPC = this;
            DialogueParser.instance.interactable = true;
        }

    }
}
