using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Campagn : MonoBehaviour
{
    public ColorLevel colorLevel;
    public GameObject dialogueUI;
    public TextMeshProUGUI dialogueTMP;

    private string greenStr = "Enigme ?";
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void greenLevel()
    {
        dialogueTMP.text = greenStr;
        dialogueUI.SetActive(true);
        /*GameObject inputField = new GameObject("answerTextFiled");
        InputField.transform.parent = dialogueUI.transform;
        inputField.AddComponent<InputField>();
        inputField.AddComponent<Image>();
        inputField.GetComponent<InputField>().targetGraphic = inputField.GetComponent<Image>();*/
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {            
            switch(colorLevel)
            {
                case ColorLevel.Vert:
                    greenLevel();
                break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            dialogueUI.SetActive(false);
            dialogueTMP.text = "New Text";
        }
    }
}

public enum ColorLevel {
    Rouge,
    Orange,
    Jaune,
    Vert, 
    Bleu, 
    Violet
};
