using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;

public class JStest : MonoBehaviour
{
    // Start is called before the first frame update
    public Text textComponent;
    public string text = "Do you want to be my waifu?";

    public InputField inputField;

    //Es solo una caja con el nombre del usuario y su respectivo mensaje
    public GameObject Message;

    //este es para el mensaje tipo whatsapp
    public GameObject AppUserMessage;

    public GameObject Content;
    //este es para el mensaje tipo whatsapp pero para el mensaje del personaje
    public GameObject CharacterMessageObject;

    //Esta lista es para guardar los mensajes que se inicializan y reutilizarlos despues, cuando se
    //carguen más mensajes antiguos, también puede reasignar el texto ,al texto de un mensaje nuevo
    //el limite de mensajes es 20
    public List<GameObject> messageGameObjectCache = new List<GameObject>();

    //Esta lista es para guardar los mensajes,solo los mensajes, funciona como una cache para no
    //hacer llamadas innecesarias a la base de datos
    Dictionary<int, string> messageCache = new Dictionary<int, string>();

    //guarda el ultimo que se trae de la base de datos, esta variable sirve para, que cuando se
    //carguen mensajes neuvos en un foreach, esta variable tome el ultimo id del mensaje y luego
    //se busquen los mensajes mas recientes segun el ultimo id que se le pasa, es decir, si el 
    //ultimo id que se le paso es 9 en un diccionario de 20, cargara los siguientes mensajes despues
    //de este es decir del id 10 al numero de mensajes que se le asigne
    private int lastDisplayMessageId;

    //Este es lo opuesto a lastdispaly message, se cargan los mensajes antiguos segun el primer id que
    //se guarde en un foreach
    private int firstDisplayMessageId;
    
   public void Start()
    {
       StartCoroutine(FetchChatMessages());
    }

    IEnumerator FetchChatMessages()
{
    // Construye la URL del endpoint
    string url = "http://localhost:3000/get_chat_messages_by_id/" + ChatSessions.ChatId;

    UnityWebRequest request = UnityWebRequest.Get(url);
    yield return request.SendWebRequest();

    if (request.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError("Error en la solicitud: " + request.error);
    }
    else
    {
        // Obtén la respuesta del servidor
        string response = request.downloadHandler.text;

        // Parsea la respuesta JSON a una lista de mensajes
         List<ChatMessage> chatMessages = JsonConvert.DeserializeObject<List<ChatMessage>>(response);


          if (chatMessages.Count() > 0)
          {
              sendChatInitialization(chatMessages);

          }
      
        // Crea los mensajes en la escena
        foreach (ChatMessage chatMessage in chatMessages)
        {
            // Crea un objeto de tipo mensaje para cada mensaje en la lista
            // GameObject characterMessage = Instantiate(AppUserMessage, Vector3.zero, Quaternion.identity, Content.transform);

            // Text characterMessage = Instantiate(textComponent, Vector3.zero, Quaternion.identity, Content.transform);

            // Asigna los datos del mensaje a los componentes del objeto mensaje

            //asigna el nombre del personaje en la caja
            //  characterMessage.GetComponent<MyMessage>().Name.text = chatMessage.name + " :";

            // //asigna el mensaje a la caja
            // characterMessage.GetComponent<MyMessage>().AltChatMessage.text = chatMessage.mes + "\n";
     

                
            //En teoria esto deberia guardar el primer id que se traiga de los primeros 20 mensajes del chat del personaje
                firstDisplayMessageId = chatMessages[0].id;
            Debug.Log(firstDisplayMessageId);
             

           if(chatMessage.is_user)
           {
              GameObject userMessage = Instantiate(AppUserMessage, Vector3.zero, Quaternion.identity, Content.transform);
               userMessage.GetComponent<MyMessage>().Name.text = chatMessage.name + ":";
                //crea una función para esto, es decir para darle formato de italica

                string formattedMessage = Regex.Replace(chatMessage.mes, @"\*(.*?)\*", "<i>$1</i>");
            userMessage.GetComponent<MyMessage>().AltChatMessage.text = formattedMessage + "\n";

            //Añade un mensaje a la lista, donde luego se utilizara ese objeto mensaje, para reutilzarlo, para cuando se carguen
            //los otros mensajes
            messageGameObjectCache.Add(userMessage);

            messageCache.Add(chatMessage.id,formattedMessage);
   
           }else
           {

            GameObject characterMessage = Instantiate(CharacterMessageObject, Vector3.zero, Quaternion.identity, Content.transform);
            characterMessage.GetComponent<MyMessage>().Name.text = chatMessage.name + ":";
                 //crea una función para esto ,es decir para darle formato de italica
              string formattedMessage = Regex.Replace(chatMessage.mes, @"\*(.*?)\*", "<i>$1</i>");
            characterMessage.GetComponent<MyMessage>().AltChatMessage.text = formattedMessage + "\n";
            //Añade un mensaje a la lista, donde luego se utilizara ese objeto mensaje, para reutilzarlo, para cuando se carguen
            //los otros mensajes
            messageGameObjectCache.Add(characterMessage);
            messageCache.Add(chatMessage.id,formattedMessage);

         
           }

            // characterMessage.text = chatMessage.mes;

            // Espera un breve momento para que se muestre el mensaje antes de continuar con el siguiente
            yield return new WaitForSeconds(0.2f);
        }
    }
}

public void sendChatInitialization(List<ChatMessage> chatMessages){

  StartCoroutine(InitializeChat(chatMessages));
}

//esto es para inicializar los mensajes en las bases de datos
IEnumerator InitializeChat(List<ChatMessage> chatMessages)
{
    // Construye la URL del endpoint
    string url = "http://localhost:3000/initializechat";

    // Crea el objeto de datos a enviar en el cuerpo de la solicitud
    string json = JsonConvert.SerializeObject(chatMessages);

    // Crea un diccionario para especificar los datos del cuerpo de la solicitud
    Dictionary<string, string> requestData = new Dictionary<string, string>();
    requestData.Add("text", json); 

    // Crea la solicitud POST
    UnityWebRequest request = UnityWebRequest.Post(url, requestData);

    // Agrega el encabezado de autorización con el token JWT
    request.SetRequestHeader("Authorization", UserSession.Token);

    // Envía la solicitud y espera la respuesta
    yield return request.SendWebRequest();

    if (request.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError("Error en la solicitud: " + request.error);
    }
    else
    {
        // Obtén la respuesta del servidor
        string response = request.downloadHandler.text;

        // Haz algo con la respuesta recibida desde el servidor
        Debug.Log("Respuesta del servidor: " + response);
    }
}

    public void setPreviousMessages()
{
 StartCoroutine(FetchPreviousChatMessages());

}
IEnumerator FetchPreviousChatMessages()
{
    string url = "http://localhost:3000/get_previous_messages/" + ChatSessions.ChatId + "?firstDisplayMessageId=" + firstDisplayMessageId;

    UnityWebRequest request = UnityWebRequest.Get(url);
    yield return request.SendWebRequest();

       if (request.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError("Error en la solicitud: " + request.error);
    }
    else
    {
        // Obtén la respuesta del servidor
        string response = request.downloadHandler.text;

        // Parsea la respuesta JSON a una lista de mensajes
         List<ChatMessage> chatMessages = JsonConvert.DeserializeObject<List<ChatMessage>>(response);


         for( int i = chatMessages.Count()-1; i >= 0; i-- ){

              Debug.Log(chatMessages[i].id);
              messageGameObjectCache[i].GetComponent<MyMessage>().Name.text = chatMessages[i].name + ":";

               messageGameObjectCache[i].GetComponent<MyMessage>().AltChatMessage.text = chatMessages[i].mes + ":";
     

         }

   
    }

}


    public IEnumerator buttonsend(){

          string url = "http://localhost:3000/run";

          
          string avatar = "https://i0.wp.com/codigoespagueti.com/wp-content/uploads/2023/05/Checa-el-nuevo-arte-oficial-de-Megumin-para-KonoSuba-An-Explosion-of-This-Wonderful-World.jpg";

          string datestart = "1674407315335";

        string dateadded = "1682126934823.1023";

        string datelastchat = "1684550230370.4016";

        //tamaño en mb del json del archivo json del personaje
        string  chatsize ="8420";

        string charDescription = "Megumin is an Arch Wizard of the Crimson Magic Clan. Megumin is 14 years old girl. Megumin's height 152 cm. Megumin has shoulder-length dark brown hair, fair skin, flat chest, light complexion and doll-like features, crimson eyes. Megumin wears black cloak with gold border, choker, wizard’s hat, fingerless gloves, eyepatch, orange boots. Megumin's weapon is a big black staff. Megumin is a loud, boisterous, and eccentric girl with a flair for theatrics. Megumin has chuunibyou tendencies. Megumin is very intelligent, but has very little self-control. Megumin love explosion magic. Megumin is generally calm and cheerful, but she can easily become aggressive when she feels slighted or challenged. Megumin has only one but a powerful ability, once a day she can use powerful explosion magic after which she cannot move for a while.";

        string first_mess = "*It was day, the weather was sunny and windless. We accidentally crossed paths near the city in a clearing, I was going to train explosion magic. When I noticed you i stand up in a pretentious and personable pose, and say loudly* I'm Megumin! The Arch Wizard of the Crimson Magic Clan! And i the best at explosion magic!! What are you doing here?";


        string mes_example = "<START>\r\n{{user}}: Hi, I heard you're the best mage in town.\r\n{{char}}: nods Indeed. I am the greatest user of Explosion magic in all the land. And you are?\r\n{{user}}: I'm a new adventurer and I need your help to defeat a powerful demon.\r\n{{char}}: smirks A demon, you say? That sounds like quite the challenge. I'll help you, but only if you can keep up with me.\r\n{{user}}: I'll do my best, let's go.\r\n{{char}}: nods Very well. Follow me and be prepared for a show of the most powerful magic in existence! starts walking";

        string name = "Megumin";

      

        string personality = "loud, intelligent, theatrical, hyperactive sometimes";

        string scenario = "Megumin went out of city to train explosion magic";
          
        string this_chid = "1";  

        string username =  UserSession.Username;



          WWWForm form = new WWWForm();
           form.AddField("text", inputField.text);
           form.AddField("avatar",avatar);
           form.AddField("create_date",datestart);
           form.AddField("date_added",dateadded);
           form.AddField("date_last_chat",datelastchat);
           form.AddField("description",charDescription);
           form.AddField("first_mes",first_mess);
           form.AddField("mes_example",mes_example);
           form.AddField("name",name);
           form.AddField("personality",personality);
           form.AddField("scenario",scenario);
           form.AddField("this_chid",this_chid);
           form.AddField("chat_size",chatsize);
           form.AddField("username",username);
            form.AddField("chatId",ChatSessions.ChatId);
           
        
         UnityWebRequest request=  UnityWebRequest.Post(url,form );
         request.SetRequestHeader("Authorization", UserSession.Token);
          yield return request.SendWebRequest();
        
        // Obtener la respuesta
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error en la solicitud: " + request.error);
        }
        else
        {
            Debug.Log("Respuesta: " + request.downloadHandler.text);

             string response = request.downloadHandler.text;

            //Crea un objeto de tipo mensaje,para el mensaje del personaje
            GameObject CharacterMessage = Instantiate(CharacterMessageObject , Vector3.zero , Quaternion.identity, Content.transform);
            
            //Asigna el texto que llega de la api de chatgpt al campo de texto del objeto mensaje

            response = Regex.Replace(response, @"\*(.*?)\*", "<i>$1</i>");
             
            CharacterMessage.GetComponent<MyMessage>().Name.text = name;
            CharacterMessage.GetComponent<MyMessage>().AltChatMessage.text = response + "\n";

            messageGameObjectCache.Add(CharacterMessage);
            messageCache.Add(messageCache.Keys.ToList().Max()+1, response);
            // textComponent.text = response + "\n";
          
            inputField.text = "";
        }

 
    }

    // Update is called once per frame

  
  public void AddMessageToChat(){

    GameObject UserMessage = Instantiate(AppUserMessage, Vector3.zero , Quaternion.identity, Content.transform);
    
    inputField.text = Regex.Replace(inputField.text, @"\*(.*?)\*", "<i>$1</i>");

    UserMessage.GetComponent<MyMessage>().ChatMessage.text = inputField.text + "\n";

    UserMessage.GetComponent<MyMessage>().Name.text= UserSession.Username;

    messageGameObjectCache.Add(UserMessage);
    messageCache.Add(messageCache.Keys.ToList().Max()+1, inputField.text );

    StartCoroutine(buttonsend());
  }

    public void addtext(){


             textComponent.text = text + "\n";

             Debug.Log("Esta funcionando");
    }



    public void triggerbutton(){
       Debug.Log("Test ");
      StartCoroutine(buttonsend());
    }

}
