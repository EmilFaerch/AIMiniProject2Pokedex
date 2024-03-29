﻿using UnityEngine;
using AsyncIO;
using NetMQ;
using NetMQ.Sockets;
using System.Collections;
using System.IO;
using System.Text;

public class HelloClient : MonoBehaviour
{
    private HelloRequester _helloRequester;
    public bool SendPack = true;
    private byte[] current_img;

    public void SendObject(string msg)
    {    
        _helloRequester.bytes = Encoding.ASCII.GetBytes(msg);
        _helloRequester.Continue();
    }


    private void OnPostRender()
    {
        /*
        if (SendPack)
        {
            string hey = "Hello world!";
            _helloRequester.bytes = Encoding.ASCII.GetBytes(hey);
            _helloRequester.Continue();
        }
        else if (!SendPack)
        {
            _helloRequester.Pause();

        }
        */
    }

    private void Start()
    {   
        _helloRequester = new HelloRequester();
        Debug.Log("HelloClient: helloRequester initialized.");
        _helloRequester.Start();
    }

    private void OnDestroy()
    {
        _helloRequester.Stop();
    }

    private void OnApplicationQuit()
    {
        _helloRequester.Stop();
        Destroy(this);
    }
}