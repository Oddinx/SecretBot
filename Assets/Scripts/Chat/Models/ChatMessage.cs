using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChatMessage
{
    public int id;
    public string name;
    public string create_date;
    public bool is_name;
    public bool is_user;
    public string mes;
    public string send_date;
    public int chat_id;
}

[System.Serializable]
public class ChatMessageList
{
    public List<ChatMessage> messages;
}