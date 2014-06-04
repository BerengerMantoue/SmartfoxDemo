using UnityEngine;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Logging;

public class SFSDemo : MonoBehaviour 
{
    public string serverIP = "127.0.0.1";
    public int serverPort = 9933;
    public string zoneName = "";
    public string userName = "Arthur";
    public string password = "Pendragon";
    public LogLevel logLevel = LogLevel.DEBUG;

    protected SmartFox _sfs;

	private void Start ()
    {
        // Create the instance to the smarfox object
        _sfs = new SmartFox(true);
        
        _sfs.AddLogListener(logLevel, OnDebugMessage);

        OnStart();

        // Connect to the server. It need to be running first !
        _sfs.Connect(serverIP, serverPort);
	}

    protected virtual void OnStart()
    {
    }
	
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

    private void OnApplicationQuit()
    {
        _sfs.Disconnect();
    }
}
