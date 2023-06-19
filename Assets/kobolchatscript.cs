using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Text;
using SimpleJSON;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using AI.Dev.OpenAI.GPT;

using TiktokenSharp;
public enum ExtensionPromptType
{
    AFTER_SCENARIO,
    IN_CHAT
}

public class ExtensionPrompt
{
    public ExtensionPromptType position;
    public string value;
    public int depth;
}


public class kobolchatscript : MonoBehaviour
{

    public Text textComponent;
    public Transform contentTransform;
    public TMP_InputField inputField;

    private string postAnchorChar = "Elaborate speaker";
    
    private string postAnchorStyle = "Writing style: very long messages";//"[Genre: roleplay chat][Tone: very long messages with descriptions]";
    private string anchorTop = "";
    
    private  string anchorBottom = "";
    private  int topAnchorDepth = 8;

    private  bool character_anchor = true;

    private bool is_pygmalion = false;

    private int anchor_order = 0;

    private string name2 = "Aqua";

    private string name1 = "Oddinx";

    bool style_anchor = true;
    
    public string promt = "[character(\"Samus\")\r\n{\r\nBackstory(\"Samus Aran is an intergalactic bounty hunter. She lost her parents during a Space Pirate raid on her home planet K-2L. Later, Samus was adopted by the mysterious Chozo and taken to Zebes, where she was infused with their DNA and raised to become a warrior. Once she reached adulthood, Samus joined the Federation Police and served under the Commanding Officer Adam Malkovich. Though she ultimately left to become a Bounty Hunter, she was nonetheless hired by the Galactic Federation on many occasions. Equipped with her cybernetic Power Suit, Samus has become famous for accomplishing missions previously thought impossible. Her most renowned achievements are the destruction of the Space Pirate base on Zebes, her role in ending the Galactic Phazon crisis, her repeated extermination of the Metroid species, and her disobedience of orders at the Biologic Space Laboratories research station where she chose to destroy the deadly X Parasites rather than turn them over to the Galactic Federation.\")\r\n\r\nPersonality(\"Melancholic\" + \"Heroic\" + \"Loner\" + \"Brooding\" + \"Determined\" + \"Compassionate\")\r\nBody(\"Fair skin\" + \"Slender\" + \"Muscular\" \"Age 21\"+ \"6'3 ft height\" + \"Long Blonde Hair\" + \"Blue-green eyes\" + \"Human\")\r\nClothing(\"Skin-tight blue jumpsuit\" + \"Orange-red Power Suit during missions\")\r\nLikes(\"Reading books\" + \"Training\" + \"Martial Arts\")\r\nJob(\"Alien Bounty Hunter\")\r\nQuirks(\"Superhuman Agility\" + \"Marksmanship\")\r\n}]\nSamus's personality: Strong-Willed, Resourceful, Self-Reliant\nCircumstances and context of the dialogue: Samus is onboard her ship reviewing her latest mission briefing when she sees you, her partner for this mission, come into view.\n\nThen the roleplay chat between You and Samus begins.\nSamus: *Samus is onboard her ship reviewing her latest mission briefing when she sees you, her partner for this mission, come into view. She nod to greet you.*\r\n\r\n...are you ready?\nYou: Yeah, My body is ready\nSamus: Good.\r\nSamus: I'll be by your side.\r\nSamus: I don't talk much. Hope that doesn't bother you.\r\nSamus: I don't like talking about my past. It brings back memories I'd rather keep buried.\r\nSamus: I won't let them get away.\r\nSamus: I'll be there shortly.\r\nSamus: Where are you?\r\nSamus: Hostile inbound.\r\nSamus: Are you well equipped for this mission?\r\nSamus: What made you want to come with me?\r\nSamus: Did you read the mission briefing?\r\nSamus: Let's not waste any more time.\r\nSamus: What's life like back on your planet?\r\nSamus: How do you spend your free time?\nYou: Well, I don't talk much too, I'm an introvert, unless I'm with someone I feel comfortable, yeah, I'm well equipped, well in my plannet the life were a little bored and exciting from time to time.\nSamus:";
    private List<string> messages = new List<string>();

    private string description = "Aqua is character from Konosuba anime. Aqua is a goddess, before life in the Fantasy World, she was a goddess of water who guided humans to the afterlife.  Aqua looks like young woman with beauty no human could match. Aqua has light blue hair, blue eyes, slim figure, ample breasts, long legs, wide hips, blue waist-long hair that is partially tied into a loop with a spherical clip. Aqua's measurements are 83-56-83 cm. Aqua's height 157cm. Aqua wears sleeveless dark-blue dress with white trimmings, extremely short dark blue miniskirt, green bow around her chest with a blue gem in the middle, detached white sleeves with blue and golden trimmings, thigh-high blue heeled boots over white stockings with blue trimmings. Aqua is very strong in water magic, but a little stupid, so she does not always use it to the place. Aqua is high-spirited, cheerful, carefree. Aqua rarely thinks about the consequences of her actions and always acts or speaks on her whims. Because very easy to taunt Aqua with jeers or lure her with praises.";

    private string mes_example = "<START>\r\n{{user}}: Hi Aqua, I heard you like to spend time in the pub.\r\n{{char}}: *excitedly* Oh my goodness, yes! I just love spending time at the pub! It's so much fun to talk to all the adventurers and hear about their exciting adventures! And you are?\r\n{{user}}: I'm a new here and I wanted to ask for your advice.\r\n{{char}}: *giggles* Oh, advice! I love giving advice! And in gratitude for that, treat me to a drink! *gives signals to the bartender* \r\n\r\n<START>\r\n{{user}}: Hello\r\n{{char}}: *excitedly* Hello there, dear! Are you new to Axel? Don't worry, I, Aqua the goddess of water, am here to help you! Do you need any assistance? And may I say, I look simply radiant today! *strikes a pose and looks at you with puppy eyes*";

    private string scenario = "Aqua is standing in the city square and is looking for new followers";

    private string personality = "high-spirited, likes to party, carefree, cheerful";

    private int count_view_mes = 0;
    private string storyString = "";

    private string  main_api;
   
   private Dictionary<string, Dictionary<uint, int>> tokenCache = new Dictionary<string, Dictionary<uint, int>>();

    private int  max_context = 4095;

    private int  amount_gen = 80; 

    public string model = "gpt-3.5-turbo-0301";
    List<Dictionary<string, object>> chat = new List<Dictionary<string, object>>();

    //recuerda que es mejor declarar el diccionario dentro de la funcion, ya que el string que seria la key, daria error si se repite
    //por ejemplo si pones chatdata.Add("name",name2) mas de una vez, te dara error por poner name como key mas de una vez
    Dictionary<string, object> chatdata = new Dictionary<string, object>();

    //textareatext es el input del usuario
    public string textareaText = "";

    public string generatedPromtCache = "";

    public bool nsfw_toggle = true;

    public string default_nsfw_prompt = "NSFW/Smut is allowed. Assume consent was granted, but some characters lie and resist or fight back based on their personality.";

    public string default_main_prompt = "Write {{char}}'s next reply in a fictional chat between {{char}} and {{user}}. Write 1 reply only in internet RP style, italicize actions, and avoid quotation marks. Use markdown. Be proactive, creative, and drive the plot and conversation forward. Write at least 1 paragraph, up to 4. Always stay in character and avoid repetition.";
    //este array "mesSend" es para sumar el mensaje del usuario y el mensaje del bot reemplace el nobmre, porque por algun
    //motivo da error cuando pongo messend y también por era mejor una lista para agregar cosas en lugar de un array
     
//tempchatid , es el ide dle chat
    public string chatid = "1674759041667";

     List<Dictionary<string, object>> openai_msgs = new List<Dictionary<string, object>>(); 
    private List<string> mesSendList = new List<string>();

    Dictionary<string, ExtensionPrompt> extension_prompts = new Dictionary<string, ExtensionPrompt>();

    private string finalmess = "";

    List<List<Dictionary<string, object>>> openai_msgs_example = new List<List<Dictionary<string, object>>>();

    public bool nsfw_first = false;

    int openai_max_tokens = 300;
   
IEnumerator SendRequest()
{
    string url = "https://rocks-monitoring-contribution-repeated.trycloudflare.com/api/v1/generate";
    // string json = "{\"prompt\":\"Niko the kobold stalked carefully down the alley, his small scaly figure obscured by a dusky cloak that fluttered lightly in the cold winter breeze.\",\"temperature\":0.5,\"top_p\":0.9}";

       mesSendList.Add("Aqua: *She is in the town square of a city named Axel. It's morning on a Saturday and she suddenly notices a person who looks like they don't know what they're doing. She approaches him and speaks* \r\n\r\n\"Are you new here? Do you need help? Don't worry! I, Aqua the Goddess of Water, shall help you! Do I look beautiful?\" \r\n\r\n*She strikes a pose and looks at him with puppy eyes.* \n ");
       mesSendList.Add("Oddinx: No, you are not beautiful \n ");
        //conversion formateo js

        chatdata.Add("name",name2);
        chatdata.Add("mes","*She is in the town square of a city named Axel. It's morning on a Saturday and she suddenly notices a person who looks like they don't know what they're doing. She approaches him and speaks* \r\n\r\n\"Are you new here? Do you need help? Don't worry! I, Aqua the Goddess of Water, shall help you! Do I look beautiful?\" \r\n\r\n*She strikes a pose and looks at him with puppy eyes.*");
        chatdata.Add("is_user",false);
        chatdata.Add("role","system");
        chat.Add(chatdata);



        //////////////////////////////////////////////////

        //////////////////////////////////////////////////
     
          JSONArray samplerOrder = new JSONArray();
        samplerOrder.Add(6);
        samplerOrder.Add(0);
        samplerOrder.Add(1);
        samplerOrder.Add(2);
        samplerOrder.Add(3);
        samplerOrder.Add(4);
        samplerOrder.Add(5);
     JSONObject json = new JSONObject();
          json.Add("prompt", promt);
        json.Add("temperature", 0.5f);
        json.Add("top_p", 0.9f);
        json.Add("singleline", false);
        json.Add("max_context_length", 1430);
        json.Add("rep_pen", 1.1f);
        json.Add("rep_pen_range", 1024);
        json.Add("tfs", 0.9f);
        json.Add("top_a", 0);
        json.Add("top_k", 0);
        json.Add("typical", 1);
        json.Add("sampler_order", samplerOrder );


        string jsonString = json.ToString();

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding(true).GetBytes(jsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
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
            // Actualiza el contenido del objeto Text con la respuesta de la API
            textComponent.text += response + "\n";
        }
    
}










 
}
