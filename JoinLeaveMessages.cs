using Rocket.API.Collections;
    using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System.Net;
using Newtonsoft.Json;
using Color = UnityEngine.Color;

namespace JoinLeaveMessages
{
    public class JoinLeaveMessages : RocketPlugin<JoinLeaveMessagesConfig>
    {
        internal JoinLeaveMessages Instance;
        private Color JoinMessageColor;
        private Color LeaveMessageColor;

        protected override void Load()
        {
            Instance = this;
            if (Instance.Configuration.Instance.JoinMessageEnable)
            {
                JoinMessageColor = ParseColor(Instance.Configuration.Instance.JoinMessageColor);
                U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            }
            if (Instance.Configuration.Instance.LeaveMessageEnable)
            {
                LeaveMessageColor = ParseColor(Instance.Configuration.Instance.LeaveMessageColor);
                U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;
            }
            Instance.Configuration.Save();
        }

        protected override void Unload()
        {
            if (Instance.Configuration.Instance.JoinMessageEnable)
                U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            if (Instance.Configuration.Instance.LeaveMessageEnable)
                U.Events.OnPlayerDisconnected -= Events_OnPlayerDisconnected;
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList
                {
                    { "connect_message", "{0} connected to the server." },
                    { "disconnect_message", "{0} disconnected from the server." },

                    { "connect_message_country", "{0} has connected from {1}." }
                };
            }
        }

        internal Color ParseColor(string color)
        {
            if (color == null)
                return Color.green;
            switch (color.Trim().ToLower())
            {
                case "black":
                    return Color.black;
                case "blue":
                    return Color.blue;
                case "cyan":
                    return Color.cyan;
                case "grey":
                    return Color.grey;
                case "green":
                    return Color.green;
                case "gray":
                    return Color.gray;
                case "magenta":
                    return Color.magenta;
                case "red":
                    return Color.red;
                case "white":
                    return Color.white;
                case "yellow":
                    return Color.yellow;
                case "gold":
                    return new Color(1.0f, 0.843137255f, 0f);
                default:
                    float r;
                    float g;
                    float b;
                    string[] colors = color.Split(',');
                    return (colors.Length == 3 && float.TryParse(colors[0], out r) && float.TryParse(colors[1], out g) && float.TryParse(colors[2], out b) && r >= 0 && r <= 255 && g >= 0 && g <= 255 && b >= 0 && b <= 255) ? new Color(r / 255, g / 255, b / 255) : Color.green;
            }
        }

        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            if (Configuration.Instance.ShowJoinCountry)
                CountryMessage(player);
            else
                Message(player, true);
        }

        private void Events_OnPlayerDisconnected(UnturnedPlayer player)
        {
            if (player != null)
                Message(player, false);
            else
                Logger.LogWarning("Warning: DC message for a player didn't run as the player data was null.");
        }

        private void Message(UnturnedPlayer player, bool join)
        {
            UnturnedChat.Say(Translate(join ? "connect_message" : "disconnect_message", player.CharacterName), join == true ? JoinMessageColor : LeaveMessageColor);
        }

        private void CountryMessage(UnturnedPlayer player)
        {
            SteamGameServerNetworking.GetP2PSessionState(player.CSteamID, out P2PSessionState_t state);
            string adress = Parser.getIPFromUInt32(state.m_nRemoteIP);

            string response = new WebClient().DownloadString("http://ip-api.com/json/" + adress);

            var rootObject = JsonConvert.DeserializeObject<RootObject>(response);

            if (!rootObject.status.Equals("success"))
                Message(player, true);
            else
                UnturnedChat.Say(Translate("connect_message_country", player.CharacterName, rootObject.country), JoinMessageColor);
        }
    }

    public class RootObject
    {
        public string _as { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string countryCode { get; set; }
        public string isp { get; set; }
        public float lat { get; set; }
        public float lon { get; set; }
        public string org { get; set; }
        public string query { get; set; }
        public string region { get; set; }
        public string regionName { get; set; }
        public string status { get; set; }
        public string timezone { get; set; }
        public string zip { get; set; }
    }

}
