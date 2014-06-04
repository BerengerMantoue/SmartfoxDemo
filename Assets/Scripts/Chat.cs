using UnityEngine;
using System;
using System.Collections.Generic;

public class Chat : MonoBehaviour
{
    public GUISkin skin;
    public Vector2 windowSize;
    public float connectWidth = 90f;
    public float usersWidth = 90f;
    public float bottomLineHeight = 30f;
    public float border = 10f;
    public float btnConnectHeight = 25f;

    private Rect _windowRect, _chatRect, _usersRect, _connectRect, _inputRect;

    [HideInInspector()]
    public List<string> userList;
    [HideInInspector()]
    public List<string> zoneList;
    [HideInInspector()]
    public List<string> roomList;

    public int zoneMode = 0;
    public int roomMode = 0;

    private string _text = "";
    public string text { get { return _text; } }

    public Action<Chat> onClickedJoinZone = null;
    public Action<Chat> onClickedJoinRoom = null;
    public Action<Chat> onClickedNewLine = null;

    private class ChatMessage
    {
        public string username;
        public string text;
        public System.DateTime date;
    }

    private List<ChatMessage> _messages;

    private void Awake()
    {
        userList = new List<string>();
        zoneList = new List<string>();
        roomList = new List<string>();

        _messages = new List<ChatMessage>();
    }

    #region GUI
    private void OnGUI()
    {
        GUI.skin = skin;

        float width = Screen.width * windowSize.x;
        float height = Screen.height * windowSize.y;

        _windowRect = new Rect((Screen.width - width) / 2f, (Screen.height - height) / 2f,
                                    Screen.width * windowSize.x, Screen.height * windowSize.y);

        GUILayout.BeginArea(_windowRect, GUI.skin.box);
        {
            GUILayout.BeginHorizontal();
            {
                // Chat
                DrawChat();

                GUILayout.FlexibleSpace();

                // Users
                DrawUsers();

                GUILayout.FlexibleSpace();

                // Connect
                DrawConnectPanel();
            }
            GUILayout.EndHorizontal();

            // Line
            DrawBottomLine();
        }
        GUILayout.EndArea();
    }

    private void DrawChat()
    {
        _chatRect = new Rect(border, border, (_windowRect.width - border * 4f) - connectWidth - usersWidth, (_windowRect.height - border * 3f) - bottomLineHeight);
        GUILayout.BeginArea(_chatRect, GUI.skin.box);
        {
            foreach (ChatMessage message in _messages)
                GUILayout.Label("<b>[" + message.username + "] : </b>" + message.text);
        }
        GUILayout.EndArea();
    }

    private void DrawUsers()
    {
        GUI.enabled = zoneMode == 1 && roomMode == 1;

        _usersRect = new Rect(_chatRect.xMax + border, _chatRect.y, usersWidth, _chatRect.height);

        GUILayout.BeginArea(_usersRect, GUI.skin.box);
        {
            foreach (string user in userList)
                GUILayout.Label(user);
        }
        GUILayout.EndArea();

        GUI.enabled = true;
    }

    private void DrawConnectPanel()
    {
        _connectRect = new Rect(_usersRect.xMax + border, _chatRect.y, connectWidth, _chatRect.height);
        float selectionHeight = (_connectRect.height - border * 3f - btnConnectHeight * 2f) / 2f;

        GUILayout.BeginArea(_connectRect);
        {
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(selectionHeight));
            {
                if (zoneList.Count == 0)
                    GUILayout.Label("");

                foreach( string zone in zoneList )
                    GUILayout.Label(zone);
            }
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(zoneMode == 0 ? "Join Zone" : "Disconnect Zone", GUILayout.Height(btnConnectHeight)))
            {
                // Join
                RaiseAction(onClickedJoinZone);
            }

            GUILayout.FlexibleSpace();

            GUI.enabled = zoneMode == 1;

            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(selectionHeight));
            {
                if (roomList.Count == 0)
                    GUILayout.Label("");

                foreach (string room in roomList)
                    GUILayout.Label(room);
            }
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(roomMode == 0 ? "Join Room" : "Disconnect Room", GUILayout.Height(btnConnectHeight)))
            {
                // Join
                RaiseAction(onClickedJoinRoom);
            }

            GUI.enabled = true;
        }
        GUILayout.EndArea();
    }

    private void DrawBottomLine()
    {
        GUI.enabled = zoneMode == 1 && roomMode == 1;

        _inputRect = new Rect(_chatRect.x, _chatRect.yMax + border, _connectRect.xMax - _chatRect.x, bottomLineHeight);
        GUILayout.BeginArea(_inputRect);
        {
            GUILayout.BeginHorizontal();
            {
                _text = GUILayout.TextArea(_text, GUILayout.Width(_usersRect.xMax - _chatRect.x), GUILayout.Height(bottomLineHeight));

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Add", GUILayout.Width(_connectRect.width), GUILayout.Height(bottomLineHeight))
                    && !string.IsNullOrEmpty(_text))
                {
                    RaiseAction(onClickedNewLine);
                    _text = "";
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();

        GUI.enabled = true;
    } 
    #endregion

    public void AddMessage(string username, string text)
    {
        ChatMessage message = new ChatMessage();
        message.username = username;
        message.text = text;
        message.date = System.DateTime.Now;

        _messages.Add(message);
    }

    private void RaiseAction(Action<Chat> action)
    {
        if (action != null)
            action(this);
    }
}
