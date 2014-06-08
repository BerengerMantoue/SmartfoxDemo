package ManagerExt;

import ChatExt.ChatMessageHandler;
import MMOExt.NonAuthoMMoServerHandler;
import com.smartfoxserver.v2.extensions.SFSExtension;
import com.smartfoxserver.v2.core.SFSEventType;

// @author Berenger
public class ManagerExtension extends SFSExtension
{    
    @Override
    public void init()
    {
        //instance = this;
        addRequestHandler("chatMessage", ChatMessageHandler.class);
        
        addEventHandler(SFSEventType.USER_VARIABLES_UPDATE, NonAuthoMMoServerHandler.class);
    }
    
    @Override
    public void destroy()
    {
        super.destroy();
    }
}
