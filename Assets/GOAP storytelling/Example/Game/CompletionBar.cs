using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompletionBar : MonoBehaviour
{
    public bool barZero = false;
    Slider completionTaskbarSlider;
    Vector3 offset = new Vector3(-10f, 1f, 0f);
    System.TimeSpan timePlaying;

    float elapsed;
    float currentDuration;
    int step = 1;
    float startTime = 0f;
    
    Text timeText;
    
    private void Start()
    {
        timeText = GetComponentInChildren<Text>();
        completionTaskbarSlider = this.GetComponentInChildren<Slider>();
        completionTaskbarSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        //completionTaskbarSlider.gameObject.transform.position = DisplayController.instance.GameCamera.WorldToScreenPoint(transform.parent.position)+ offset;
    }

    public void StartTaskBar(float duration)
    {
        completionTaskbarSlider.value = 0f;
        currentDuration = duration;//seconds
        StartCoroutine("CompleteTaskBar");
        elapsed = 0f;
        startTime = 0;
        timeText.text = "00.00";
        StartCoroutine("StartCronometer");

    }

    public IEnumerator CompleteTaskBar()
    {
        float stepBar = step / (currentDuration / step);
        //Debug.Log((int)(currentDuration / step));
        for (int i = 0; i < ((int)(currentDuration / step)); i++)
        {
            completionTaskbarSlider.value += stepBar;
            //Debug.Log(i);
            yield return new WaitForSeconds(step);
        }

        completionTaskbarSlider.value = 0f;
    }

    public IEnumerator StartCronometer()
    {
        do
        {
            elapsed += Time.deltaTime;
            timePlaying = System.TimeSpan.FromSeconds(elapsed);
            var timeToDisplay = "" + timePlaying.ToString("ss':.'ff");
            timeText.text = timeToDisplay;
            yield return null;

        } while (elapsed < currentDuration);
        
    }
}