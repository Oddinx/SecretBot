using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserSession : MonoBehaviour
{
   public static bool IsLoggedIn { get; private set; }
    public static string Username { get; private set; }
    public static string Token { get; private set; }

    public static int UserId { get; private set; }

    public static void Login(string username, string token, int userId)
    {
        IsLoggedIn = true;
        Username = username;
        Token = token;
        UserId = userId;

    }

    public static void Logout()
    {
        IsLoggedIn = false;
        Username = null;
        Token = null;

        UserId = 0;
    }
}
