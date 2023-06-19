using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;


public static class CharacterData
{
    public static int selectedCharacterID;
}
public class CharacterManager : MonoBehaviour
{
   public string charactersEndpoint = "http://localhost:3000/getcharacters";
    public GameObject characterPrefab;
    public Transform gridContainer;

    void Start()
    {
        StartCoroutine(GetCharacters());
    }

    IEnumerator GetCharacters()
    {
        UnityWebRequest request = UnityWebRequest.Get(charactersEndpoint);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al obtener los personajes: " + request.error);
            yield break;
        }

        string responseJson = request.downloadHandler.text;
        JSONNode response = JSON.Parse(responseJson);

        List<Character> characters = new List<Character>();

        foreach (JSONNode characterJson in response.AsArray)
        {
            Character character = new Character();
            character.id = characterJson["id"];
            character.name = characterJson["name"];
            character.avatar = characterJson["avatar"];
            character.description = characterJson["description"];

            characters.Add(character);
        }
        

        CreateCharacterGrid(characters);
    }


    void CreateCharacterGrid(List<Character> characters)
    {
        foreach (Character character in characters)
        {
            GameObject characterObject = Instantiate(characterPrefab, gridContainer);
            characterObject.GetComponent<CharacterUI>().Initialize(character);
        }
    }
}
