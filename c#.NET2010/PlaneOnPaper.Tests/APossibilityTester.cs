using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using NUnit.Framework;

namespace PlaneOnPaper.Tests
{
    [TestFixture]
    public class APossibilityTester
    {
        [Test]
        public static void ResetPossiblePlanesTest()
        {
            List<ProbablePlane> possibilites = new List<ProbablePlane>();
            APossibilityOpponent.resetPossibilities(possibilites);

            Assert.AreEqual( 168, possibilites.Count );
        }

        [Test]
        public static void RemoveMatchingNotShotProbablePlanesTest()
        {
            List<ProbablePlane> possibilites = new List<ProbablePlane>();
            APossibilityOpponent.resetPossibilities(possibilites);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites, 55, 4);
            Assert.AreEqual(168 - 40, possibilites.Count);
        }

        [Test]
        public static void RemoveMatchingShotProbablePlanesTest()
        {
            List<ProbablePlane> possibilites = new List<ProbablePlane>();
            APossibilityOpponent.resetPossibilities(possibilites);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites, 55, 5);
            Assert.AreEqual(168 - 4, possibilites.Count);
        }

        [Test]
        public static void RemoveMatchingShotDownProbablePlanesTest()
        {
            List<ProbablePlane> possibilites = new List<ProbablePlane>();
            APossibilityOpponent.resetPossibilities(possibilites);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites, 55, 6);
            Assert.AreEqual(168 - 36, possibilites.Count);
        }

        

        [Test]
        public static void CalculateProbablePlaneImprobableTest()
        {
            byte[] actual_field = new byte[]{
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 5, 0, 0, 0, 0, 0,
                0, 0, 6, 4, 0, 0, 0, 0, 0, 0,
                0, 0, 4, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0
            };

            List<ProbablePlane> possibilites = new List<ProbablePlane>();
            APossibilityOpponent.resetPossibilities(possibilites);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites, 2 * 10 + 4, 5);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites, 3 * 10 + 2, 6);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites, 3 * 10 + 3, 4);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites, 4 * 10 + 2, 4);

            Assert.AreEqual(112, possibilites.Count);

            int count = possibilites.Count;

            ProbablePlane pl_to_test = possibilites
                .Where(pl => pl.Direction == (int)PlaneDirections.North
                    && pl.Coordinates == new Point(6, 2))
                .SingleOrDefault();

            Assert.IsNotNull(pl_to_test);

            Assert.AreEqual(0, APossibilityOpponent.evaluateProbablePlane(possibilites, pl_to_test, 2, 2, actual_field));

            foreach (ProbablePlane pl in possibilites)
            {
                Assert.AreEqual(0, pl.Probability);
            }

            pl_to_test = possibilites
                .Where(pl => pl.Direction == (int)PlaneDirections.South
                    && pl.Coordinates == new Point(2, 2))
                .SingleOrDefault();

            Assert.IsNotNull(pl_to_test);
        }

        [Test]
        public static void CalculateProbablePlaneProbableTest()
        {
            byte[] actual_field = new byte[]{
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 5, 0, 0, 0, 0, 0,
                0, 0, 6, 4, 0, 0, 0, 0, 0, 0,
                0, 0, 4, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0
            };

            List<ProbablePlane> possibilites = new List<ProbablePlane>();
            APossibilityOpponent.resetPossibilities(possibilites);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites, 2 * 10 + 4, 5);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites, 3 * 10 + 2, 6);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites, 3 * 10 + 3, 4);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites, 4 * 10 + 2, 4);

            Assert.AreEqual(112, possibilites.Count);

            int count = possibilites.Count;

            ProbablePlane pl_to_test = possibilites
                .Where(pl => pl.Direction == (int)PlaneDirections.North
                    && pl.Coordinates == new Point(7, 7))
                .SingleOrDefault();

            Assert.IsNotNull(pl_to_test);

            Assert.Greater(APossibilityOpponent.evaluateProbablePlane(possibilites, pl_to_test, 2, 2, actual_field), 0);

            foreach (ProbablePlane pl in possibilites)
            {
                Assert.AreEqual(0, pl.Probability);
            }

            pl_to_test = possibilites
                .Where(pl => pl.Direction == (int)PlaneDirections.South
                    && pl.Coordinates == new Point(2, 2))
                .SingleOrDefault();

            Assert.IsNotNull(pl_to_test);
        }


        [Test]
        public void CalculateProbablePlaneDeepTest()
        {
            byte[] actual_field = new byte[]{
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 5, 0, 0, 0, 0, 0,
                0, 0, 6, 4, 0, 0, 0, 0, 0, 0,
                0, 0, 4, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0
            };

            List<ProbablePlane> possibilites = new List<ProbablePlane>();
            APossibilityOpponent.resetPossibilities(possibilites);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites, 2 * 10 + 4, 5);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites, 3 * 10 + 2, 6);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites, 3 * 10 + 3, 4);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites, 4 * 10 + 2, 4);

            Assert.AreEqual(112, possibilites.Count);

            ProbablePlane pl_to_test = possibilites
                .Where(pl => pl.Direction == (int)PlaneDirections.North
                    && pl.Coordinates == new Point(7, 7))
                .SingleOrDefault();

            pl_to_test.Probability = 1;

            APossibilityOpponent.removeOverlappedProbablePlanes(possibilites, pl_to_test);

            pl_to_test = possibilites
                .Where(pl => pl.Direction == (int)PlaneDirections.South
                    && pl.Coordinates == new Point(2, 2))
                .SingleOrDefault();

            Assert.AreEqual(1, APossibilityOpponent.evaluateProbablePlane(possibilites, pl_to_test, 1, 2, actual_field));

            ProbablePlane[] temp = new ProbablePlane[possibilites.Count];
            possibilites.CopyTo(temp);
            foreach (ProbablePlane pl in temp)
            {
                if (pl.Probability < 1)
                {
                    if (APossibilityOpponent.evaluateProbablePlane(possibilites, pl, 1, 2, actual_field) == 0)
                    {
                        possibilites.Remove(pl);
                    }
                }
            }

            int enemies_found = APossibilityOpponent.findUniqueShotsAndIdentifyPlanes(actual_field, possibilites);

            int count_1 = possibilites.Where(pl => pl.Probability == 1).Count();
            Assert.AreEqual(2, count_1);

        }


        [Test]
        public void CalculateProbablePlaneDeepTest2()
        {
            byte[] actual_field = new byte[]{
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 5,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0
            };

            List<ProbablePlane> possibilities = new List<ProbablePlane>();
            APossibilityOpponent.resetPossibilities(possibilities);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilities, 9 + 7 * 10, 5);

            ProbablePlane pl_to_test = possibilities
                .Where(pl => pl.Direction == (int)PlaneDirections.North
                    && pl.Coordinates == new Point(7, 7))
                .SingleOrDefault();

            pl_to_test.Probability = 1;
            APossibilityOpponent.removeOverlappedProbablePlanes(possibilities, pl_to_test);

            pl_to_test = possibilities
                .Where(pl => pl.Direction == (int)PlaneDirections.South
                    && pl.Coordinates == new Point(2, 2))
                .SingleOrDefault();

            Assert.AreEqual(1, APossibilityOpponent.evaluateProbablePlane(possibilities, pl_to_test, 1, 2, actual_field));

            /*
            foreach (ProbablePlane pl in possibilities)
            {
                if (pl.Probability < 1)
                {
                    pl.Probability = APossibilityOpponent.evaluateProbablePlane(possibilities, pl, 1, 2, actual_field);
                }
            }

            int count_1 = possibilities.Where(pl => pl.Probability == 1).Count();
            Assert.AreEqual(2, count_1);
            */

        }

        [Test]
        public void removeOverlappedPlaneTest()
        {
            List<ProbablePlane> possibilites = new List<ProbablePlane>();
            List<ProbablePlane> possibilites2 = new List<ProbablePlane>();
            APossibilityOpponent.resetPossibilities(possibilites);
            APossibilityOpponent.resetPossibilities(possibilites2);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites2, 6 * 10 + 3, 4);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites2, 6 * 10 + 4, 4);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites2, 6 * 10 + 5, 4);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites2, 5 * 10 + 4, 4);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites2, 4 * 10 + 2, 4);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites2, 4 * 10 + 3, 4);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites2, 4 * 10 + 4, 4);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites2, 4 * 10 + 5, 4);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites2, 4 * 10 + 6, 4);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites2, 3 * 10 + 4, 4);

            ProbablePlane pl_to_test = possibilites
                .Where(pl => pl.Direction == (int)PlaneDirections.North
                    && pl.Coordinates == new Point(4, 4))
                .SingleOrDefault();
            APossibilityOpponent.removeOverlappedProbablePlanes(possibilites, pl_to_test);

            Assert.Greater(possibilites.IndexOf(pl_to_test), -1);

            Assert.AreEqual(possibilites.Count - 1, possibilites2.Count);
        }

        [Test]
        public void reevaluatePossibilitiesTest()
        {
            byte[] actual_board = new byte[]{
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0
            };
            List<ProbablePlane> possibilites = new List<ProbablePlane>();
            List<ProbablePlane> possibilites2 = new List<ProbablePlane>();
            APossibilityOpponent.resetPossibilities(possibilites);
            APossibilityOpponent.resetPossibilities(possibilites2);


            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites2, 3 * 10 + 3, 4);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites2, 4 * 10 + 2, 4);

            int count = possibilites2.Count;

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites2, 2 * 10 + 4, 5);

            APossibilityOpponent.removeProbablePlanesNotMatchingCell(possibilites2, 3 * 10 + 2, 6);


            Assert.AreEqual(112, possibilites2.Count);

            ProbablePlane pl_to_test = possibilites2
                .Where(pl => pl.Direction == (int)PlaneDirections.South
                    && pl.Coordinates == new Point(2, 2))
                .SingleOrDefault();

            APossibilityOpponent.removeOverlappedProbablePlanes(possibilites2, pl_to_test);

            actual_board[3 + 3 * 10] = 4;
            APossibilityOpponent.reevaluatePossibilities(possibilites, 4, (3 + 3 * 10), 2, actual_board);

            actual_board[2 + 4 * 10] = 4;
            APossibilityOpponent.reevaluatePossibilities(possibilites, 4, (2 + 4 * 10), 2, actual_board);

            Assert.AreEqual(count, possibilites.Count);

            actual_board[2 + 3 * 10] = 6;
            APossibilityOpponent.reevaluatePossibilities(possibilites, 6, (2 + 3 * 10), 2, actual_board);

            actual_board[4 + 2 * 10] = 5;
            APossibilityOpponent.reevaluatePossibilities(possibilites, 5, (4 + 2 * 10), 2, actual_board);

            //
            Assert.AreEqual(possibilites2.Count, possibilites.Count);
            
        }

        [Test]
        public void reevaluatePossibilitiesTest2()
        {
            byte[] actual_board = new byte[]{
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0
            };
            List<ProbablePlane> possibilites = new List<ProbablePlane>();
            APossibilityOpponent.resetPossibilities(possibilites);


            actual_board[9 + 7 * 10] = 5;
            APossibilityOpponent.reevaluatePossibilities(possibilites, 5, (9 + 7 * 10), 2, actual_board);

            Assert.Greater(possibilites
                .Count(), 0);

            actual_board[7 + 6 * 10] = 6;
            APossibilityOpponent.reevaluatePossibilities(possibilites, 6, (7 + 6 * 10), 2, actual_board);

            Assert.AreEqual(1, possibilites
                .Where(pl => pl.Probability == 1)
                .Count());
            
            actual_board[3 + 7 * 10] = 5;
            APossibilityOpponent.reevaluatePossibilities(possibilites, 5, (3 + 7 * 10), 2, actual_board);

            Assert.AreEqual(1, possibilites.Where(pl => pl.Probability == 1).Count());

            //
            Assert.Greater(possibilites.Count, 5);

        }

        [Test]
        public void findUniqueShotsAndIdentifyPlanesTest()
        {
            byte[] actual_board = new byte[]{
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 5, 0, 0, 0, 0, 0,
                0, 0, 0, 5, 0, 5, 0, 0, 0, 0,
                0, 0, 0, 0, 5, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0
            };
            List<ProbablePlane> possibilites = new List<ProbablePlane>();

            possibilites.Add(new ProbablePlane(Color.Orange, new Point(4, 4), (int)PlaneDirections.South));
            possibilites.Add(new ProbablePlane(Color.Orange, new Point(3, 3), (int)PlaneDirections.South));
            possibilites.Add(new ProbablePlane(Color.Orange, new Point(5, 3), (int)PlaneDirections.South));
            possibilites.Add(new ProbablePlane(Color.Orange, new Point(5, 5), (int)PlaneDirections.South));
            possibilites.Add(new ProbablePlane(Color.Orange, new Point(3, 5), (int)PlaneDirections.South));

            APossibilityOpponent.findUniqueShotsAndIdentifyPlanes(actual_board, possibilites);
            Assert.AreEqual(1, possibilites.Count);
        }
    }

    [TestFixture]
    public class PlaneTester
    {

        [Test]
        public void PlaneFieldTest()
        {
            ProbablePlane pl = new ProbablePlane(Color.Orange, new Point(2, 1), (int)PlaneDirections.North);

            bool val = true;

            if (pl.Fields[2 + 0 * 10] == 0)
            {
                val = false;
            }

            if (pl.Fields[0 + 1 * 10] == 0)
            {
                val = false;
            }
            if (pl.Fields[1 + 1 * 10] == 0)
            {
                val = false;
            }
            if (pl.Fields[2 + 1 * 10] == 0)
            {
                val = false;
            }
            if (pl.Fields[3 + 1 * 10] == 0)
            {
                val = false;
            }
            if (pl.Fields[4 + 1 * 10] == 0)
            {
                val = false;
            }

            if (pl.Fields[2 + 2 * 10] == 0)
            {
                val = false;
            }

            if (pl.Fields[1 + 3 * 10] == 0)
            {
                val = false;
            }
            if (pl.Fields[2 + 3 * 10] == 0)
            {
                val = false;
            }
            if (pl.Fields[3 + 3 * 10] == 0)
            {
                val = false;
            }

            Assert.IsTrue(val);
        }

        [Test]
        public void PlaneCloneTest()
        {
            ProbablePlane pl = new ProbablePlane(Color.Orange, new Point(2, 1), (int)PlaneDirections.North);
            pl.Probability = 0.4F;

            ProbablePlane pl_clone = (ProbablePlane)pl.Clone();
            bool val = true;

            if (pl_clone.Fields[2 + 0 * 10] == 0)
            {
                val = false;
            }

            if (pl_clone.Fields[0 + 1 * 10] == 0)
            {
                val = false;
            }
            if (pl_clone.Fields[1 + 1 * 10] == 0)
            {
                val = false;
            }
            if (pl_clone.Fields[2 + 1 * 10] == 0)
            {
                val = false;
            }
            if (pl_clone.Fields[3 + 1 * 10] == 0)
            {
                val = false;
            }
            if (pl_clone.Fields[4 + 1 * 10] == 0)
            {
                val = false;
            }

            if (pl_clone.Fields[2 + 2 * 10] == 0)
            {
                val = false;
            }

            if (pl_clone.Fields[1 + 3 * 10] == 0)
            {
                val = false;
            }
            if (pl_clone.Fields[2 + 3 * 10] == 0)
            {
                val = false;
            }
            if (pl_clone.Fields[3 + 3 * 10] == 0)
            {
                val = false;
            }

            Assert.IsTrue(val);

            Assert.AreEqual(pl.Direction, pl_clone.Direction);

            Assert.AreEqual(pl.Coordinates, pl_clone.Coordinates);

            Assert.AreEqual(pl.Color, pl_clone.Color);

            Assert.AreEqual(pl.Probability, pl_clone.Probability);

            Assert.IsFalse(Object.ReferenceEquals(pl, pl_clone));
        }

        [Test]
        public void PlaneIsOverlappingTest()
        {
            Plane pl0 = new Plane(Color.Green, 2, 2, (int)PlaneDirections.South);
            Plane pl1 = new Plane(Color.Green, 6, 2, (int)PlaneDirections.South);

            Assert.IsTrue(Plane.isOverlapping(pl0, pl1));

            pl1 = new Plane(Color.Green, 7, 2, (int)PlaneDirections.South);

            Assert.IsFalse(Plane.isOverlapping(pl0, pl1));

            Assert.IsFalse(pl0.isOverlapping(pl1));

            pl1 = new Plane(Color.Green, 6, 2, (int)PlaneDirections.South);

            Assert.IsTrue(pl0.isOverlapping(pl1));
        }
    }
}
;