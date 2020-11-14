using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private GraphicMover restartButton;
    [SerializeField] private ScreenFader tryAgainText;
    [SerializeField] private ScreenFader thanksText;
    [SerializeField] private ScreenFader restartButtonFader;
    
    void Start()
    {
        restartButton.Move();
        tryAgainText.FadeOn();
        thanksText.FadeOn();
        restartButtonFader.FadeOn();
    }
}
