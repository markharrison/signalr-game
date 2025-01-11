using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks.Dataflow;
using System.Xml.Linq;
using System.Xml;

namespace Pathinox.Pages
{
    public class GameRoomsModel : PageModel
    {
        public string strHtml;
        IConfiguration _config;
        private readonly GameRoomsState _gameRoomsState;

        public GameRoomsModel(IConfiguration config, GameRoomsState gameRoomsState)
        {
            _config = config;
            _gameRoomsState = gameRoomsState;
            strHtml = "";

        }

        public void OnGet()
        {

            var nameCookie = HttpContext.Request.Cookies["pxname"];
            var uniqueIdCookie = HttpContext.Request.Cookies["pxuniqueid"];
            nameCookie = (!string.IsNullOrEmpty(nameCookie)) ? nameCookie.Replace("%20", " ") : "";
            uniqueIdCookie = (!string.IsNullOrEmpty(nameCookie)) ? uniqueIdCookie : "";

            strHtml += "<table>";
            strHtml += "<tr><td><b>Timestamp</b></td><td>&nbsp;&nbsp;</td><td><b>Game</b></td><td>&nbsp;&nbsp;</td><td><b>Player 1</b></td><td>&nbsp;&nbsp;</td><td><b>Player 2</b></td></tr>";

            foreach (var room in _gameRoomsState.GameRooms)
            {
                GameState gameState = room.Value;
                var gameCode = room.Key;

                TimeSpan timeDifference = DateTime.Now - gameState.Timestamp;
                if (timeDifference.TotalHours > 24)
                {
                    _gameRoomsState.GameRooms.Remove(gameCode, out _);
                    continue;
                }

                string timeStamp = gameState.Timestamp.ToString("dd/MMM HH:mm:ss");
                var player1 = gameState.Players[0].Name;
                if (gameState.Players[0].UniqueId == uniqueIdCookie)
                {
                    player1 = $"<b>{player1}</b>&nbsp;&nbsp;&nbsp;<button onclick=\"location.href='./?join={gameCode}'\">Open</button>";
                }

                var player2 = "";
                if (gameState.Players.Count >= 2)
                {
                    player2 = gameState.Players[1].Name;
                    if (gameState.Players[1].UniqueId == uniqueIdCookie)
                    {
                        player2 = $"<b>{player2}</b>&nbsp;&nbsp;&nbsp;<button onclick=\"location.href='./?join={gameCode}'\">Open</button>";
                    }
                }
                else
                {
                    player2 = "-- none --";
                    if (gameState.Players[0].UniqueId != uniqueIdCookie && gameState.Players[0].Name != nameCookie) 
                    {
                        player2 = $"<button onclick=\"location.href='./?join={gameCode}'\">Join</button>";
                    }
                }

                strHtml += $"<tr><td>{timeStamp}</td><td>&nbsp;&nbsp;</td><td>{gameCode}</td><td>&nbsp;&nbsp;</td><td>{player1}</td><td>&nbsp;&nbsp;</td><td>{player2}</td></tr>";
            }

            strHtml += "</table>";
            strHtml += "<br/>";

        }
    }
}
