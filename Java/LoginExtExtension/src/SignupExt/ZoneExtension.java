package SignupExt;

import com.smartfoxserver.v2.components.signup.SignUpAssistantComponent;
import com.smartfoxserver.v2.extensions.SFSExtension;

/**
 *
 * @author Berenger
 */
public class ZoneExtension extends SFSExtension
{
    private SignUpAssistantComponent suac;
    
    @Override
    public void init()
    {
        suac = new SignUpAssistantComponent();
        
        addRequestHandler(SignUpAssistantComponent.COMMAND_PREFIX, suac);
    }
    
    @Override
    public void destroy()
    {
        super.destroy();
    }
}
