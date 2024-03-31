using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MathSumManager : MonoBehaviour
{
    [Header("Gameplay Options")]
    [SerializeField] private int minNum;
    [SerializeField] private int maxNum;

    private int _sum = 0;

    [SerializeField] private int minRandomNum;
    [SerializeField] private int maxRandomNum;
    
    [SerializeField] private float timeToNextNumber;
    [SerializeField] private float timeToReplay = 3f;

    [Header("UI Options")]
    [SerializeField] private TMP_Text numberText;
    [SerializeField] private TMP_InputField answerInputField;
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    private WaitForSeconds _waitForSeconds;

    [Header("Events Options")]
    public UnityEvent onStartGenerate = new UnityEvent();
    public UnityEvent<int> onNumberGenerated = new UnityEvent<int>();
    public UnityEvent onFinishGenerate = new UnityEvent();

    private void Awake()
    {
        _waitForSeconds = new WaitForSeconds(timeToNextNumber);
    }

    public void StartGeneration()
    {
        Reset();
        
        onStartGenerate?.Invoke();
        
        StartCoroutine(GenerateNumbers());
    }
    
    private IEnumerator GenerateNumbers()
    {
        var randomTimes = Random.Range(minRandomNum, maxRandomNum + 1);

        for (var i = 0; i < randomTimes; i++)
        {
            yield return StartCoroutine(GenerateNumber());
        }
        
        onFinishGenerate?.Invoke();
    }

    private IEnumerator GenerateNumber()
    {
        var generateNumber = Random.Range(minNum, maxNum + 1);

        _sum += generateNumber;

         UpdateText(generateNumber.ToString());
            
        onNumberGenerated?.Invoke(generateNumber);

        yield return _waitForSeconds;
    }

    public void UpdateText(string text)
    {
        numberText.text = text;
    }

    public void ValidateAnswer()
    {
        if (int.TryParse(answerInputField.text, out var value))
        {
            numberText.text = _sum.ToString();

            numberText.color = value == _sum ? Color.green : Color.red;

            Invoke(nameof(Reset), timeToReplay);
        }
        else
        {
            answerInputField.text = "Entrada Inválida!";
        }
    }

    private void Reset()
    {
        UpdateText(string.Empty);
        
        numberText.color = Color.white;
        _sum = 0;

        answerInputField.text = string.Empty;
        answerInputField.gameObject.SetActive(false);
        
        startButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
