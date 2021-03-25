using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NPCPlugin : EditorWindow
{
    // Start is called before the first frame update
    string objectName = ""; 
    float spawnRadius = 5f;
    bool isGoap = false;
    //bool isWanderer = false;
    int openness;
    int consciousness;
    int extraversion;
    int agreeableness;
    int neuroticism;

    [MenuItem("Moody5/Create Personality Agent")]
    public static void ShowWindow()
    {
        GetWindow(typeof(NPCPlugin));
    }

    private void OnGUI()
    {
        GUILayout.Label("Spawn New Object", EditorStyles.boldLabel);
        objectName = EditorGUILayout.TextField("Name", objectName);
        GUILayout.Label("Big 5 personality model", EditorStyles.boldLabel);
        openness = (int)EditorGUILayout.Slider("Openness", openness, -1, 1);
        consciousness = (int)EditorGUILayout.Slider("Consciousness", consciousness, -1, 1);
        extraversion = (int)EditorGUILayout.Slider("Extraversion", extraversion, -1, 1);
        agreeableness = (int)EditorGUILayout.Slider("Agreeableness", agreeableness, -1, 1);
        neuroticism = (int)EditorGUILayout.Slider("Neuroticism", neuroticism, -1, 1);
        spawnRadius = EditorGUILayout.FloatField("Spawn Radius", spawnRadius);
        //isWanderer = EditorGUILayout.Toggle("Wanderer", isWanderer);
        isGoap = EditorGUILayout.Toggle("GOAP", isGoap);
        
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
        npcToSpawn.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/tile_npc_"+MoodType.Neutral);
        npcToSpawn.GetComponent<SpriteRenderer>().sortingOrder = 2;


        if (isGoap)
        {
            npcToSpawn.AddComponent(typeof(PersonalityAgent));
            npcToSpawn.GetComponent<PersonalityAgent>().openness = openness;
            npcToSpawn.GetComponent<PersonalityAgent>().consciousness = consciousness;
            npcToSpawn.GetComponent<PersonalityAgent>().extraversion = extraversion;
            npcToSpawn.GetComponent<PersonalityAgent>().agreeableness = agreeableness;
            npcToSpawn.GetComponent<PersonalityAgent>().neuroticism = neuroticism;
        } else
        {
            npcToSpawn.AddComponent(typeof(PersonalityCommon));
            npcToSpawn.GetComponent<PersonalityCommon>().openness = openness;
            npcToSpawn.GetComponent<PersonalityCommon>().consciousness = consciousness;
            npcToSpawn.GetComponent<PersonalityCommon>().extraversion = extraversion;
            npcToSpawn.GetComponent<PersonalityCommon>().agreeableness = agreeableness;
            npcToSpawn.GetComponent<PersonalityCommon>().neuroticism = neuroticism;
        }

        //go.AddComponent(typeof(Animation));
        // go.SetActiveRecursively(false);
        //AssetDatabase.CreateAsset(go, "Assets/Prefabs/TestAsset.prefab");
        //GameObject.DestroyImmediate(go);
    }

    
}
