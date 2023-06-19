using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CharacterSelection : MonoBehaviour
{
  private static int selectedCharacterId;

    public static int GetSelectedCharacterId()
    {
        return selectedCharacterId;
    }

    public static void SetSelectedCharacterId(int characterId)
    {
        selectedCharacterId = characterId;
    }
}
