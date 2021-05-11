using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DialogueSystem.Runtime;
using DialogueSystem.DataContainers;
using UnityEngine.UI;
using TMPro;

public class Moody5CreateAgent : EditorWindow
{
    // Start is called before the first frame update
    string objectName = ""; 
    float spawnRadius = 5f;
    bool isGOAP = false;
    bool isDialogue = false;
    bool adHocTraits = false;
    int openness;
    int consciousness;
    int extraversion;
    int agreeableness;
    int neuroticism;
    GameObject interactV;
    GameObject interactH;
    Sprite sprite;
    //add something for the movement


    [MenuItem("Moody5/Create Personality Agent")]
    public static void ShowWindow()
    {
        GetWindow(typeof(Moody5CreateAgent), true, "Moody5");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create new agent with personality", EditorStyles.boldLabel);
        if (objectName == "")
            objectName = "NPC name";
        objectName = EditorGUILayout.TextField("Name", objectName);

        sprite = EditorGUILayout.ObjectField("Sprite",sprite, typeof(Sprite)) as Sprite;
        GUILayout.Space(10);
        GUILayout.Label("Big 5 personality model", EditorStyles.boldLabel);
        openness = (int)EditorGUILayout.Slider("Openness", openness, -1, 1);
        consciousness = (int)EditorGUILayout.Slider("Consciousness", consciousness, -1, 1);
        extraversion = (int)EditorGUILayout.Slider("Extraversion", extraversion, -1, 1);
        agreeableness = (int)EditorGUILayout.Slider("Agreeableness", agreeableness, -1, 1);
        neuroticism = (int)EditorGUILayout.Slider("Neuroticism", neuroticism, -1, 1);
        spawnRadius = EditorGUILayout.FloatField("Spawn Radius", spawnRadius);
        //isWanderer = EditorGUILayout.Toggle("Wanderer", isWanderer);
        GUILayout.Space(10);
        isGOAP = EditorGUILayout.Toggle("GOAP", isGOAP);

        /* if(isGOAP)
             GUILayout.Label("Define parameter needed for GOAP", EditorStyles.boldLabel);*/
        GUILayout.Space(10);
        GUILayout.Label("Add existing traits to the Agent", EditorStyles.boldLabel);
        isDialogue = EditorGUILayout.Toggle("Add Dialogue System", isDialogue);
        if(isDialogue)
        {
            interactV = EditorGUILayout.ObjectField("Interact Vertical", Resources.Load("Prefab/InteractVertical"), typeof(GameObject), false) as GameObject;
            interactH = EditorGUILayout.ObjectField("Interact Horizontal", Resources.Load("Prefab/InteractHorizontal"),  typeof(GameObject), false) as GameObject;
        }

        /*GUILayout.Space(10);
        GUILayout.Label("Add existing traits to the Agent", EditorStyles.boldLabel);
        adHocTraits = EditorGUILayout.Toggle("Ad hoc traits", adHocTraits);
        if (adHocTraits)
        {

        }*/

        if (GUILayout.Button("Create NPC"))
        {
            CreateNPC();
        }

    }

    private void CreateNPC()
    {

        if (objectName == string.Empty)
        {
            Debug.LogError("Error: Please enter a base name for the object");
            return;
        }

        Vector2 spawnCircle = Random.insideUnitCircle * spawnRadius;
        Vector2 spawnPos = new Vector2(spawnCircle.x, spawnCircle.y);
        GameObject npcToSpawn = new GameObject(objectName);
        npcToSpawn.transform.position = spawnPos;
        npcToSpawn.AddComponent(typeof(SpriteRenderer));
        npcToSpawn.AddComponent(typeof(Animator));
        npcToSpawn.AddComponent(typeof(ReskinAnimator));

        npcToSpawn.GetComponent<SpriteRenderer>().sprite = sprite;
        npcToSpawn.GetComponent<SpriteRenderer>().sortingOrder = 2;
        npcToSpawn.tag = "NPC";
        npcToSpawn.AddComponent(typeof(BigFivePersonality));
        npcToSpawn.GetComponent<BigFivePersonality>().openness = openness;
        npcToSpawn.GetComponent<BigFivePersonality>().consciousness = consciousness;
        npcToSpawn.GetComponent<BigFivePersonality>().extraversion = extraversion;
        npcToSpawn.GetComponent<BigFivePersonality>().agreeableness = agreeableness;
        npcToSpawn.GetComponent<BigFivePersonality>().neuroticism = neuroticism;
        npcToSpawn.AddComponent(typeof(Rigidbody2D));
        npcToSpawn.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        npcToSpawn.GetComponent<Rigidbody2D>().freezeRotation = true;
        npcToSpawn.AddComponent(typeof(BoxCollider2D));
        npcToSpawn.GetComponent<BoxCollider2D>().size = new Vector2(0.5f,0.5f);
        


        var camera = (GameObject)Instantiate(Resources.Load("Prefab/NPCCamera"));
        camera.transform.parent = npcToSpawn.transform;
        camera.name = "Camera";
        camera.transform.localPosition = new Vector3(0f,0f,-10f);
        if (isGOAP)
        {
            npcToSpawn.AddComponent(typeof(PersonalityAgent));
            npcToSpawn.AddComponent(typeof(MovementNPC));

        }
        else
        {
            npcToSpawn.AddComponent(typeof(PersonalityCommon));
            npcToSpawn.GetComponent<PersonalityCommon>().m_personality = new Personality();
            npcToSpawn.GetComponent<PersonalityCommon>().m_personality.isGOAP = false;
        }

        GameObject CanvasNPC = Instantiate(Resources.Load("Prefab/Canvas"), Vector3.zero, Quaternion.identity) as GameObject;
        CanvasNPC.transform.parent = npcToSpawn.transform;

        if (isDialogue)
        {
            //Add Dialogue System Script to the NPC and set default value on the inspector
            npcToSpawn.AddComponent(typeof(DialogueParser));
            var dialoguePrefab = GameObject.FindGameObjectWithTag("DialogueContainer");
            if(dialoguePrefab == null)
                dialoguePrefab = (GameObject)Instantiate(Resources.Load("Prefab/DialogueContainer"));
            npcToSpawn.GetComponent<DialogueParser>().dialogueText = dialoguePrefab.gameObject.transform.GetChild(0).transform.GetComponent<TextMeshProUGUI>();
            npcToSpawn.GetComponent<DialogueParser>().buttonContainer = dialoguePrefab.gameObject.transform.GetChild(1);
            npcToSpawn.GetComponent<DialogueParser>().dialogueFace = dialoguePrefab.gameObject.transform.GetChild(2).gameObject;
            npcToSpawn.GetComponent<DialogueParser>().choicePrefab = dialoguePrefab.GetComponentInChildren<Button>();

            //add here variables on inspector for the dialogue Parser - Dialogue Container / Face e button prefab
            GameObject InteractVertical = Instantiate(interactV, Vector3.zero, Quaternion.identity);
            GameObject InteractHorizontal = Instantiate(interactH, Vector3.zero, Quaternion.identity);

            InteractVertical.transform.parent = npcToSpawn.transform;
            InteractHorizontal.transform.parent = npcToSpawn.transform;

        }

        GameObject ParticleSystem = Instantiate(Resources.Load("Prefab/ParticleSystem"), Vector3.zero, Quaternion.identity) as GameObject;
        ParticleSystem.transform.parent = npcToSpawn.transform;

        objectName = "NPC name";
        
        //go.AddComponent(typeof(Animation));
        // go.SetActiveRecursively(false);
        //AssetDatabase.CreateAsset(go, "Assets/Prefabs/TestAsset.prefab");
        //GameObject.DestroyImmediate(go);

    }

    
}
