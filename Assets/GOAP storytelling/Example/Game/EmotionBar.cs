using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmotionBar : MonoBehaviour
{
    Slider emotionBarSlider;
    Vector3 offset = new Vector3(-10f, 1f, 0f);
    public bool barZero = false;
    private float count;
    private float currentDuration;
    private int step = 1;

    private void Start()
    {
        emotionBarSlider = this.GetComponentInChildren<Slider>();
        emotionBarSlider.value = 0f;
    }

    private void Update()
    {

    }

    public void StartTaskBar(float duration)
    {
        
        currentDuration = duration;//seconds
        StartCoroutine("CompleteTaskBar");
    }

    
}
