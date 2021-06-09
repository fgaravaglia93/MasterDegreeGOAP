using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DialogueSystem.DataContainers;

namespace DialogueSystem.Runtime
{
    //Here we the change of mood tresholds during dialogues
    public class DialogueParser : MonoBehaviour
    {
        [HideInInspector]
        public static DialogueParser instance = null;

        [SerializeField]
        public DialogueContainer narrativeSequence;
        private DialogueContainer goalSequence;
        public TextMeshProUGUI dialogueText;

        public GameObject dialogueFace;
        public Button choicePrefab;
        public Transform buttonContainer;
        public DialogueController dialogueNPC;
        
        private NodeLinkData dialogueData;
        public bool dialogueOnGoing = false;
        private bool lastBlock = false;
        string answer;

        [HideInInspector]
        public bool flagNPC;
        [HideInInspector]
        public bool interactable;
        [HideInInspector]
        public bool flagOCEAN;
        [HideInInspector]
        public bool flagMood;
        [HideInInspector]
        public bool flagGOAP;
        bool goalAssigned = false;


        private Sprite face;

        //[HideInInspector]
        public bool storyEvent;

       void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

        }

        void Update()
        {
            if(interactable && !dialogueOnGoing)
            {

                if (Input.GetButtonDown("Enter") || storyEvent || goalAssigned)
                {
                    storyEvent = false;
                    //Debug.Log("interact");

                    if (!lastBlock)
                    {
                        //stop planning if isGOAP
                        if (dialogueNPC.GetComponent<Moody5Agent>() != null)
                        {
                            dialogueNPC.GetComponent<Moody5Agent>().interaction = true;
                        }

                        //look at the hero
                        if(dialogueNPC.GetComponent<MovementNPC>() != null)
                        {
                            dialogueNPC.GetComponent<MovementNPC>().firstInteract = true;
                            dialogueNPC.GetComponent<MovementNPC>().hero.GetComponent<Movement2D>().interact = true;
                        }

                        //disable elements on display
                        if (!DisplayManager.instance.displayGOAP.active)
                            flagGOAP = true;
                        if (!DisplayManager.instance.moodBar.transform.parent.gameObject.active)
                            flagMood = true;
                        if (!DisplayManager.instance.displayOCEAN.transform.parent.gameObject.active)
                            flagOCEAN = true;


                        DisplayManager.instance.displayBox.SetActive(false);
                        //disable info mode on NPC
                        DisplayManager.instance.interact = true;

                        dialogueData = narrativeSequence.NodeLinks.First();

                        dialogueOnGoing = true;
                        dialogueFace.transform.parent.gameObject.SetActive(true);

                        //Manage interaction with GOAP agent
                        if (dialogueNPC.GetComponent<Moody5Agent>() != null)
                        {
                            goalSequence = dialogueNPC.GetComponent<Moody5Agent>().goapInteraction;
                            StartDialogueGoal(goalSequence.NodeLinks.First().TargetNodeGUID, dialogueNPC.GetComponent<MoodController>().mood);
                        }
                        else
                        {
                            StartDialogue(dialogueData.TargetNodeGUID, dialogueNPC.GetComponent<MoodController>().mood);
                        }

                    }
                    else
                    {
                        goalAssigned = false;
                        dialogueFace.transform.parent.gameObject.SetActive(false);
                        lastBlock = false;
                        dialogueData = narrativeSequence.NodeLinks.First();
                        //return to planning if isGOAP
                        if (dialogueNPC.GetComponent<Moody5Agent>() != null)
                            dialogueNPC.GetComponent<Moody5Agent>().interaction = false;

                        //look up previous direction
                        if (dialogueNPC.GetComponent<MovementNPC>() != null)
                        {
                            dialogueNPC.GetComponent<MovementNPC>().hero.GetComponent<Movement2D>().interact = false;
                            dialogueNPC.GetComponent<MovementNPC>().backtoMove = true;
                        }

                        //enable elements on display
                        DisplayManager.instance.displayBox.SetActive(true);
                        if (flagGOAP)
                        {
                            DisplayManager.instance.displayGOAP.SetActive(false);
                            flagGOAP = false;
                        }
                        
                        if (flagMood)
                        {
                            DisplayManager.instance.moodBar.transform.parent.gameObject.SetActive(false);
                            DisplayManager.instance.displaySwitchMood.transform.parent.gameObject.SetActive(false);
                            DisplayManager.instance.displayTraitAdhoc.transform.parent.gameObject.SetActive(false);
                            flagMood = false;
                        }

                        if (flagOCEAN)
                        {
                            DisplayManager.instance.displayOCEAN.transform.parent.gameObject.SetActive(false);
                            flagOCEAN = false;
                        }

                        if (!goalAssigned)
                            dialogueOnGoing = false;

                        //enable info mode on NPC
                        DisplayManager.instance.interact = false;
                    }
                }
            }
        }

        public void StartDialogue(string dialogueDataGUID, MoodType changeTo)
        {
            var text = narrativeSequence.DialogueNodeData.Find(x => x.NodeGUID == dialogueDataGUID).DialogueText;
            var choices = narrativeSequence.NodeLinks.Where(x => x.BaseNodeGUID == dialogueDataGUID);
            //Manage NPC expression
            dialogueFace.GetComponent<Image>().sprite = dialogueNPC.face;
            var face = DisplayManager.instance.GetFace(dialogueNPC.face.name, (int)changeTo);
            dialogueText.text = text;
            dialogueFace.GetComponent<Image>().sprite = face;
            var buttons = buttonContainer.GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                Destroy(buttons[i].gameObject);
            }
            int nChoices = choices.Count();

            //If leaf node
            if (nChoices == 0)
            {
                dialogueOnGoing = false;
                lastBlock = true;
            }
                
            //This handle the trait option

            int j = 0;
            bool skip = false;
            Button button = null;
           
            foreach (NodeLinkData choice in choices)
            {
                //Debug.Log(choice.PortName);

                //se ho trait instanza bottone e dimmi di non istanziare il successivo
                if (!skip)
                {
                    if (choice.trait == null)
                    {
                        j++;
                        button = InstanciateButtonChoice(choice, nChoices, j);
                    }
                    else
                    {
                        //check trait, se ce'ho istanzialo
                        //Debug.Log("not trait");
                        flagNPC = CheckTrait();
                        if (flagNPC)
                        {
                            skip = true;
                            j++;
                            button = InstanciateButtonChoice(choice, nChoices, j);
                        }
                    }

                }
                else
                    skip = false;

                //this will manage the change of mood during interaction by talking
                if (button != null && choice.changeMoodTo != MoodType.Neutral)
                    button.onClick.AddListener(() => DisplayManager.instance.ChangeMood(dialogueNPC.gameObject, choice.changeMoodTo,10f));
            }
        }

        public void StartDialogueGoal(string dialogueDataGUID, MoodType changeTo)
        {
            var text = goalSequence.DialogueNodeData.Find(x => x.NodeGUID == dialogueDataGUID).DialogueText;
            var choices = goalSequence.NodeLinks.Where(x => x.BaseNodeGUID == dialogueDataGUID);
            //Manage NPC expression
            dialogueFace.GetComponent<Image>().sprite = dialogueNPC.face;
            var face = DisplayManager.instance.GetFace(dialogueNPC.face.name, (int)changeTo);
            dialogueText.text = text;
            dialogueFace.GetComponent<Image>().sprite = face;
            var buttons = buttonContainer.GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                Destroy(buttons[i].gameObject);
            }
            int nChoices = choices.Count();

            //This handle the trait option
            int j = 0;
            Button button = null;

            foreach (NodeLinkData choice in choices)
            { 
                j++;
                button = InstanciateButtonChoiceGoal(choice, nChoices, j);
            }
        }

        //Instantiate a node in the right position depending on the number of choices
        private Button InstanciateButtonChoice(NodeLinkData choice, int nChoices, int j)
        {   
            Button button = Instantiate(choicePrefab, buttonContainer);
            var rectTransform = button.GetComponent<RectTransform>();
            if (nChoices == 1)
                button.GetComponent<RectTransform>().anchoredPosition =
                new Vector3(buttonContainer.GetComponent<RectTransform>().anchoredPosition.x -150, buttonContainer.GetComponent<RectTransform>().anchoredPosition.y - 50f, 0f);
            else
                button.GetComponent<RectTransform>().anchoredPosition =
                new Vector3(buttonContainer.GetComponent<RectTransform>().anchoredPosition.x -150 + 150 * (j-1), buttonContainer.GetComponent<RectTransform>().anchoredPosition.x -50f, 0f);

            button.GetComponentInChildren<TextMeshProUGUI>().text = choice.PortName;
            button.onClick.AddListener(() => StartDialogue(choice.TargetNodeGUID,choice.changeMoodTo));
            return button;
        }

        //Used for GOAP interaction for new goal
        private Button InstanciateButtonChoiceGoal(NodeLinkData choice, int nChoices, int j)
        {
            Button button = Instantiate(choicePrefab, buttonContainer);
            var rectTransform = button.GetComponent<RectTransform>();
            if (nChoices == 1)
                button.GetComponent<RectTransform>().anchoredPosition =
                new Vector3(buttonContainer.GetComponent<RectTransform>().anchoredPosition.x - 150, buttonContainer.GetComponent<RectTransform>().anchoredPosition.y - 50f, 0f);
            else
                button.GetComponent<RectTransform>().anchoredPosition =
                new Vector3(buttonContainer.GetComponent<RectTransform>().anchoredPosition.x - 150 + 150 * (j - 1), buttonContainer.GetComponent<RectTransform>().anchoredPosition.x - 50f, 0f);

            button.GetComponentInChildren<TextMeshProUGUI>().text = choice.PortName;

            if (choice.PortName == "Talk")
                button.onClick.AddListener(() => StartDialogue(narrativeSequence.NodeLinks.First().TargetNodeGUID, choice.changeMoodTo));
            if (choice.PortName == "Assign goal")
            {
                lastBlock = true;
                dialogueOnGoing = false;
                button.onClick.AddListener(() => EndDialogue());

            }


            return button;
        }

        private bool CheckTrait()
        {
            return true;
        }
        
        private void EndDialogue()
        {
            goalAssigned = true;
        }

    }

}
