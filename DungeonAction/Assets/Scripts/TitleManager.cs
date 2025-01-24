using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _modeSelectCanvas;

    void Start()
    {
        _modeSelectCanvas.SetActive(false);
    }

    void Update()
    {

    }

    public void ViewModeSelectCanvas()
    {
        _modeSelectCanvas.SetActive(true);
    }

    public void HideModeSelectCanvas()
    {
        _modeSelectCanvas.SetActive(false);
    }
}