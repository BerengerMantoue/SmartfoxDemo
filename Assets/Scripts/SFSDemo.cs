﻿using UnityEngine;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Logging;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;

public abstract class SFSDemo : MonoBehaviour 
{
    public string serverIP = "127.0.0.1";
    public int serverPort = 9933;
    public string zoneName = "";
    public string userName = "Arthur";
    public string password = "Pendragon";
    public LogLevel logLevel = LogLevel.DEBUG;

    protected SmartFox _sfs;
    protected bool _isConnecting = false;
    protected User _me = null;

	private void Start ()
    {
        // Create the instance to the smarfox object
        _sfs = new SmartFox(true);

        _sfs.AddLogListener(logLevel, OnDebugMessage);

        _sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
        _sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);

        _sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
        _sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
        _sfs.AddEventListener(SFSEvent.LOGOUT, OnLogout);

        OnStart();

        // Connect to the server. It need to be running first !
        _sfs.Connect(serverIP, serverPort);
	}

    protected virtual void OnStart()
    {
    }

    #region Connection
    private void OnConnection(BaseEvent e)
    {
        if (e.Params.GetSuccess())
        {
            _isConnecting = false;
            print("Connection Success");
            SmartfoxConnection.Connection = _sfs;

            OnConnectionSuccess(e);
        }
        else
            print("Connection Failure");
    }

    protected virtual void OnConnectionSuccess(BaseEvent e)
    {
    }

    protected virtual void OnConnectionLost(BaseEvent e)
    {
        print("OnConnectionLost");
        _isConnecting = false;
    }
    #endregion

    #region Login
    protected virtual void OnLogin(BaseEvent e)
    {
        print("OnLogin");
        _isConnecting = false;

        _me = e.Params.GetUser();
    }

    protected virtual void OnLoginError(BaseEvent e)
    {
        print("OnLoginError");
        _isConnecting = false;
        _me = null;
    }

    protected virtual void OnLogout(BaseEvent e)
    {
        print("OnLogout");
        _isConnecting = false;
        _me = null;
    }
    #endregion
	
    /// <summary>
    /// As Unity is not thread safe, we process the queued up callbacks every physics tick
    /// Nothing will happen without this.
    /// </summary>
    void FixedUpdate()
    {
        if (_sfs != null)
        {
            _sfs.ProcessEvents();
        }
    }

    public void OnDebugMessage(BaseEvent e)
    {
        string message = e.Params.GetValue<string>("message");
        Debug.Log("[SFS DEBUG] " + message);
    }
}
