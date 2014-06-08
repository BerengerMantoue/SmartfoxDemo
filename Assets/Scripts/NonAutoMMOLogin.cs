using UnityEngine;
using System.Collections;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Requests.MMO;

public class NonAutoMMOLogin : SFSRoomDemo 
{
    protected override void OnConnectionSuccess(Sfs2X.Core.BaseEvent e)
    {
    }

    protected override void OnLogin(Sfs2X.Core.BaseEvent e)
    {
        base.OnLogin(e);

        _sfs.SendJoinRoomRequest(_sfs.RoomList[0]);

        //Debug.Log("Logged in successfully");
        //if (_sfs.GetRoomByName("Test") == null)
        //{
        //    var settings = new MMORoomSettings("Test");
        //    settings.DefaultAOI = new Vec3D(25f, 1f, 25f);
        //    settings.MapLimits = new MapLimits(new Vec3D(-100f, 1f, -100f), new Vec3D(100f, 1f, 100f));
        //    settings.MaxUsers = 100;
        //    settings.Extension = new RoomExtension("LoginExt", "LoginExtExtension.jar");

        //    //settings.ProximityListUpdateMillis = 50;
        //    _sfs.Send(new CreateRoomRequest(settings, true, null));
        //}
        //else
        //{
        //    // We either create the Game Room or join it if it exists already
        //    _sfs.Send(new JoinRoomRequest("Test"));
        //}
    }

    protected override void OnRoomJoin(Sfs2X.Core.BaseEvent e)
    {
        base.OnRoomJoin(e);

        print("Room joined : " + _sfs.LastJoinedRoom.Name);
    }

    protected void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, Screen.height));
        {
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Arthur"))
                _sfs.SendLoginRequest("Arthur", "Pendragon", zoneName);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Lancelot"))
                _sfs.SendLoginRequest("Lancelot", "Du Lac", zoneName);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("<size=25><b>Connection : </b></size>" + (_sfs.IsConnected ? "<color=green>ON</color>" : "<color=red>OFF</color>"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("<size=25><b>Login : </b></size>" + (!string.IsNullOrEmpty(_sfs.CurrentZone) ? "<color=green>OK</color>" : "<color=red>Error</color>"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("<size=25><b>Join room : </b></size>" + (_sfs.JoinedRooms.Count > 0 ? "<color=green>OK</color>" : "<color=red>Error</color>"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Game on"))
            {
                _sfs.RemoveAllEventListeners();
                Application.LoadLevel(1);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
        }
        GUILayout.EndArea();
    }
}
