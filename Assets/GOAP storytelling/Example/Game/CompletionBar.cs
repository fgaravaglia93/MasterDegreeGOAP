using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompletionBar : MonoBehaviour
{
    Slider completionTaskbarSlider;
    Vector3 offset = new Vector3(-10f, 1f, 0f);
    public bool barZero = false;
    private float count;
    private float currentDuration;
    private int step = 1;
    
    private void Start()
    {
        completionTaskbarSlider = this.GetComponentInChildren<Slider>();
        gameObject.GetComponent<Canvas>().enabled = false;
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
    }

    public IEnumerator CompleteTaskBar()
    {
        float stepBar = step / (currentDuration / step);
        //Debug.Log((int)(currentDuration / step));
        for(int i=0; i< ((int)(currentDuration/step)); i++)
        {
            completionTaskbarSlider.value += stepBar;
            //Debug.Log(i);
            yield return new WaitForSeconds(step);
        }

        completionTaskbarSlider.value = 0f;
    }
}
