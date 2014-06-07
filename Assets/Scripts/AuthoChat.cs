using UnityEngine;
using System;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Logging;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;


public class AuthoChat : SFSDemo
{
    public Chat chat;

    private bool _isConnecting = false;
    private User _me = null;

    protected override void OnStart()
    {
        _sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
        _sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);

        _sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
        _sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
        _sfs.AddEventListener(SFSEvent.LOGOUT, OnLogout);

        _sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
        _sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);
        _sfs.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
        _sfs.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);

        _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);

        chat.onClickedJoinZone += OnClickedJoinZone;
        chat.onClickedJoinRoom += OnClickedJoinRoom;
        chat.onClickedNewLine += OnClickedNewLine;

        chat.zoneList.Add(zoneName);

        _isConnecting = true;
    }

    #region SFS
    #region Connection
    private void OnConnection(BaseEvent e)
    {
        if (e.Params.GetValue<bool>("success"))
        {
            _isConnecting = false;
            print("Connection Success");
        }
        else
            print("Connection Failure");
    }

    private void OnConnectionLost(BaseEvent e)
    {
        print("OnConnectionLost");
        _isConnecting = false;
    } 
    #endregion

    #region Login
    private void OnLogin(BaseEvent e)
    {
        print("OnLogin");
        _isConnecting = false;

        _me = e.Params.GetUser();

        chat.roomList.Clear();
        foreach (Room room in _sfs.RoomList)
            chat.roomList.Add(room.Name);
        chat.zoneMode = 1;
    }

    private void OnLoginError(BaseEvent e)
    {
        print("OnLoginError");
        _isConnecting = false;
        _me = null;
    }

    private void OnLogout(BaseEvent e)
    {
        print("OnLogout");
        _isConnecting = false;
        _me = null;

        chat.roomList.Clear();
        chat.zoneMode = 0;
    } 
    #endregion

    #region RoomJoin
    private void OnRoomJoin(BaseEvent e)
    {
        print("OnRoomJoin");
        _isConnecting = false;
        chat.roomMode = 1;

        RefreshUserList();
    }

    private void OnRoomJoinError(BaseEvent e)
    {
        print("OnRoomJoinError");
        _isConnecting = false;
    }

    private void OnUserEnterRoom(BaseEvent e)
    {
        User user = e.Params.GetUser();
        print("OnUserEnterRoom : " + user.Name);

        RefreshUserList();
    }

    private void OnUserExitRoom(BaseEvent e)
    {
        User user = e.Params.GetUser();

        print("OnUserExitRoom : " + user.Name);

        if (user == _me)
        {
            _isConnecting = false;
            chat.roomMode = 0;
            chat.userList.Clear();
        }
        else
            RefreshUserList();
    }  
    #endregion

    private void OnExtensionResponse(BaseEvent e)
    {
        string cmd = e.Params.GetCommande();
        ISFSObject sfsParams = e.Params.GetParams();

        if (cmd == "chatMessage")
        {
            User sender = e.Params.GetSender();
            string message = sfsParams.GetUtfString("text");

            chat.AddMessage("Anonymous", message);
        }
        else
        {
            // ...
        }
    }
    #endregion

    private void RefreshUserList()
    {
        List<Room> rooms = _sfs.RoomManager.GetJoinedRooms();
        List<User> users = rooms[0].UserList;

        chat.userList.Clear();
        foreach (User user in users)
            chat.userList.Add(user.Name);
    }

    private void OnClickedJoinZone(Chat chat)
    {
        print("OnClickedJoinZone");
        if (!_isConnecting)
        {
            _isConnecting = true;

            if (string.IsNullOrEmpty(_sfs.CurrentZone))
                _sfs.Send(new LoginRequest("", "", zoneName));
            else
                _sfs.Send(new LogoutRequest());
        }
    }

    private void OnClickedJoinRoom(Chat chat)
    {
        print("OnClickedJoinRoom");
        if (!_isConnecting)
        {
            _isConnecting = true;

            if (_sfs.RoomManager.GetJoinedRooms().Count == 0)
                _sfs.Send(new JoinRoomRequest(_sfs.RoomList[0]));
            else
                _sfs.Send(new LeaveRoomRequest());
        }
    }

    private void OnClickedNewLine(Chat chat)
    {
        ISFSObject msg = new SFSObject();
        msg.PutUtfString("text", chat.text);

        _sfs.Send(new ExtensionRequest("chatMessage", msg));
    }

}
