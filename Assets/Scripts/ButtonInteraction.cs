using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

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

    private GameObject Questionaire;
    private GameObject StartPanel;
    private GameObject VoiceSwitch;
    private GameObject TabletSwitch;
    private GameObject GestureSwitch;

    // Start is called before the first frame update
    void Start()
    {
        Canvas = this.gameObject;
        Questionaire = Canvas.transform.Find("Questionaire").gameObject;
        StartPanel = Canvas.transform.Find("StartPanel").gameObject;
        VoiceCommand.SetActive(false);
        GestureCommand.SetActive(false);
        DataCollection.SetActive(false);
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
        GameObject SpeechInstructions = Canvas.transform.Find("SpeechInstructions").gameObject;
        VoiceSwitch = Canvas.transform.Find("VoiceSwitch").gameObject;
        Interactable myInteractable = VoiceSwitch.GetComponent<Interactable>();
        if (!VoiceCommand.activeSelf)
        {
            SpeechInstructions.SetActive(true);
            VoiceCommand.SetActive(true);
            myInteractable.IsToggled = true;

            Questionaire.SetActive(false);
            Debug.Log("Voice on!");
        }
        else
        {
            SpeechInstructions.SetActive(false);
            VoiceCommand.SetActive(false);
            myInteractable.IsToggled = false;

            Questionaire.SetActive(true);
            Debug.Log("Voice off!");
        }
    }

    public void ToggleGesture()
    {
        GameObject GestureInstructions = Canvas.transform.Find("GestureInstructions").gameObject;
        GestureSwitch = Canvas.transform.Find("GestureSwitch").gameObject;
        Interactable myInteractable = GestureSwitch.GetComponent<Interactable>();
        if (!GestureCommand.activeSelf)
        {
            GestureInstructions.SetActive(true);
            GestureCommand.SetActive(true);
            myInteractable.IsToggled = true;
            GestureCommand.GetComponent<RosPublisherHandKeypoints>().enabled = true;

            Questionaire.SetActive(false);
            Debug.Log("Gesture on!");
        }
        else
        {
            GestureCommand.GetComponent<RosPublisherHandKeypoints>().enabled = false;
            GestureCommand.SetActive(false);
            GestureInstructions.SetActive(false);
            myInteractable.IsToggled = false;

            Questionaire.SetActive(true);
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
            Questionaire.SetActive(false);
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
        TabletSwitch = Canvas.transform.Find("TabletSwitch").gameObject;
        Interactable myInteractable = TabletSwitch.GetComponent<Interactable>();
        if (GestureCommand.activeSelf)
        {
            ToggleGesture();
        }
        if (VoiceCommand.activeSelf)
        {
            ToggleVoice();
        }
        if (myInteractable.IsToggled == false)
        {
            Questionaire.SetActive(true);
        }
        else
        {
            Questionaire.SetActive(false);
        }
        Debug.Log("Tablet on!");
    }

    // public async void StartButton()
    public void StartButton()
    {
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

    public void Reset()
    {
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
