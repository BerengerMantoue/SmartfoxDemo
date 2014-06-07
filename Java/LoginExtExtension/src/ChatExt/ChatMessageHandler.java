package ChatExt;

import ManagerExt.ManagerExtension;
import java.sql.SQLException;
import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.PreparedStatement;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;
import com.smartfoxserver.v2.extensions.ExtensionLogLevel;

// @author Berenger
public class ChatMessageHandler extends BaseClientRequestHandler
{
    @Override
    public void handleClientRequest(User user, ISFSObject isfso)
    {
        String msg = (String)isfso.getUtfString("text");
        
        trace(user.getName() + " just sent a public message : " + msg);
        
        try 
        {
            Connection conn = getParentExtension().getParentZone().getDBManager().getConnection();//This will strip potential SQL injections
        
            int minCount = GetSQLValue(conn, "min_words");
            int maxCount = GetSQLValue(conn, "max_words");
            trace( "Min Count : " + minCount + ". Max : " + maxCount);

            String[] words = msg.split(" ");
            trace("words : " + words);
            if( words.length >= minCount && words.length <= maxCount )
                msg = "Too many words !";
        } 
        catch (SQLException e) 
        {
            trace(ExtensionLogLevel.WARN, " SQL Failed: " + e.toString());
        }
        
        ManagerExtension mgrExt = (ManagerExtension) getParentExtension();
        Room room = user.getLastJoinedRoom();
                
        // Create a response object
        ISFSObject resObj = SFSObject.newInstance(); 
        resObj.putUtfString("text", msg);
        resObj.putUtfString("who", user.getName());
         
        // Send it back to everyone
        send("chatMessage", resObj, room.getUserList());
    }
    
    private int GetSQLValue( Connection conn, String name )
    {
        try 
        {
            PreparedStatement sql = conn.prepareStatement("SELECT value FROM chatsettings WHERE name = ?");
            sql.setString(1, name);
            ResultSet result = sql.executeQuery();
            result.next();
            return result.getInt("value");
        } 
        catch (SQLException e) 
        {
            trace(ExtensionLogLevel.WARN, " SQL Failed: " + e.toString());
            return 0;
        }
    }
}
