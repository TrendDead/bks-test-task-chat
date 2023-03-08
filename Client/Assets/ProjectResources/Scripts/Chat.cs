using System.Collections.Generic;
using UnityEngine;

public class Chat : MonoBehaviour
{
    public static List<string> Message = new List<string>();

    private const int PORT = 90;

    private Client _client;
    private string _outMessge = "";
    private string _ip = "";
    private string _name = "";

    private void Start()
    {
        Application.runInBackground = true;
    }

    private void Connect()
    {
        Debug.Log(_ip);
        _client = new Client(PORT, _ip);
        _client.Work();
    }

    private void OnGUI()
    {
        GUILayout.BeginScrollView(new Vector2(0, 0), GUILayout.Width(500), GUILayout.Height(300));
        foreach (var message in Message)
        {
            GUILayout.Label(message);
        }
        GUILayout.EndScrollView();

        GUILayout.Label("Enter name:");
        _name = GUILayout.TextField(_name);
        _outMessge = GUILayout.TextArea(_outMessge, GUILayout.Width(500), GUILayout.Height(100));

        if(GUILayout.Button("Send"))
        {
            _client.SendMessage(name + " : " + _outMessge);
        }

        GUILayout.BeginHorizontal();

        GUILayout.Label("Enter ip:");
        _ip = GUILayout.TextField(_ip);
        if (GUILayout.Button("Connect"))
        {
            Connect();
        }

        GUILayout.EndHorizontal();
    }
}
