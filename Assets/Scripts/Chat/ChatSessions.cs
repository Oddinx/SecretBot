using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatSessions : MonoBehaviour
{
    // Start is called before the first frame update

   public static int ChatId { get; private set; }

   public static bool isFirstMessage { get; private set; }

   public static void SetChatId(int chatId)
    {
        ChatId= chatId;

    }

    public static void ResetChatId()
    {
        ChatId = 0;
    }
}
