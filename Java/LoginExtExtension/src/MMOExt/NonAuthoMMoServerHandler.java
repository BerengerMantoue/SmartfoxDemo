/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

package MMOExt;

import com.smartfoxserver.v2.SmartFoxServer;
import com.smartfoxserver.v2.api.ISFSMMOApi;
import java.util.ArrayList;
import com.smartfoxserver.v2.core.ISFSEvent;
import com.smartfoxserver.v2.core.SFSEventParam;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.Room;
import com.smartfoxserver.v2.entities.variables.SFSUserVariable;
import com.smartfoxserver.v2.mmo.Vec3D;
import com.smartfoxserver.v2.exceptions.SFSException;
import com.smartfoxserver.v2.extensions.BaseServerEventHandler;

/**
 *
 * @author Berenger
 * This class does only one thing : It takes the 2D coordinates sent by the client
 * and send them back with getMMOApi().setUserPosition, allowing the API to dispatch
 * the PROXIMITY_LIST_UPDATE to every player with an AoI that contains the position.
 * 
 * This will NOT visually move the clients, that is handled by the USER_VARIABLES_UPDATE
 * event, client side (for this non authoritative example).
 */
public class NonAuthoMMoServerHandler extends BaseServerEventHandler
{
    @Override
    public void handleServerEvent(ISFSEvent event) throws SFSException
    {
        ArrayList variables = (ArrayList)event.getParameter(SFSEventParam.VARIABLES);
        User user = (User)event.getParameter(SFSEventParam.USER);
        
        if( user != null && variables != null )
        {
            // Get the coordinates
            SFSUserVariable x_var = (SFSUserVariable)variables.get(0);
            SFSUserVariable z_var = (SFSUserVariable)variables.get(1);
            float x = x_var.getDoubleValue().floatValue();
            float z = z_var.getDoubleValue().floatValue();

            //trace( user.getName() + " new pos : (" + x + ", " + z + ")");

            Vec3D pos = new Vec3D(x, 1f, z);
            ISFSMMOApi mmoAPI = SmartFoxServer.getInstance().getAPIManager().getMMOApi();
            Room room = getParentExtension().getParentRoom();
            
            // This will fire the PROXIMITY_LIST_UPDATE for every client close enough
            mmoAPI.setUserPosition(user, pos, room);
        }
    }    
}