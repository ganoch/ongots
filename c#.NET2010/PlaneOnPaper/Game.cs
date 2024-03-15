using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Security.Cryptography;

namespace PlaneOnPaper
{
    public enum GameStatuses { Null =0,
        WaitingForSettings, 
        PlacePlanes, 
        Ready, 
        Disconnected, 
        GameStarted,
        ReadyToShoot,
        Validating,
        NotValid,
        Finished};
    public class Game
    {
        public static Game TheGame = null;

        public static UserOpponent Me { get; set; }
        public static AOpponent Opponent { get; set; }

        public static string Message { get; set; }


        private static AOpponent[] _players = new AOpponent[2];
        public static AOpponent[] Players { get { return Game._players; } }

        private static int turn = 0;
        private static int _myturn = -1;

        public static int MyTurn 
        { 
            get {
                if (Game._myturn < 0 && (!Game.IsNetwork || Game.IsServer))
                {
                    Game._myturn = Game._rand.Next(2);
                    if (Game.IsNetwork)
                    {
                        TCPProtocol.SendTurnInfo(Game._myturn);
                    }
                }
                return Game._myturn;
            }
            set { if (Game.IsNetwork && !Game.IsServer) Game._myturn = value; }
        }
        public static int Turn { get { return Game.turn; } }

        private static Thread[] place_plane_threads = new Thread[2];
        private static Thread game;

        private static Random _rand;
        public static Random Rand { get { return Game._rand; } }

        public static void InitializeGame()
        {
            Game.Me = new UserOpponent();
            Game.GameStatus = GameStatuses.WaitingForSettings;
            Game.IsNetwork = false;
            Game._rand = new Random();
        }

        private static GameStatuses status;
        public static GameStatuses GameStatus
        {
            set
            {
                if (Game.status != value)
                {
                    GameStatuses old_status = Game.status;
                    Game.status = value;
                    Game.Me.GameStatus = value;
                    if (Game.Opponent != null)
                        Game.GameStatus = value;

                    if (value == GameStatuses.PlacePlanes)
                    {
                        System.Diagnostics.Debug.WriteLine("Place planes");
                        Game.GamePlacePlanes();
                    }
                    else if (value == GameStatuses.Ready)
                    {
                        System.Diagnostics.Debug.WriteLine("Ready to play");
                        Game.GameStart();
                    }
                    else if (value == GameStatuses.Finished && Game.IsNetwork 
                        && old_status != GameStatuses.Validating
                        && old_status != GameStatuses.Null)
                    {
                        System.Diagnostics.Debug.WriteLine("Validating");
                        TCPProtocol.SendInitialBoard(Game.Me.MyField);
                        Game.status = GameStatuses.Validating;
                    }
                    else if (value == GameStatuses.Finished && old_status != GameStatuses.GameStarted && old_status != GameStatuses.Validating)
                    {
                        System.Diagnostics.Debug.WriteLine("Finish not allowed");
                        Game.status = old_status;
                    }
                    else if (value == GameStatuses.Disconnected &&
                         old_status != GameStatuses.Finished &&
                        old_status != GameStatuses.NotValid &&
                         old_status != GameStatuses.Validating)
                    {
                        System.Diagnostics.Debug.WriteLine("Network Game Disconnected");
                        Game.StopGame();
                        TCPProtocol.StopTCPListener();
                        UDPProtocol.StopUDPListener();
                    }
                    else if (value == GameStatuses.Disconnected &&
                         (old_status == GameStatuses.Finished ||
                        old_status == GameStatuses.NotValid ||
                         old_status == GameStatuses.Validating))
                    {
                        System.Diagnostics.Debug.WriteLine("Network Game Disconnected, but finished");
                        TCPProtocol.StopTCPListener();
                        UDPProtocol.StopUDPListener();
                        Game.status = old_status;
                    }
                    else if (value == GameStatuses.Finished && Game.IsNetwork)
                    {
                        System.Diagnostics.Debug.WriteLine("Network Game finished");
                        TCPProtocol.StopTCPListener();
                    }
                    else
                    {
                        switch (Game.status)
                        {
                            case GameStatuses.WaitingForSettings:
                                System.Diagnostics.Debug.WriteLine("Тохиргоо дутуу байна");
                                break;

                            case GameStatuses.PlacePlanes:
                                System.Diagnostics.Debug.WriteLine("Онгоц байршлуулна уу");
                                break;

                            case GameStatuses.Ready:
                                System.Diagnostics.Debug.WriteLine("Тоглоом эхлэж байна");
                                break;

                            case GameStatuses.GameStarted:
                                System.Diagnostics.Debug.WriteLine("Тоглоом эхлэв");
                                break;

                            case GameStatuses.Validating:
                                System.Diagnostics.Debug.WriteLine("Тоглолт дууссан, хожлыг шалгаж байна");
                                break;

                            case GameStatuses.NotValid:
                                System.Diagnostics.Debug.WriteLine("Алдаатай тоглоом (анхны утга тохирохгүй байна)");
                                break;

                            case GameStatuses.Finished:
                                System.Diagnostics.Debug.WriteLine("Дуусав");
                                break;

                            case GameStatuses.Disconnected:
                                System.Diagnostics.Debug.WriteLine("Холболт тасарсан");
                                break;

                        }
                    }
                }

                switch (Game.status)
                {
                    case GameStatuses.WaitingForSettings:
                        Game.Message = "Тохиргоо дутуу байна";
                        break;

                    case GameStatuses.PlacePlanes:
                        Game.Message = "Онгоц байршлуулна уу";
                        break;

                    case GameStatuses.Ready:
                        Game.Message = "Тоглоом эхлэж байна";
                        break;

                    case GameStatuses.GameStarted:
                        Game.Message = "Тоглоом эхлэв";
                        break;
                    
                    case GameStatuses.Validating:
                        Game.Message = "Тоглолт дууссан, хожлыг шалгаж байна";
                        break;

                    case GameStatuses.NotValid:
                        Game.Message = "Алдаатай тоглоом (анхны утга тохирохгүй байна)";
                        break;

                    case GameStatuses.Finished:
                        Game.Message = (Game.Opponent !=null?((Game.Me.Defeated ? Game.Opponent.Name : Game.Me.Name) + " хожив"):"");
                        break;

                    case GameStatuses.Disconnected:
                        Game.Message = "Холболт тасарсан";
                        break;

                }
            }
            get { return Game.status; }
        }

        #region Game Control
        private static void GamePlacePlanes()
        {
            abort_place_planes = true;
            for (int i = 0; i < 2; i++)
            {
                if (place_plane_threads[i] != null)
                {
                    place_plane_threads[i].Join(1000);
                }
            }
            abort_place_planes = false;


            Thread placeMyPlanesThread = new Thread(PlaceMyPlanes);
            Thread placeEnemyPlanesThread = new Thread(PlaceEnemyPlanes);

            placeEnemyPlanesThread.Start();
            placeMyPlanesThread.Start();

            place_plane_threads[0] = placeEnemyPlanesThread;
            place_plane_threads[1] = placeMyPlanesThread;
        }

        private static volatile bool abort_place_planes = false;

        //thread
        private static void PlaceMyPlanes()
        {
        
            while (!Game.Me.PlacePlanes(Game.NumberOfPlanes) && !abort_place_planes)
            {

            }
            if (Game.Opponent != null && Game.Opponent.PlanesArePlaced && Game.GameStatus == GameStatuses.PlacePlanes && !abort_place_planes)
            {
                Game.GameStatus = GameStatuses.Ready;
            }

#if DEBUG
            if (abort_place_planes)
                System.Diagnostics.Debug.WriteLine("place enemy my aborted");
#endif
            
        }

        //thread
        private static void PlaceEnemyPlanes()
        {
            while ((Game.Opponent == null || !Game.Opponent.PlacePlanes(Game.NumberOfPlanes)) && !abort_place_planes)
            {

            }
            if (Game.Me.PlanesArePlaced && Game.GameStatus == GameStatuses.PlacePlanes && !abort_place_planes)
            {
                Game.GameStatus = GameStatuses.Ready;
            }
#if DEBUG
            if (abort_place_planes)
                System.Diagnostics.Debug.WriteLine("place enemy planes aborted");
#endif
        }

        private static void GameStart()
        {
            if (Game.game != null && Game.game.ThreadState == ThreadState.Running)
                Game.game.Interrupt();

            while (Game.MyTurn < 0)
            {
            }

            Game.Players[Game.MyTurn] = Game.Me;

            if (Game.MyTurn != 1)
                Game.Players[1] = Game.Opponent;
            else
                Game.Players[0] = Game.Opponent;

            Game.turn = 0;
            Game.GameStatus = GameStatuses.GameStarted;
            Game.game = new Thread(GameRun);
            game.Start();
        }

        //thread
        private static void GameRun()
        {
            Game.Me.ShootEnemyHandle = Game.Opponent.ShotAt;
            Game.Opponent.ShootEnemyHandle = Game.Me.ShotAt;

            while (!Game.Me.Defeated && !Game.Opponent.Defeated && Game.GameStatus == GameStatuses.GameStarted) //тоглоом эхлэсэн болон тоглогчид хожигдоогүй үед
            {
                while (!Game.Players[turn % 2].MakeShot() && Game.GameStatus == GameStatuses.GameStarted) //while тухайн тоглогч буудалт хийгээгүй
                {
                }
                Game.turn++;//дараагийн тоглогчруу шилжих
            }
            Game.GameStatus = GameStatuses.Finished;

        }


        public static void Reset()
        {
            System.Diagnostics.Debug.WriteLine("resetting game");
            Game.abort_place_planes = true;
            for (int i = 0; i < 2; i++)
            {

                if (place_plane_threads[i] != null)
                    place_plane_threads[i].Join(1000);
            }
            Game.abort_place_planes = false;

            if (Game.IsNetwork && Game.Opponent != null && Game.Opponent is TCPOpponent)
            {
                ((TCPOpponent)Game.Opponent).CurrentShot = 99;
            }

            Game.GameStatus = GameStatuses.Null;
            if (Game.game != null)
                Game.game.Interrupt();

            Game.Me.Clear();
            Game.Opponent = null;
            Game.IsNetwork = false;
            Game.IsServer = false;
            
            Game.GameStatus = GameStatuses.WaitingForSettings;
            Game.turn = 0;
            Game._myturn = -1;



            
        }

        private static int _number_of_planes;
        public static int NumberOfPlanes
        {
            get { return Game._number_of_planes; }
            set
            {
                int old_value = Game._number_of_planes;
                Game._number_of_planes = value;
                if (Game.Opponent != null)
                    Game.Opponent.Clear();
                Game.GameStatus = GameStatuses.WaitingForSettings;
                if(old_value != value)
                    Game.RefreshStates();
            }
        }


        public static void RefreshStates()
        {
            if (Game.NumberOfPlanes == 0
                || Game.Me.Name == null
                || Game.Me.Name.Length == 0
                
                )
            {
                Game.GameStatus = GameStatuses.WaitingForSettings;
            }
            else if (Game.GameStatus == GameStatuses.WaitingForSettings)
            {
                Game.GameStatus = GameStatuses.PlacePlanes;
            }

           

        }

        public static void StopGame()
        {
            Game.abort_place_planes = true;
            for (int i = 0; i < 2; i++)
            {
                
                if (place_plane_threads[i] != null)
                    place_plane_threads[i].Join();
            }
            Game.abort_place_planes = false;

            if (Game.IsNetwork && Game.GameStatus == GameStatuses.GameStarted)
                ((TCPOpponent)Game.Opponent).CurrentShot = 99;

            if(Game.GameStatus != GameStatuses.Disconnected)
                Game.GameStatus = GameStatuses.Finished;

            if (game != null)
                game.Interrupt();
        }
        #endregion

        #region Network Related
        public static bool IsNetwork { get; set; }

        private static byte[] salt = new byte[6];
        private static byte[] my_half;
        private static bool first_half = true;

        public static bool IsServer
        {
            set { Game.first_half = value; }
            get { return Game.first_half; }
        }

        public static byte[] MyHalf
        {
            get
            {
                if (Game.my_half == null)
                {
                    Game.my_half = new byte[3];
                    for (int i = 0; i < 3; i++)
                    {
                        Game.my_half[i] = (byte)Game.Rand.Next(256);
                    }
                }
                return Game.my_half;
            }
        }

        public static byte[] OtherHalf
        {
            set
            {
                if (Game.first_half)
                {
                    Array.Copy(Game.MyHalf,0, Game.salt,0, 3);
                    Array.Copy(value, 0, Game.salt, 3, 3);
                }
                else
                {
                    Array.Copy(value,0, Game.salt,0, 3);
                    Array.Copy(Game.MyHalf, 0, Game.salt, 3, 3);
                }
            }
        }

        public static byte[] Salt { get { return Game.salt; } }

        public static byte[] GetHash(byte[] initial_board)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            byte[] salted_board =
                new byte[100 + salt.Length];

            Array.Copy(initial_board, 0, salted_board, 0, 100);
            Array.Copy(salt, 0, salted_board, 100, salt.Length);

            return algorithm.ComputeHash(salted_board);
        }

        public static void CheckValidity()
        {
            System.Diagnostics.Debug.WriteLine("Checking validity");
            if (Game.IsValid)
                Game.GameStatus = GameStatuses.Finished;
            else
                Game.GameStatus = GameStatuses.NotValid;
        }

        public static bool IsValid
        {
            get
            {
                if (Game.IsNetwork)
                {
                    TCPOpponent opponent = (TCPOpponent)Game.Opponent;
                    byte[] hashed_value = Game.GetHash(opponent.InitialBoard);
                    int index = 0;

                    foreach (byte cell in opponent.Board)
                    {
                        if (cell == 0)
                        {
                            index++;
                            continue;
                        }

                        if ((opponent.InitialBoard[index] | 4) != cell)
                        {
                            return false;
                        }
                        index++;
                    }

                    if (hashed_value.Length != opponent.Hash.Length)
                        return false;

                    for (int i = 0; i < hashed_value.Length; i++)
                    {
                        if (hashed_value[i] != opponent.Hash[i])
                            return false;
                    }
                }
                return true;
            }
        }
        #endregion
    }
}
