package LoginExt;

import com.smartfoxserver.v2.components.login.LoginAssistantComponent;
import com.smartfoxserver.v2.extensions.SFSExtension;

/**
 *
 * @author Berenger
 */
public class ZoneExtension extends SFSExtension
{
    private LoginAssistantComponent lac;
    
    @Override
    public void init()
    {
        lac = new LoginAssistantComponent(this);
        
        lac.getConfig().loginTable = "users";
        lac.getConfig().userNameField = "username";
        lac.getConfig().passwordField = "password";        
    }
    
    @Override
    public void destroy()
    {
        super.destroy();
    }
}
