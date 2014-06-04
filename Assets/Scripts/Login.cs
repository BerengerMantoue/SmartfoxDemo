using UnityEngine;
using System.Collections;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Logging;


public class Login : SFSDemo
{
    protected override void OnStart()
    {
        _sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
        _sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        _sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
        _sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
    }

    private void OnConnection(BaseEvent e)
    {
        if (e.Params.GetValue<bool>("success"))
        {
            print("Connection Success");
            _sfs.Send(new LoginRequest(userName, password, zoneName));
        }
        else
            print("Connection Failure");
    }

    private void OnConnectionLost(BaseEvent e)
    {
        print("OnConnectionLost");
    }

    private void OnLogin(BaseEvent e)
    {
        print("OnLogin : " + e.Params.GetObject("user"));
    }

    private void OnLoginError(BaseEvent e)
    {
        print("OnLoginError");
    }
}
