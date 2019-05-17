using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Newtonsoft.Json;

///     You can copy this class and modify Run() to suits your needs.
///     To use this class, you just instantiate, call Start() when you want to start and Stop() when you want to stop.

public class HelloRequester : RunAbleThread
{

    public GameObject Pokedex;

    public byte[] bytes;
    ///     Stop requesting when Running=false.
    protected override void Run()
    {
        ForceDotNet.Force();

        using (RequestSocket client = new RequestSocket())
        {
            client.Connect("tcp://localhost:5555");

            Debug.Log("Client Connected");

            while (Running)
            {
                Debug.Log("Server Running ...");

                if (Send)
                {
                    //string message = client.ReceiveFrameString();
                    client.SendFrame(bytes);
                    string message = null;
                    bool gotMessage = false;

                    while (Running)
                    {
                        Debug.Log("Server Listening ...");
                        gotMessage = client.TryReceiveFrameString(out message); // this returns true if it's successful
                        if (gotMessage) break;
                    }

                    if (gotMessage)
                    {
                        Debug.Log("Received: " + message);



                        Send = false;
                    }
                }

                NetMQConfig.Cleanup();
            }
        }
    }
}