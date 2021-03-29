using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Moody5CreateAgent : EditorWindow
{
    // Start is called before the first frame update
    string objectName = ""; 
    float spawnRadius = 5f;
    bool isGoap = false;
    int openness;
    int consciousness;
    int extraversion;
    int agreeableness;
    int neuroticism;

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

        npcToSpawn.AddComponent(typeof(BigFivePersonality));
        npcToSpawn.GetComponent<BigFivePersonality>().openness = openness;
        npcToSpawn.GetComponent<BigFivePersonality>().consciousness = consciousness;
        npcToSpawn.GetComponent<BigFivePersonality>().extraversion = extraversion;
        npcToSpawn.GetComponent<BigFivePersonality>().agreeableness = agreeableness;
        npcToSpawn.GetComponent<BigFivePersonality>().neuroticism = neuroticism;

        if (isGoap)
        {
            npcToSpawn.AddComponent(typeof(PersonalityAgent));
        } else
        {
            npcToSpawn.AddComponent(typeof(PersonalityCommon));
        }
        npcToSpawn.AddComponent(typeof(OverlayAgentStatus));
        objectName = "NPC name";
        //go.AddComponent(typeof(Animation));
        // go.SetActiveRecursively(false);
        //AssetDatabase.CreateAsset(go, "Assets/Prefabs/TestAsset.prefab");
        //GameObject.DestroyImmediate(go);

    }

    
}
