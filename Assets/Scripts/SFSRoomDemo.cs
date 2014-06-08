using UnityEngine;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Logging;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;

public abstract class SFSRoomDemo : SFSDemo
{
    protected override void OnStart()
    {
        base.OnStart();

        _sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
        _sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);
        _sfs.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);
        _sfs.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);

        _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);

        _isConnecting = true;
    }

    #region RoomJoin
    protected virtual void OnRoomJoin(BaseEvent e)
    {
        print("OnRoomJoin");
        _isConnecting = false;
    }

    protected virtual void OnRoomJoinError(BaseEvent e)
    {
        print("OnRoomJoinError");
        _isConnecting = false;
    }

    private void OnUserEnterRoom(BaseEvent e)
    {
        User user = e.Params.GetUser();
        print("OnUserEnterRoom : " + user.Name);

        OnUserEnteredRoom(user);
    }

    protected virtual void OnUserEnteredRoom(User user)
    {
    }

    private void OnUserExitRoom(BaseEvent e)
    {
        User user = e.Params.GetUser();

        print("OnUserExitRoom : " + user.Name);

        _isConnecting = false;

        OnUserExitedRoom(user);
    }

    protected virtual void OnUserExitedRoom(User user)
    {
    }
    #endregion

    protected virtual void OnExtensionResponse(BaseEvent e)
    {
        //string cmd = e.Params.GetCommande();
        //ISFSObject sfsParams = e.Params.GetParams();
    }
}
