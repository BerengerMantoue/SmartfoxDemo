using UnityEngine;
using System.Collections;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Logging;


public class Login : SFSDemo
{
    protected override void  OnConnectionSuccess(BaseEvent e)
    {
         _sfs.SendLoginRequest(userName, password, zoneName);
    }
}
