using UnityEditor;
using UnityEngine;

public class BasicObjectSpawner : EditorWindow
{
    string objectBaseName = "";
    int objectID;
    GameObject objectToSpawn;
    float objectScale;
    float spawnRadius = 5f;
    // Start is called before the first frame update
    //[MenuItem("Moody5/Basic Object Spawner")]
    public static void ShowWindow()
    {
        GetWindow(typeof(BasicObjectSpawner));
    }

    private void OnGUI()
    {
        GUILayout.Label("Spawn New Object", EditorStyles.boldLabel);
        objectBaseName = EditorGUILayout.TextField("Base Name", objectBaseName);
        objectID = EditorGUILayout.IntField("Object ID", objectID);
        objectScale = EditorGUILayout.Slider("Object Scale", objectScale, 0.5f, 3f);
        spawnRadius = EditorGUILayout.FloatField("Spawn Radius", spawnRadius);
        objectToSpawn = EditorGUILayout.ObjectField("Prefab to Spawn", objectToSpawn, typeof(GameObject), false) as GameObject;

        if (GUILayout.Button("Spawn Object"))
        {
            SpawnObject();
        }
    }

    private void SpawnObject()
    {
        if(objectToSpawn == null)
        {
            Debug.LogError("Error: Please assign an object to be spawned");
            return;
        }
        if(objectBaseName == string.Empty)
        {
            Debug.LogError("Error: Please enter a base name for the object");
            return;
        }

        Vector2 spawnCircle = Random.insideUnitCircle * spawnRadius;
        Vector2 spawnPos = new Vector2(spawnCircle.x, spawnCircle.y);

        GameObject newObject = Instantiate(objectToSpawn, spawnPos, Quaternion.identity);
        newObject.name = objectBaseName + objectID;
        newObject.transform.localScale = Vector3.one * objectScale;

        objectID++;
    }

}
