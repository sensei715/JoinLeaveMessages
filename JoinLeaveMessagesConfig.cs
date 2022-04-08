using Rocket.API;

namespace JoinLeaveMessages
{
    public class JoinLeaveMessagesConfig : IRocketPluginConfiguration
    {

        public bool JoinMessageEnable = true;
        public bool LeaveMessageEnable = true;
        public string JoinMessageColor = "green";
        public string LeaveMessageColor = "green";
        public bool ShowJoinCountry = true;

        public void LoadDefaults() { }
    }
}