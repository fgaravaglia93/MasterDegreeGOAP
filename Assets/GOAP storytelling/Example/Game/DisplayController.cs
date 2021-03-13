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

    private float cooldownTime = 5f;
    private int countClicks;

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
        countClicks = 0;
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
        countClicks += 1;
        print(npc.GetComponent<PersonalityAgent>().CalculateSwitchEmotionFactor(2));

        if (countClicks >= npc.GetComponent<PersonalityAgent>().CalculateSwitchEmotionFactor(2))
        {
            npc.GetComponent<PersonalityAgent>().mood = Mood.Joy;
            npc.GetComponent<HogwartsStudent>().durationActionInfluence = 0.5f;
            npc.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/tile_npc_joy");
            cooldownTime = 5f;
            StartCoroutine("CooldownEmotion");
            countClicks = 0;
        }

    }

    public void ChangeMoodToSad()
    {
        npc.GetComponent<PersonalityAgent>().mood = Mood.Sad;
        npc.GetComponent<HogwartsStudent>().durationActionInfluence = 2f;
        npc.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/tile_npc_sad");
        cooldownTime = 5f;
        StartCoroutine("CooldownEmotion");
    }

    public void ChangeMoodToFear()
    {
        npc.GetComponent<PersonalityAgent>().mood = Mood.Fear;
        npc.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/tile_npc_fear");
        cooldownTime = 2f;
        StartCoroutine("CooldownEmotion");
    }

    public void ChangeMoodToAngry()
    {
        npc.GetComponent<PersonalityAgent>().mood = Mood.Angry;
        npc.GetComponent<HogwartsStudent>().durationActionInfluence = 0.5f;
        npc.GetComponent<HogwartsStudent>().successActionInfluence = 0.5f;
        npc.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/tile_npc_angry");
        cooldownTime = 5f;
        StartCoroutine("CooldownEmotion");
    }

    IEnumerator CooldownEmotion()
    {
        yield return new WaitForSeconds(cooldownTime);
        npc.GetComponent<PersonalityAgent>().mood = Mood.Neutral;
        npc.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/tile_npc");

    }
}
