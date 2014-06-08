using UnityEngine;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using System.Collections;
using System.Collections.Generic;

public class NonAutoMMOGame : MonoBehaviour 
{
    public GameObject playerPrefab;

    private SmartFox smartfox;
    private GameObject localPlayer;
    private PlayerController localPlayerController;
    private Dictionary<SFSUser, GameObject> remotePlayers = new Dictionary<SFSUser, GameObject>();

    private void Start()
    {
        // Make sure the smartfox connection is up and running
	    if (!SmartfoxConnection.isInitialized) 
	    {
		    Application.LoadLevel(0);
		    return;
	    }
        
        // Save the smartox object ref
        smartfox = SmartfoxConnection.Connection;

        // Register events
        smartfox.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        smartfox.AddEventListener(SFSEvent.USER_VARIABLES_UPDATE, OnUserVariableUpdate);
        smartfox.AddEventListener(SFSEvent.PROXIMITY_LIST_UPDATE, OnProximityListUpdate);
        
        // This will spawn player
        SpawnLocalPlayer();
    }

    void FixedUpdate()
    {
        if (smartfox != null)
        {
            smartfox.ProcessEvents();

            // If the player made a move, send it to the server
            if (localPlayer != null && localPlayerController != null && localPlayerController.movementDirty)
            {
                List<UserVariable> userVariables = new List<UserVariable>();
                userVariables.Add(new SFSUserVariable("x", (double)localPlayer.transform.position.x));
                userVariables.Add(new SFSUserVariable("z", (double)localPlayer.transform.position.z));
                userVariables.Add(new SFSUserVariable("rot", (double)localPlayer.transform.rotation.eulerAngles.y));
                smartfox.SendUserVariablesRequest(userVariables);

                localPlayerController.movementDirty = false;
            }
        }
    }

    private void OnConnectionLost(BaseEvent e)
    {
        // Reset all internal states so we kick back to login screen
        smartfox.RemoveAllEventListeners();
        Application.LoadLevel(0);
    }

    // When user variable is updated on any client, then this callback is being received
    // This is where most of the magic happens
    private void OnUserVariableUpdate(BaseEvent e)
    {
        SFSUser user = e.Params.GetSFSUser();
        
        if (user == smartfox.MySelf) return;
        if (!remotePlayers.ContainsKey(user)) return;

        ArrayList changedVars = e.Params.GetChangedVars();

        // Check if the remote user changed his position or rotation
        if (changedVars.Contains("x") || changedVars.Contains("y") || changedVars.Contains("z") || changedVars.Contains("rot"))
        {
            // Move the character to a new position...
            Vector3 pos = new Vector3((float)user.GetVariable("x").GetDoubleValue(), 1, (float)user.GetVariable("z").GetDoubleValue());
            Quaternion quat = Quaternion.Euler(0, (float)user.GetVariable("rot").GetDoubleValue(), 0);
            SimpleRemoteInterpolation interp = remotePlayers[user].GetComponent<SimpleRemoteInterpolation>();

            interp.SetTransform(pos, quat, true);
        }

        // Remote client got new name?
        if (changedVars.Contains("name"))
        {
            remotePlayers[user].GetComponentInChildren<TextMesh>().text = user.Name;
        }
    }

    /// <summary>
    /// This is called by Smartfox when the mmo api detects that a user enters or leaves the AoI.
    /// For that, it is necessary to have an extension that sends a setUserPosition to the mmo api.
    /// </summary>
    /// <param name="e"></param>
    private void OnProximityListUpdate(BaseEvent e)
    {
        List<User> addedUsers = e.Params.GetAddedUsers();
        List<User> removedUsers = e.Params.GetRemovedUsers();

        // Handle all new Users
        foreach (User user in addedUsers)
        {
            Vector3 pos = new Vector3(user.AOIEntryPoint.FloatX, user.AOIEntryPoint.FloatY, user.AOIEntryPoint.FloatZ);
            Quaternion quat = Quaternion.Euler(0, (float)user.GetVariable("rot").GetDoubleValue(), 0);
            SFSUser sfsuser = (SFSUser)user;

            SpawnRemotePlayer(sfsuser, pos, quat);
        }

        // Handle removed users
        foreach (User user in removedUsers)
        {
            RemoveRemotePlayer((SFSUser)user);
        }
    }
    
    /// <summary>
    /// Creates my player
    /// </summary>
    private void SpawnLocalPlayer()
    {
        Vector3 pos;
        Quaternion rot;

        // See if there already exists a model - if so, take its pos+rot before destroying it
        if (localPlayer != null)
        {
            pos = localPlayer.transform.position;
            rot = localPlayer.transform.rotation;
            Camera.main.transform.parent = null;
            Destroy(localPlayer);
        }

        else
        {
            pos = new Vector3(0, 1, 0);
            rot = Quaternion.identity;
        }

        // Lets spawn our local player model
        localPlayer = GameObject.Instantiate(playerPrefab) as GameObject;
        localPlayer.transform.position = pos;
        localPlayer.transform.rotation = rot;
        
        localPlayerController = localPlayer.AddComponent<PlayerController>();
        localPlayer.GetComponentInChildren<TextMesh>().text = smartfox.MySelf.Name;
        Camera.main.transform.parent = localPlayer.transform;

        // Send the position/rotation to the server so the other player can see me move
        List<UserVariable> userVariables = new List<UserVariable>();
        userVariables.Add(new SFSUserVariable("x", (double)localPlayer.transform.position.x));
        //userVariables.Add(new SFSUserVariable("y", (double)localPlayer.transform.position.y));
        userVariables.Add(new SFSUserVariable("z", (double)localPlayer.transform.position.z));
        userVariables.Add(new SFSUserVariable("rot", (double)localPlayer.transform.rotation.eulerAngles.y));
        
        // Send request
        smartfox.SendUserVariablesRequest(userVariables);
    }

    /// <summary>
    /// A player entered the AoI, it has to be spawned.
    /// </summary>
    /// <param name="user">The user corresponding to the futur player</param>
    /// <param name="pos">Where to spot him.</param>
    /// <param name="rot">It's rotation.</param>
    private void SpawnRemotePlayer(SFSUser user, Vector3 pos, Quaternion rot)
    {
        // See if there already exists a model so we can destroy it first
        if (remotePlayers.ContainsKey(user) && remotePlayers[user] != null)
        {
            Destroy(remotePlayers[user]);
            remotePlayers.Remove(user);
        }

        // Lets spawn our remote player model
        GameObject remotePlayer = GameObject.Instantiate(playerPrefab) as GameObject;
        remotePlayer.AddComponent<SimpleRemoteInterpolation>();
        remotePlayer.GetComponent<SimpleRemoteInterpolation>().SetTransform(pos, rot, false);
        remotePlayer.renderer.material.color = Color.red;

        // Color and name
        remotePlayer.GetComponentInChildren<TextMesh>().text = user.Name;

        // Lets track the dude
        remotePlayers.Add(user, remotePlayer);
    }

    /// <summary>
    /// A player left the AoI, stop tracking him
    /// </summary>
    /// <param name="user"></param>
    private void RemoveRemotePlayer(SFSUser user)
    {
        if (user == smartfox.MySelf) return;

        if (remotePlayers.ContainsKey(user))
        {
            Destroy(remotePlayers[user]);
            remotePlayers.Remove(user);
        }
    }	
}
