using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    [SerializeField] private KartController kartController;
    [SerializeField] private TMP_Text speedText;
    
    private void Update()
    {
        speedText.text = kartController.CurrentSpeed.ToString("0");
    }
}
