using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    public Image avatarImage;
    public Text nameText;
    // public Text descriptionText;
    private int characterID;
    public void Initialize(Character character)
    {
        // Configurar los elementos de la UI con los datos del personaje
        StartCoroutine(LoadAvatarSprite(character.avatar));
        nameText.text = character.name;
        characterID = character.id;
        // descriptionText.text = character.description;
    }

        public int GetCharacterID()
    {
        return characterID; // Devuelve el ID del personaje
    }

private IEnumerator LoadAvatarSprite(string avatarUrl)
{
    UnityWebRequest request = UnityWebRequestTexture.GetTexture(avatarUrl);
    yield return request.SendWebRequest();

    if (request.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError("Error al cargar la imagen del avatar: " + request.error);
        yield break;
    }

    Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

    // Asignar el sprite del avatar a la Image de la UI
    avatarImage.sprite = sprite;
}
}
