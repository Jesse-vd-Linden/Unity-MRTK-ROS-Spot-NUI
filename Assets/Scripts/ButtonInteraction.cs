using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;


#if ENABLE_WINMD_SUPPORT && UNITY_WSA
using Windows.Media.Capture;
using Windows.Storage;
#endif

public class ButtonInteraction : MonoBehaviour
{
    private GameObject Canvas;
    public GameObject VoiceDebug;
    public TMP_Text LoggingPanel;
    public GameObject DataCollection;
    public GameObject VoiceCommand;
    public GameObject GestureCommand;

    private GameObject StartButton;
    private GameObject StartPanel;
    private GameObject VoiceSwitch;
    private GameObject TabletSwitch;
    private GameObject GestureSwitch;
    private GameObject Notification;

    //private bool IsVoiceExplained = false;
    //private bool IsGestureExplained = false;
    //private bool IsTabletExplained = false;

    // Start is called before the first frame update
    void Start()
    {
        Canvas = gameObject;
        StartButton = Canvas.transform.Find("Start").gameObject;
        StartPanel = Canvas.transform.Find("StartPanel").gameObject;
        VoiceSwitch = Canvas.transform.Find("VoiceSwitch").gameObject;
        GestureSwitch = Canvas.transform.Find("GestureSwitch").gameObject;
        TabletSwitch = Canvas.transform.Find("TabletSwitch").gameObject;
        Notification = Canvas.transform.Find("Notification").gameObject;
        VoiceCommand.SetActive(false);
        GestureCommand.SetActive(false);
        DataCollection.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleVoice()
    {
        GameObject SpeechInstructions = Canvas.transform.Find("SpeechInstructions").gameObject;
        Interactable voiceInteractable = VoiceSwitch.GetComponent<Interactable>();
        if (!VoiceCommand.activeSelf)
        {
            SpeechInstructions.SetActive(true);
            VoiceCommand.SetActive(true);
            voiceInteractable.IsToggled = true;

            Debug.Log("Voice on!");
        }
        else
        {
            SpeechInstructions.SetActive(false);
            VoiceCommand.SetActive(false);
            voiceInteractable.IsToggled = false;

            Debug.Log("Voice off!");
        }
    }

    public void ToggleGesture()
    {
        GameObject GestureInstructions = Canvas.transform.Find("GestureInstructions").gameObject;
        Interactable gestureInteractable = GestureSwitch.GetComponent<Interactable>();
        if (!GestureCommand.activeSelf)
        {
            GestureInstructions.SetActive(true);
            GestureCommand.SetActive(true);
            gestureInteractable.IsToggled = true;
            GestureCommand.GetComponent<RosPublisherHandKeypoints>().enabled = true;

            Debug.Log("Gesture on!");
        }
        else
        {
            GestureCommand.GetComponent<RosPublisherHandKeypoints>().enabled = false;
            GestureCommand.SetActive(false);
            GestureInstructions.SetActive(false);
            gestureInteractable.IsToggled = false;

            Debug.Log("Gesture off!");
        }
    }

    public void ToggleDebug()
    {
        GameObject InfoPanel = Canvas.transform.Find("InfoPanel").gameObject;
        if (!InfoPanel.activeSelf)
        {
            InfoPanel.SetActive(true);
            VoiceDebug.SetActive(true);
            Debug.Log("Debug on!");
        }
        else
        {
            InfoPanel.SetActive(false);
            VoiceDebug.SetActive(false);
            Debug.Log("Debug off!");
        }
    }

    public void ToggleTablet()
    {
        Interactable tabletInteractable = TabletSwitch.GetComponent<Interactable>();
        if (GestureCommand.activeSelf)
        {
            ToggleGesture();
        }
        if (VoiceCommand.activeSelf)
        {
            ToggleVoice();
        }
        Debug.Log("Tablet on!");
    }

    // public async void StartButton()
    public void OnStartButton()
    {
        StartCoroutine(HideAndShow(2.0f));
        if (!DataCollection.activeSelf)
        {
            DataCollection.SetActive(true);
        }
        StartPanel.SetActive(false);
        Debug.Log("Data collection on!");
        //try
        //{
        //    LoggingPanel.text = "try";
        //    await VideoRecordingAsync();
        //}
        //catch (Exception ex)
        //{
        //    LoggingPanel.text = "catch";
        //    Debug.LogException(ex);
        //    LoggingPanel.text = ex.ToString();
        //}
    }


    IEnumerator HideAndShow(float delay)
    {
        StartButton.SetActive(false);
        yield return new WaitForSeconds(delay);
        VoiceSwitch.SetActive(true);
        GestureSwitch.SetActive(true);
        TabletSwitch.SetActive(true);
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

    public async Task VideoRecordingAsync()
    {
#if ENABLE_WINMD_SUPPORT
        CameraCaptureUI captureUI = new CameraCaptureUI();
        LoggingPanel.text = "camera created";
        captureUI.VideoSettings.Format = CameraCaptureUIVideoFormat.Mp4;
        captureUI.VideoSettings.AllowTrimming = true;
        StorageFile videoFile = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Video);
        LoggingPanel.text = "camera started";

        if (videoFile == null)
        {
            // User cancelled photo capture
            LoggingPanel.text = "camera stopped";
            return;
        }
        StorageFolder destinationFolder =
            await ApplicationData.Current.LocalFolder.CreateFolderAsync("ProfileVideoFolder",
                CreationCollisionOption.OpenIfExists);

        await videoFile.CopyAsync(destinationFolder, "VideoCapture.mp4", NameCollisionOption.ReplaceExisting);
        await videoFile.DeleteAsync();
#endif
    }

}
