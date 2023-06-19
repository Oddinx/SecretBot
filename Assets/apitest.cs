using System.Collections;

using UnityEngine;
using UnityEngine.Networking;

using System;
using System.Text;
using OpenAI;

[Serializable]
public class tts
{
    public string text;
    public string speaker;
    public string language;
    public float speed;
}

public class apitest : MonoBehaviour
{
    // Start is called before the first frame update
    private string currentInput = "";


    public void TaskOnClick()
    {
        var s = "こんにちわ";
        StartCoroutine(ttsender(s));
    }

    public void readinput(string s){
     
       currentInput = s;

       Debug.Log(currentInput.ToString());
      StartCoroutine(ttsender(currentInput));
        
    }
    
    private async void SendRequest()
{
    var openai = new OpenAIApi("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");
    var request = new CreateCompletionRequest{
        Model="text-davinci-003",
        Prompt="This is how a toxic entitled evil female streamer responded in a conversation. She would respond in a toxic and rude manner. She would talk about the message and would elaborate on it as well as share some of her experiences if possible. She would also go on a tangent if possible.She can understand english but just answer in Japanese",
        MaxTokens=128,
        Temperature= 0.9f,
        PresencePenalty=1,
        FrequencyPenalty= 1,

    };
    var response = await openai.CreateCompletion(request);
}


    public IEnumerator ttsender(string s)
    {
        // Datos que se enviarán a la API
         tts mytss = new tts();

         mytss.text = s;

         mytss.speaker = "YaeSakura";

        mytss.language = "日本語";

        mytss.speed = 1f;

        string json = JsonUtility.ToJson(mytss);

        UnityWebRequest request = UnityWebRequest.Post("http://c1a4-34-87-66-16.ngrok-free.app/ttsvv", "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");




        // Verificar si hubo algún error en la respuesta
         yield return request.SendWebRequest();

        // Maneja la respuesta
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            // Obtiene los datos de la respuesta
            string filePath = request.GetResponseHeader("Content-Disposition");
            filePath = filePath.Replace("attachment; filename=", "");
            byte[] audioData = request.downloadHandler.data;

            // Usa los datos de la respuesta para crear el audio en Unity
            AudioClip audioClip = WavUtility.ToAudioClip(audioData);
            AudioSource.PlayClipAtPoint(audioClip, Vector3.zero);
        }
    }

  

  
}
