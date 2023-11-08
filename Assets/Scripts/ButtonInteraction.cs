using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;


public class ButtonInteraction : MonoBehaviour
{
    private GameObject Canvas;
    public TMP_Text LoggingPanel;
    public GameObject VoiceCommand;
    public GameObject GestureCommand;
    private GameObject VoiceSwitch;
    private GameObject GestureSwitch;
    // Start is called before the first frame update
    void Start()
    {
        VoiceCommand.SetActive(false);
        GestureCommand.SetActive(false);
        Canvas = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShutDown()
    {
        LoggingPanel.text = "Click on Button";
        Debug.Log("Exit Pressed!");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    public void ToggleVoice()
    {
        if (!VoiceCommand.activeSelf)
        {
            VoiceCommand.SetActive(true);
            Debug.Log("Voice on!");
        }
        else
        {
            VoiceCommand.SetActive(false);
            Debug.Log("Voice off!");
        }
    }

    public void ToggleGesture()
    {
        if (!GestureCommand.activeSelf)
        {
            GestureCommand.SetActive(true);
            Debug.Log("Gesture on!");
        }
        else
        {
            GestureCommand.SetActive(false);
            Debug.Log("Gesture off!");
        }
    }

    public void ToggleTablet()
    {
        VoiceSwitch = Canvas.transform.Find("VoiceSwitch").gameObject;
        Interactable myInteractable = VoiceSwitch.GetComponent<Interactable>();
        myInteractable.IsToggled = false;
        GestureSwitch = Canvas.transform.Find("GestureSwitch").gameObject;
        myInteractable = GestureSwitch.GetComponent<Interactable>();
        myInteractable.IsToggled = false;
        VoiceCommand.SetActive(false);
        GestureCommand.SetActive(false);
        Debug.Log("Tablet on!");
    }

    public void Reset()
    {
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
