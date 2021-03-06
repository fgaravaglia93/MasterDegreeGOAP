using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayController : MonoBehaviour
{
    public static DisplayController instance = null;

    public Camera GameCamera;
    public GameObject SlotCamera;
    public GameObject DisplayConsoleText;
    RenderTexture RenderGameScene;


    public float SmallCameraSize = 4f;

    //to redefine later
    public GameObject npc;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            RenderGameScene = new RenderTexture(350, 300, 24, RenderTextureFormat.ARGB32);
            RenderGameScene.Create();
            GameCamera.GetComponent<Camera>().targetTexture = RenderGameScene;
            RenderGameScene.name = "RT_Game";

        }
        else
        {
            Destroy(gameObject);
        }



    }

    void Start()
    {
        SlotCamera.GetComponent<RawImage>().texture = RenderGameScene;
        GameCamera.orthographicSize = SmallCameraSize;
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void ShowOnConsole(string text)
    {
        DisplayConsoleText.GetComponent<Text>().text = text;
    }

    public void ShowOnConsole(string text, Color color)
    {
        DisplayConsoleText.GetComponent<Text>().text = text;
        DisplayConsoleText.GetComponent<Text>().color = color;
    }

    public void ChangeMoodToJoy()
    {
        npc.GetComponent<Person>().durationChange = 0.5f;
        npc.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/tile_npc_joy");

    }

    public void ChangeMoodToSad()
    {
        npc.GetComponent<Person>().durationChange = 2f;
        npc.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/tile_npc_sad");
    }

    public void ChangeMoodToFear()
    {
        npc.GetComponent<Person>().durationChange = 2f;
        npc.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/tile_npc_fear");
        StartCoroutine("CooldownFear");
    }

    IEnumerator CooldownFear()
    {

        yield return new WaitForSeconds(2f);
        npc.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/tile_npc");

    }
}
