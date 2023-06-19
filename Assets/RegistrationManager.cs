using System.Collections;
using System.Collections.Generic;
using System.Text;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using UnityEngine.SceneManagement;
public class RegistrationManager : MonoBehaviour
{
   public string registrationEndpoint = "http://localhost:3000/register";

   public string loginEndpoint = "http://localhost:3000/login";

    [Header("Register")]
    public InputField usernameInput;
    public InputField emailInput;
    public InputField passwordInput;
    // public Button registerButton;


    [Header("Login")]
    public InputField usernameLogin;
    public InputField passwordLogin;
    // public Button loginButton;

 
    public void RegisterUser()
    {
         string username = usernameInput.text;
         string email = emailInput.text;
         string password = passwordInput.text;
        StartCoroutine(SendRegistrationRequest( username, email, password));
    }

    public IEnumerator SendRegistrationRequest( string username, string email, string password)
    {
        // Crear un objeto JSON con los datos del usuario
        JSONNode json = new JSONObject();
        json["username"] = username;
        json["email"] = email;
        json["password"] = password;

        // Convertir el objeto JSON a una cadena de texto
        string jsonBody = json.ToString();

        byte[] bodyBytes = Encoding.UTF8.GetBytes(jsonBody);

        // Crear la solicitud HTTP POST
            UnityWebRequest request = new UnityWebRequest(registrationEndpoint, "POST");
            request.uploadHandler = new UploadHandlerRaw(bodyBytes);
            request.downloadHandler = new DownloadHandlerBuffer();

        // Establecer la cabecera Content-Type como application/json
        request.SetRequestHeader("Content-Type", "application/json");

        // Enviar la solicitud y esperar la respuesta
        yield return request.SendWebRequest();

        // Verificar si ocurrió algún error
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al enviar la solicitud de registro: " + request.error + request.downloadHandler.text);
            yield break;
        }

        if (request.responseCode == 409)
        {
            Debug.LogError("Error al registrar usuario: El usuario ya existe" + request.downloadHandler.text);
            yield break;
        }
        // Obtener la respuesta del servidor
        string responseJson = request.downloadHandler.text;

        // Convertir la respuesta a un objeto JSON
        JSONNode response = JSON.Parse(responseJson);

        // Verificar si el registro fue exitoso
        if (response.HasKey("error"))
        {
            Debug.LogError("Error al registrar usuario: " + response["error"]);
        }
        else
        {
            Debug.Log("Usuario registrado exitosamente!");

            RegistrationUI.instance.LoginScreen();
        }
    }

     public void LoginUser()
    {
       string username = usernameLogin.text;

       string password = passwordLogin.text;
        StartCoroutine(SendLoginRequest(username, password));
    }

    public IEnumerator SendLoginRequest(string username, string password)
    {
        // Crear un objeto JSON con los datos de inicio de sesión
        JSONNode json = new JSONObject();
        json["username"] = username;
        json["password"] = password;

        // Convertir el objeto JSON a una cadena de texto
        string jsonBody = json.ToString();

        byte[] bodyBytes = Encoding.UTF8.GetBytes(jsonBody);

        // Crear la solicitud HTTP POST
        UnityWebRequest request = new UnityWebRequest(loginEndpoint, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyBytes);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Establecer la cabecera Content-Type como application/json
        request.SetRequestHeader("Content-Type", "application/json");

        // Enviar la solicitud y esperar la respuesta
        yield return request.SendWebRequest();

        // Verificar si ocurrió algún error
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al enviar la solicitud de inicio de sesión: " + request.error);
            yield break;
        }

        // Obtener la respuesta del servidor
        string responseJson = request.downloadHandler.text;

        // Convertir la respuesta a un objeto JSON
        JSONNode response = JSON.Parse(responseJson);

        // Verificar si el inicio de sesión fue exitoso


        if (response.HasKey("error"))
        {
            Debug.LogError("Error al iniciar sesión: " + response["error"]);
        }
        else
        {   
             int userId = response["userId"].AsInt;

             Debug.Log(userId);
            UserSession.Login(username, response["token"],userId);
            Debug.Log("Inicio de sesión exitoso!");
            SceneManager.LoadScene("CharacterMenu");
        }
    }
}
