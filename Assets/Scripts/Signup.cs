using UnityEngine;
using System.Collections;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Logging;
using Sfs2X.Entities.Data;


public class Signup : SFSDemo 
{
    public string email = "arthur.p@kaamelott.lgr";
    private const string CMD_SIGNUP = "$SignUp.Submit";

	protected override void  OnStart()
    {
        _sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
	}

    protected override void OnConnectionSuccess(BaseEvent e)
    {
        _sfs.SendLoginRequest("", "", zoneName);
    }

    protected override void OnLogin(BaseEvent e)
    {
        base.OnLogin(e);

        print("OnLogin : " + e.Params.GetObject("user"));
        ISFSObject objOut = new SFSObject();
        objOut.PutUtfString("username", userName);
        objOut.PutUtfString("password", password);
        objOut.PutUtfString("email", email);

        _sfs.SendExtensionRequest(CMD_SIGNUP, objOut);
    }

    private void OnExtensionResponse(BaseEvent e)
    {
        print("OnLoginError");
        string cmd = e.Params.GetValue<string>("cmd");
        ISFSObject objIn = new SFSObject();

        if (cmd == CMD_SIGNUP)
        {
            if (objIn.ContainsKey("errorMessage"))
                Debug.LogError("Signup error : " + objIn.GetUtfString("errorMessage"));
            else if(objIn.ContainsKey("success"))
                Debug.Log("Signup successful");
            else
                Debug.LogError("Signup unknown error");
        }
    }
}
