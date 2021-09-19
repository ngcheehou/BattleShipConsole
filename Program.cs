using BattleShipConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace BattleShipConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            bool isShowShips = false;

            NavyAsset MyNavyAsset = new NavyAsset();
            NavyAsset EnemyNavyAsset = new NavyAsset();



            Dictionary<char, int> Coordinates = PopulateDictionary();
            PrintHeader();
            Console.WriteLine();
            PrintMap(EnemyNavyAsset);

            int Game;
            for (Game = 0; Game < 100; Game++)
            {
                MyNavyAsset.StepsTaken++;


                Position position = new Position();

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Input target.");
                string input = Console.ReadLine();
                position = AnalyzeInput(input, Coordinates);

                if (position.x == -1 || position.y == -1)
                {
                    Console.WriteLine("Invalid coordinates!");
                    Game--;
                    continue;
                }

                if (MyNavyAsset.FirePositions.Any(EFP => EFP.x == position.x && EFP.y == position.y))
                {
                    Console.WriteLine("This coordinate already being shot.");
                    Game--;
                    continue;
                }

                for (int i = 0; i < 2; i++)
                {
                    EnemyNavyAsset.Fire();
                }

                var index = MyNavyAsset.FirePositions.FindIndex(p => p.x == position.x && p.y == position.y);

                if (index == -1)
                    MyNavyAsset.FirePositions.Add(position);

                Clear();
                Console.WriteLine("Fire!!");


                MyNavyAsset.AllPosition.OrderBy(o => o.x).ThenBy(n => n.y).ToList();
                MyNavyAsset.CheckShipStatus(EnemyNavyAsset.FirePositions);

                EnemyNavyAsset.AllPosition.OrderBy(o => o.x).ThenBy(n => n.y).ToList();
                EnemyNavyAsset.CheckShipStatus(MyNavyAsset.FirePositions);

                PrintHeader();
                for (int h = 0; h < 19; h++)
                {
                    Console.Write(" ");
                }

                PrintHeader();
                Console.WriteLine();

                PrintMapAfterAdjust(MyNavyAsset.FirePositions, MyNavyAsset, EnemyNavyAsset, isShowShips);

                Commentator(MyNavyAsset, true);
                Commentator(EnemyNavyAsset, false);
                if (EnemyNavyAsset.IsAllObliterated || MyNavyAsset.IsAllObliterated)
                { break; }

            }

            Console.ForegroundColor = ConsoleColor.White;

            if (EnemyNavyAsset.IsAllObliterated)
            {
                Console.WriteLine("Game Ended, you win.");
            }
            else

            {
                Console.WriteLine("Game Ended, you lose.");
            }

            Console.WriteLine("Total steps taken:{0} ", Game);
            Console.ReadLine();


        }

        static void PrintStatistic(int x, int y, NavyAsset navyAsset, bool isMyBoard)
        {
            if (x == 1 && y == 10)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Indicator:    ");
            }


            if (x == 2 && y == 10)
            {
                if (navyAsset.IsCarrierSunk)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Carrier [5]   ");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("Carrier [5]   ");
                }

            }

            if (x == 3 && y == 10)
            {
                if (navyAsset.IsBattleshipSunk)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Battleship [4]");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("Battleship [4]");
                }
            }

            if (x == 4 && y == 10)
            {

                if (navyAsset.IsDestroyerSunk)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Destroyer [3] ");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("Destroyer [3] ");
                }
            }

            if (x == 5 && y == 10)
            {

                if (navyAsset.IsSubmarineSunk)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Submarine [3] ");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("Submarine [3] ");
                }
            }

            if (x == 6 && y == 10)
            {

                if (navyAsset.IsPatrolBoatSunk)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("PatrolBoat [2]");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("PatrolBoat [2]");
                }

            }


            if (x > 6 && y == 10)
            {
                for (int i = 0; i < 14; i++)
                {
                    Console.Write(" ");
                }
            }



        }

        static void PrintMapAfterAdjust(List<Position> positions, NavyAsset MyNavyAsset, NavyAsset EnemyMyNavyAsset, bool showEnemyShips)
        {

            if (!showEnemyShips)
                showEnemyShips = MyNavyAsset.IsAllObliterated;

            Random random = new Random();
            List<Position> SortedLFirePositions = positions.OrderBy(o => o.x).ThenBy(n => n.y).ToList();
            List<Position> SortedLShipsPositions = EnemyMyNavyAsset.AllPosition.OrderBy(o => o.x).ThenBy(n => n.y).ToList();

            SortedLShipsPositions = SortedLShipsPositions.Where(FP => !SortedLFirePositions.Exists(ShipPos => ShipPos.x == FP.x && ShipPos.y == FP.y)).ToList();


            int hitCounter = 0;
            int EnemyshipCounter = 0;
            int myShipCounter = 0;
            int enemyHitCounter = 0;

            char row = 'A';
            try
            {
                for (int x = 1; x < 11; x++)
                {
                    for (int y = 1; y < 11; y++)
                    {
                        bool keepGoing = true;

                        #region row indicator
                        if (y == 1)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write("[" + row + "]");
                            row++;
                        }
                        #endregion


                        if (SortedLFirePositions.Count != 0 && SortedLFirePositions[hitCounter].x == x && SortedLFirePositions[hitCounter].y == y)
                        {

                            if (SortedLFirePositions.Count - 1 > hitCounter)
                                hitCounter++;

                            if (EnemyMyNavyAsset.AllPosition.Exists(ShipPos => ShipPos.x == x && ShipPos.y == y))
                            {

                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("[*]");

                                //PrintStatistic(x, y, navyAsset,true);
                                keepGoing = false;
                                //continue;

                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Black;
                                Console.Write("[X]");

                                //PrintStatistic(x, y, navyAsset, true);
                                keepGoing = false;
                                //continue;

                            }

                        }

                        if (keepGoing && showEnemyShips && SortedLShipsPositions.Count != 0 && SortedLShipsPositions[EnemyshipCounter].x == x && SortedLShipsPositions[EnemyshipCounter].y == y)

                        {

                            if (SortedLShipsPositions.Count - 1 > EnemyshipCounter)
                                EnemyshipCounter++;

                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.Write("[O]");

                            // PrintStatistic(x, y, navyAsset, true);
                            keepGoing = false;
                            //continue;

                        }

                        if (keepGoing)
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write("[~]");
                        }


                        PrintStatistic(x, y, MyNavyAsset, true);



                        if (y == 10)
                        {
                            Console.Write("      ");

                            PrintMapAfterAdjustEnemy(x, row, MyNavyAsset, EnemyMyNavyAsset, ref myShipCounter, ref enemyHitCounter);
                        }

                    }





                    Console.WriteLine();
                }

            }
            catch (Exception e)
            {
                string error = e.Message.ToString();
            }
        }

        static void PrintMapAfterAdjustEnemy(int x, char row, NavyAsset MyNavyAsset, NavyAsset EnemyNavyAsset, ref int MyshipCounter, ref int EnemyHitCounter)
        {
            List<Position> EnemyFirePositions = new List<Position>();
            row--;
            Random random = new Random();
            List<Position> SortedLFirePositions = EnemyNavyAsset.FirePositions.OrderBy(o => o.x).ThenBy(n => n.y).ToList();
            List<Position> SortedLShipsPositions = MyNavyAsset.AllPosition.OrderBy(o => o.x).ThenBy(n => n.y).ToList();

            SortedLShipsPositions = SortedLShipsPositions.Where(FP => !SortedLFirePositions.Exists(ShipPos => ShipPos.x == FP.x && ShipPos.y == FP.y)).ToList();





            try
            {

                for (int y = 1; y < 11; y++)
                {
                    bool keepGoing = true;

                    #region row indicator
                    if (y == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("[" + row + "]");
                        row++;
                    }
                    #endregion


                    if (SortedLFirePositions.Count != 0 && SortedLFirePositions[EnemyHitCounter].x == x && SortedLFirePositions[EnemyHitCounter].y == y)
                    {

                        if (SortedLFirePositions.Count - 1 > EnemyHitCounter)
                            EnemyHitCounter++;

                        if (MyNavyAsset.AllPosition.Exists(ShipPos => ShipPos.x == x && ShipPos.y == y))
                        {

                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("[*]");

                            //PrintStatistic(x, y, navyAsset,true);
                            keepGoing = false;
                            //continue;

                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write("[X]");

                            //PrintStatistic(x, y, navyAsset, true);
                            keepGoing = false;
                            //continue;

                        }

                    }

                    if (keepGoing && SortedLShipsPositions.Count != 0 && SortedLShipsPositions[MyshipCounter].x == x && SortedLShipsPositions[MyshipCounter].y == y)

                    {

                        if (SortedLShipsPositions.Count - 1 > MyshipCounter)
                            MyshipCounter++;

                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write("[O]");

                        // PrintStatistic(x, y, navyAsset, true);
                        keepGoing = false;
                        //continue;

                    }

                    if (keepGoing)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("[~]");
                    }


                    PrintStatistic(x, y, EnemyNavyAsset, true);

                }


                // Console.WriteLine();


            }
            catch (Exception e)
            {
                string error = e.Message.ToString();
            }
        }

        static Position AnalyzeInput(string input, Dictionary<char, int> Coordinates)
        {
            Position pos = new Position();

            char[] inputSplit = input.ToUpper().ToCharArray();
            string[] inputSplit2 = input.Split();



            if (inputSplit.Length < 2 || inputSplit.Length > 4)
            {
                return pos;
            }




            if (Coordinates.TryGetValue(inputSplit[0], out int value))
            {
                pos.x = value;
            }
            else
            {
                return pos;
            }


            if (inputSplit.Length == 3)
            {

                if (inputSplit[1] == '1' && inputSplit[2] == '0')
                {
                    pos.y = 10;
                    return pos;
                }
                else
                {
                    return pos;
                }

            }


            if (inputSplit[1] - '0' > 9)
            {
                return pos;
            }
            else
            {
                pos.y = inputSplit[1] - '0';
            }

            return pos;
        }

        static void PrintHeader()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("[ ]");
            for (int i = 1; i < 11; i++)
                Console.Write("[" + i + "]");


        }

        static void PrintMap(NavyAsset navyAsset)
        {
            Random random = new Random();


            char row = 'A';

            for (int i = 1; i < 11; i++)
            {


                for (int y = 1; y < 11; y++)
                {
                    #region row indicator
                    if (y == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("[" + row + "]");
                        row++;
                    }
                    #endregion

                    if (random.Next(1, 100) > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("[~]");
                    }
                    else if (random.Next(0, 100) > 20)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write("[O]");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write("[*]");
                    }
                    PrintStatistic(i, y, navyAsset, true);

                }
                Console.WriteLine();

            }
        }

        static Dictionary<char, int> PopulateDictionary()
        {
            Dictionary<char, int> Coordinate =
                     new Dictionary<char, int>
                     {
                         { 'A', 1 },
                         { 'B', 2 },
                         { 'C', 3 },
                         { 'D', 4 },
                         { 'E', 5 },
                         { 'F', 6 },
                         { 'G', 7 },
                         { 'H', 8 },
                         { 'I', 9 },
                         { 'J', 10 }
                     };

            return Coordinate;
        }

        static void Commentator(NavyAsset navyAsset, bool isMyShip)
        {

            string title = isMyShip ? "Your" : "Enemy";

            if (navyAsset.CheckPBattleship && navyAsset.IsBattleshipSunk)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("{0} {1} is sunk", title, nameof(navyAsset.Battleship));
                navyAsset.CheckPBattleship = false;
            }

            if (navyAsset.CheckCarrier && navyAsset.IsCarrierSunk)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("{0} {1} is sunk", title, nameof(navyAsset.Carrier));
                navyAsset.CheckCarrier = false;
            }

            if (navyAsset.CheckDestroyer && navyAsset.IsDestroyerSunk)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("{0} {1} is sunk", title, nameof(navyAsset.Destroyer));
                navyAsset.CheckDestroyer = false;
            }

            if (navyAsset.CheckPatrolBoat && navyAsset.IsPatrolBoatSunk)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("{0} {1} is sunk", title, nameof(navyAsset.PatrolBoat));
                navyAsset.CheckPatrolBoat = false;
            }

            if (navyAsset.CheckSubmarine && navyAsset.IsSubmarineSunk)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("{0} {1} is sunk", title, nameof(navyAsset.Submarine));
                navyAsset.CheckSubmarine = false;
            }


            // navyAsset.IsBattleshipSunk

        }
    }
}
