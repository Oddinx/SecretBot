using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using NAudio.Wave;

using UnityEngine.UI;



public enum Chat {
    read_chat,
    automatic_chat
}
public class ScriptTest : MonoBehaviour
{
    public string ttsUrl = "http://2bc4-34-122-126-209.ngrok-free.app/ttsvv";
    public string ttsSpeaker = "YaeSakura";
    public string ttsLanguage = "日本語";
    public float ttsSpeed = 1.0f;

    public static Chat chat_type;

    public string input;
    // Start is called before the first frame update
      
      public void apitest(){
        input = "Hi";
        GradioTTS(input);
      }

[System.Serializable]
public class TTSRequest
{
    public string text;
    public string speaker;
    public string language;
    public float speed;
}
    public void GradioTTS(string message)
    {
         Dictionary<string, string> headers = new Dictionary<string, string>();
    headers.Add("Content-Type", "application/json");

    TTSRequest requestObject = new TTSRequest();
    requestObject.text = message;
    requestObject.speaker = ttsSpeaker;
    requestObject.language = ttsLanguage;
    requestObject.speed = ttsSpeed;

    string jsonBody = JsonUtility.ToJson(requestObject);
    Debug.Log("JSON Body: " + jsonBody);

    UnityWebRequest request = UnityWebRequest.Post(ttsUrl, jsonBody);
    foreach (KeyValuePair<string, string> header in headers)
    {
        request.SetRequestHeader(header.Key, header.Value);
    }
    request.SendWebRequest();

    while (!request.isDone)
    {
        // wait for request to finish
    }

    string audioFilePath = Application.persistentDataPath + "/audio_file.wav";
    System.IO.File.WriteAllBytes(audioFilePath, request.downloadHandler.data);
    StartCoroutine(PlayAudio(audioFilePath));
    }


      public void rstring( string s){
          
          input = s;
          ReadChat(input);
      }
    public void ReadChat(string message)
    {
        
           

            if (!string.IsNullOrEmpty(message))
            {
                string response = message; // here you can put any processing you want for the input message
                Debug.Log(response);
                GradioTTS(response);
            }
       
    }


    public void rtest(){
         ltest();
    }

    public void ltest (){
        htest();
    }

    public void htest(){

        Debug.Log("asd");
    }

    private IEnumerator PlayAudio(string audioFilePath)
    {
        using (var audioFile = new AudioFileReader(audioFilePath))
        using (var outputDevice = new WaveOutEvent())
        {
            outputDevice.Init(audioFile);
            outputDevice.Play();
            while (outputDevice.PlaybackState == PlaybackState.Playing)
            {
                yield return null;
            }
        }
    }
}
