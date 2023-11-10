using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;

#if ENABLE_WINMD_SUPPORT && UNITY_WSA
using Microsoft.Windows.Media.Capture;
using Microsoft.Windows.Storage;
#endif

public class ButtonInteraction : MonoBehaviour
{
    private GameObject Canvas;
    public TMP_Text LoggingPanel;
    public GameObject DataCollection;
    public GameObject VoiceCommand;
    public GameObject GestureCommand;
    private GameObject VoiceSwitch;
    private GameObject GestureSwitch;
    // Start is called before the first frame update
    void Start()
    {
        VoiceCommand.SetActive(false);
        GestureCommand.SetActive(false);
        DataCollection.SetActive(false);
        Canvas = this.gameObject;
        #if ENABLE_WINMD_SUPPORT
        Debug.Log("Windows Runtime Support enabled");
        // Put calls to your custom .winmd API here
        #endif
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
            GestureCommand.GetComponent<RosPublisherHandKeypoints>().enabled = true;
            Debug.Log("Gesture on!");
        }
        else
        {
            GestureCommand.GetComponent<RosPublisherHandKeypoints>().enabled = false;
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
    public void StartButton()
    {
        DataCollection.SetActive(true);
        Debug.Log("Data collection on!");
    }

    public void VideoRecording()
    {
        #if ENABLE_WINMD_SUPPORT
        CameraCaptureUI captureUI = new CameraCaptureUI();
        captureUI.VideoSettings.Format = CameraCaptureUIVideoFormat.Mp4;
        StorageFile videoFile = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Video);

        // StorageFolder destinationFolder = 
        //     await ApplicationData.Current.LocalFolder.CreateFolderAsync("ProfilePhotoFolder", 
        //         CreationCollisionOption.OpenIfExists);

        // await videoFile.CopyAsync(destinationFolder, "ProfilePhoto.jpg", NameCollisionOption.ReplaceExisting);
        // await videoFile.DeleteAsync();

        if (videoFile == null)
        {
            // User cancelled photo capture
            return;
        }
        #endif
    }

    public void Reset()
    {
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}