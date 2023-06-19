using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class ChatManager : MonoBehaviour
{
    // Start is called before the first frame update


  

    public IEnumerator GetChatEndpoint(int characterId)
    {
    // Crear la URL del endpoint
    
    string url = "http://localhost:3000/getchat?token=" + UserSession.Token + "&characterId=" + characterId.ToString();

    // Crear la solicitud HTTP GET
    UnityWebRequest request = UnityWebRequest.Get(url);

    // Enviar la solicitud y esperar la respuesta
    yield return request.SendWebRequest();

    // Verificar si ocurrió algún error
    if (request.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError("Error al obtener el chat: " + request.error);
        yield break;
    }

    // Obtener la respuesta del servidor
    string responseJson = request.downloadHandler.text;

    // Convertir la respuesta a un objeto JSON
    JSONNode response = JSON.Parse(responseJson);

    // Verificar si ocurrió algún error en la respuesta
    if (response.HasKey("error"))
    {
        Debug.LogError("Error al obtener el chat: " + response["error"]);
    }
    else
    {
        // Obtener el ID del chat
       int ChatId = response["chatId"].AsInt;

       ChatSessions.SetChatId(ChatId);

       

        // Hacer algo con el ID del chat...
        Debug.Log("ID del chat: " + ChatSessions.ChatId );
    }
}

}
