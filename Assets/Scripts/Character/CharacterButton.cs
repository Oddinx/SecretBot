using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;





public class CharacterButton : MonoBehaviour,IPointerDownHandler
{
   public CharacterUI characterUI;
   
   public ChatManager chatManager;
    public void OnButtonClick(PointerEventData eventData)
    {
        int characterId = characterUI.GetCharacterID();
        CharacterData.selectedCharacterID = characterId; 
         
         StartCoroutine(chatManager.GetChatEndpoint(characterId));

        Debug.Log(characterId);
        // Guardar el ID del personaje seleccionado en una variable estática o en un objeto de persistencia de datos
        SceneManager.LoadScene("chatexample"); // Cargar la siguiente escena
    }


    public void  OnPointerDown(PointerEventData eventData)
    {
         int characterId = characterUI.GetCharacterID();
        CharacterData.selectedCharacterID = characterId; 

        StartCoroutine(LoadSceneAfterChatEndpoint(characterId));

    }

     private IEnumerator LoadSceneAfterChatEndpoint(int characterId)
    {
        yield return StartCoroutine(chatManager.GetChatEndpoint(characterId));

        Debug.Log(ChatSessions.ChatId);
        Debug.Log(characterId);
        Debug.Log(UserSession.UserId);

        SceneManager.LoadScene("chatexample");
    }
}
