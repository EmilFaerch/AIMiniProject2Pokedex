using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class myClient : MonoBehaviour
{
    TcpClient finder;
    Thread t_Listener;
    Socket client, reader;
    int port = 5222;
    public bool pokemonSearching, isConnected;
    bool updatePokedex;

    Text pokedexText;
    string pokeName;

    // Start is called before the first frame update
    void Start()
    {
        pokedexText = GameObject.Find("Pokedex").transform.Find("Pokemon").GetComponent<Text>();

        isConnected = false;
        updatePokedex = false;
        pokemonSearching = true;

        try
        {
            Debug.Log("Trying to connect to server");

            finder = new TcpClient("127.0.0.1", port);
            client = finder.Client;
            
            if (finder.Connected)
            {
                Debug.Log("Connected to server!");
                isConnected = true;
                client.Blocking = true; // Wait for Socket to finish sending/receiving before attempting to do stuff

                string imagesPath = Application.dataPath + "/Images/";
                byte[] message = Encoding.UTF8.GetBytes(imagesPath);
                client.Send(message);

                t_Listener = new Thread(new ThreadStart(ReceiveMessage));
                t_Listener.IsBackground = false;
                t_Listener.Start();
            }
        }
        catch
        {
            Debug.Log("Error connecting to server.\nMake sure server is running.");
        }
    }

    public new void SendMessage(string text)
    {
        byte[] message = Encoding.UTF8.GetBytes(text);
        client.Send(message);
    }

    void ReceiveMessage()
    {
        while (isConnected)
        {
            while (pokemonSearching == false)
            {
                Debug.Log("Listening for Pokedex result ...");
                byte[] buffer = new byte[1024];
                int bufferLength = client.Receive(buffer);
                string message = Encoding.UTF8.GetString(buffer, 0, bufferLength);

                updatePokedex = true;
                pokeName = message;

                pokemonSearching = true;
            }
        }
    }

    void Update()
    {
        if (updatePokedex)
        {
            pokedexText.text = pokeName;
            updatePokedex = false;
        }
    }

    private void OnApplicationQuit()
    {
        SendMessage("exit");
        isConnected = false;
        t_Listener.Join();
        client.Close();
    }
}
