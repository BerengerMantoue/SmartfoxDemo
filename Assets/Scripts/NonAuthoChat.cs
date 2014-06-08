using UnityEngine;
using System;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Logging;
using Sfs2X.Entities;


public class NonAuthoChat : SFSRoomDemo
{
    public Chat chat;

    protected override void OnStart()
    {
        base.OnStart();

        chat.onClickedJoinZone += OnClickedJoinZone;
        chat.onClickedJoinRoom += OnClickedJoinRoom;
        chat.onClickedNewLine += OnClickedNewLine;

        chat.zoneList.Add(zoneName);
    }

    #region SFS

    #region Login
    protected override void OnLogin(BaseEvent e)
    {
        base.OnLogin(e);

        chat.roomList.Clear();
        foreach (Room room in _sfs.RoomList)
            chat.roomList.Add(room.Name);
        chat.zoneMode = 1;
    }

    protected override void OnLogout(BaseEvent e)
    {
        base.OnLogout(e);

        chat.roomList.Clear();
        chat.zoneMode = 0;
    } 
    #endregion

    #region RoomJoin
    protected override void OnRoomJoin(BaseEvent e)
    {
        base.OnRoomJoin(e);

        chat.roomMode = 1;
        RefreshUserList();
    }

    protected override void OnUserEnteredRoom(User user)
    {
        RefreshUserList();
    }


    protected override void OnUserExitedRoom(User user)
    {
        if (user == _me)
        {
            chat.roomMode = 0;
            chat.userList.Clear();
        }
        else
            RefreshUserList();
    }  
    #endregion

    private void OnPublicMessageReceived(BaseEvent e)
    {
        User sender = e.Params.GetSender();
        string message = e.Params.GetMessage();

        chat.AddMessage(sender.Name, message);
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
                _sfs.SendLoginRequest("", "", zoneName);
            else
                _sfs.SendLogoutRequest();
        }
    }

    private void OnClickedJoinRoom(Chat chat)
    {
        print("OnClickedJoinRoom");
        if (!_isConnecting)
        {
            _isConnecting = true;

            if (_sfs.RoomManager.GetJoinedRooms().Count == 0)
                _sfs.SendJoinRoomRequest(_sfs.RoomList[0]);
            else
                _sfs.SendLeaveRoomRequest();
        }
    }

    private void OnClickedNewLine(Chat chat)
    {
        _sfs.SendPublicMessageRequest(chat.text);
    }

}
