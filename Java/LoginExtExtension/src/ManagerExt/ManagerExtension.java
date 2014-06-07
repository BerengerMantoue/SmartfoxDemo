package ManagerExt;

import ChatExt.ChatMessageHandler;
import com.smartfoxserver.v2.extensions.SFSExtension;

// @author Berenger
public class ManagerExtension extends SFSExtension
{    
    @Override
    public void init()
    {
        //instance = this;
        addRequestHandler("chatMessage", ChatMessageHandler.class);
    }
    
    @Override
    public void destroy()
    {
        super.destroy();
    }
}
