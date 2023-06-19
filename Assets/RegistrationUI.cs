using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegistrationUI : MonoBehaviour
{

    public static RegistrationUI instance;

    public GameObject loginUI;
    public GameObject registerUI;
    [SerializeField] 
  public RegistrationManager registrationManager;
   
    public InputField usernameInput;
    public InputField emailInput;
    public InputField passwordInput;
    public Button registerButton;


 private void Awake(){

    if(instance == null){
        instance = this;
    }else if (instance != null){

        Destroy(this);
    }
 }

    // private void Start()
    // {
    //     // Asignar la función RegisterUser al evento onClick del botón de registro
    //     registerButton.onClick.AddListener(RegisterUser);
    // }

      //Functions to change the login screen UI
    public void LoginScreen() //Back button
    {
        loginUI.SetActive(true);
        registerUI.SetActive(false);
    }
    public void RegisterScreen() // Regester button
    {
        loginUI.SetActive(false);
        registerUI.SetActive(true);
    }

    // private void RegisterUser()
    // {
    //     // Obtener los valores de los campos de entrada
   
    //     string username = usernameInput.text;
    //     string email = emailInput.text;
    //     string password = passwordInput.text;

    //     Debug.Log(password);

    //     // Llamar a la función de registro del RegistrationManager
    //     registrationManager.RegisterUser(username, email, password);
    // }
}
