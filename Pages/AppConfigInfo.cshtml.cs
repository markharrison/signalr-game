using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Pathinox.Pages
{
    public class AppConfigInfoModel : PageModel
    {
        public string strAppConfigInfoHtml;
        IConfiguration _config;
        private readonly GameRoomsState _gameRoomsState;

        public AppConfigInfoModel(IConfiguration config, GameRoomsState gameRoomsState)
        {
            _config = config;
            _gameRoomsState = gameRoomsState;
            strAppConfigInfoHtml = "";

        }

        public void OnGet()
        {
            string pw = HttpContext.Request.Query["pw"].ToString();
            if (string.IsNullOrEmpty(pw) || pw != _config.GetValue<string>("AdminPW"))
                return;

            try
            {
                strAppConfigInfoHtml += "OS Description: " + System.Runtime.InteropServices.RuntimeInformation.OSDescription + "<br/>";
                strAppConfigInfoHtml += "ASPNETCORE_ENVIRONMENT: " + _config.GetValue<string>("ASPNETCORE_ENVIRONMENT") + "<br/>";
                strAppConfigInfoHtml += "Framework Description: " + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription + "<br/>";
                strAppConfigInfoHtml += "Instrumentation Key: " + _config.GetValue<string>("ApplicationInsights:InstrumentationKey") + "<br/>";
                strAppConfigInfoHtml += "Build Identifier: " + _config.GetValue<string>("BuildIdentifier") + "<br/>";
                strAppConfigInfoHtml += "OTEL_EXPORTER_OTLP_ENDPOINT: " + _config.GetValue<string>("OTEL_EXPORTER_OTLP_ENDPOINT") + "<br/>";

                strAppConfigInfoHtml += "<br/>";

                strAppConfigInfoHtml += "<table>";
                strAppConfigInfoHtml += "<tr><td><b>Timestamp</b></td><td>&nbsp;&nbsp;</td><td><b>Game</b></td><td>&nbsp;&nbsp;</td><td><b>Player 1</b></td><td>&nbsp;&nbsp;</td><td><b>Player 2</b></td></tr>";

                foreach (var room in _gameRoomsState.GameRooms)
                {
                    GameState gameState = room.Value;
                    var gameCode = room.Key;

                    string timeStamp = gameState.Timestamp.ToString("dd/MMM HH:mm:ss");
                    var player1 = $"{gameState.Players[0].Name} <span style=\"font-size: smaller;\">({gameState.Players[0].UniqueId})</span>";
                    var player2 = (gameState.Players.Count >= 2) ? $"{gameState.Players[1].Name} <span style=\"font-size: smaller;\">({gameState.Players[1].UniqueId})</span>" : "None";

                    strAppConfigInfoHtml += $"<tr><td>{timeStamp}</td><td>&nbsp;&nbsp;</td><td>{gameCode}</td><td>&nbsp;&nbsp;</td><td>{player1}</td><td>&nbsp;&nbsp;</td><td>{player2}</td></tr>";
                }

                strAppConfigInfoHtml += "</table>";
                strAppConfigInfoHtml += "<br/>";

            }
            catch (Exception ex)
            {
                strAppConfigInfoHtml += ex.Message;
            }

        }
    }
}
