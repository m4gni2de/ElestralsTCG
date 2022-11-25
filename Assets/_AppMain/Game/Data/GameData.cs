using System.Collections;
using System.Collections.Generic;
using Decks;
using UnityEngine;
using Databases;

namespace Gameplay.Data
{
    public class GameData
    {
        #region Static Constructors/Properties
        public static readonly string GameDataTable = "GameDTO";
        public static GameData RandomGame()
        {
            List<GameDTO> games = DataService.GetAll<GameDTO>("GameDTO");

            int rand = Random.Range(0, games.Count);
            GameDTO game = games[rand];
            return new GameData(game);
        }
        //public static GameData ByKey(string gameKey)
        //{
        //    GameDTO game = DataService.ByKey<GameDTO>(GameDataTable, "")
        //}
        #endregion

        #region Properties
        public string gameId { get; set; }
        public Player opponent { get; set; }
        public Decklist oppDeck
        {
            get
            {
                return opponent.decklist;
            }
        }
        #endregion

        public GameData(GameDTO dto)
        {
            gameId = dto.gameId;
            Decklist deck = Decklist.Load(dto.deckKey);
            opponent = new Player(dto.playerId, deck, false);
            opponent.SetOfflineLobbyId(99);
        }
       
        
    }
}

