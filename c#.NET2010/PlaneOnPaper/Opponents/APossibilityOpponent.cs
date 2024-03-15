using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using System.Drawing;

namespace PlaneOnPaper
{
    public abstract class APossibilityOpponent : AOpponent
    {
        public APossibilityOpponent()
            : base()
        {
            this._possible_enemy_planes = new List<ProbablePlane>();
            resetPossibleEnemyPlanes();
        }

        public override bool PlacePlanes(int number_of_planes)
        {
            this._enemies_left = number_of_planes;
            return true;
        }

        public override void Clear()
        {
            base.Clear();
            resetPossibleEnemyPlanes();
        }

        //possible planes
        private List<ProbablePlane> _possible_enemy_planes;
        public ICollection<Plane> PossibleEnemyPlanes
        {
            get { return this._possible_enemy_planes.ConvertAll(x => (Plane)x); }
        }
        public List<ProbablePlane> ProbablePlanes
        {
            get { return this._possible_enemy_planes; }
            set { this._possible_enemy_planes = value; }
        }

        private void resetPossibleEnemyPlanes()
        {
            resetPossibilities(this._possible_enemy_planes);
        }

        public static void resetPossibilities(IList possibilites)
        {
            possibilites.Clear();

            for (int y = 1; y < 8; y++)
            {
                for (int x = 2; x < 8; x++)
                    possibilites.Add(new ProbablePlane(Color.Orange, x, y, (int)PlaneDirections.North));
            }

            for (int y = 2; y < 9; y++)
            {
                for (int x = 2; x < 8; x++)
                    possibilites.Add(new ProbablePlane(Color.Orange, x, y, (int)PlaneDirections.South));
            }

            for (int y = 2; y < 8; y++)
            {
                for (int x = 1; x < 8; x++)
                    possibilites.Add(new ProbablePlane(Color.Orange, x, y, (int)PlaneDirections.West));
            }

            for (int y = 2; y < 8; y++)
            {
                for (int x = 2; x < 9; x++)
                    possibilites.Add(new ProbablePlane(Color.Orange, x, y, (int)PlaneDirections.East));
            }
        }

        private int _enemies_left;
        public static void reevaluatePossibilities(List<ProbablePlane> possible_enemy_planes, byte shot, int index, int total_planes, byte[] actual_board)
        {
#if DEBUG
            String msg = "reevaluatePossibilities";
            //System.Diagnostics.Debug.WriteLine(msg);
#endif

            int enemies_left = total_planes;
            foreach (ProbablePlane pl in possible_enemy_planes)
            {
                if (pl.Probability == 1)//Хэрвээ онгоц тодорхой байвал
                {
                    enemies_left--; //үлдэгдэл дайсны тоог багасгах
                }
            }

            int probable_plane_shot_count = 0;
            ProbablePlane last_shot_pl = null;
            removeProbablePlanesNotMatchingCell(possible_enemy_planes, index, shot); //тухайн буудалтнаас болж боломжит онгоцнуудыг хасах 
            foreach (ProbablePlane pl in possible_enemy_planes) //бүх байж болох онгоцуудын
            {
                if (pl.Fields[index] == (shot & 3) && shot != 4)//хэрвээ тухайн буудалт нь тухайн шалгагдаж байгаа онгоцыг оносон болон хоосон буудалт байгаагүй бол
                {
                    if (pl.Probability < 1) //хэрвээ шалгагдаж байгаа онгоц тодорхой бус бол
                    {//магагдал тооцох
                        if (pl.Probability < 1 && shot > 4 && shot < 7 && pl.Fields[index] != shot) //хэрвээ тодорхой бус болон оносон буудалт болон өмнө буудаж байгаагүй газар бол
                        {
                            if (pl.Probability == 0)
                                pl.Probability = .4F;
                            if (shot == 5)
                                pl.Probability += .6F * 0.07F;
                            else if (shot == 6)
                                pl.Probability += .6F * 0.36F;
                        }
                    }
                    pl.Fields[index] = shot; //онолт
                    probable_plane_shot_count++; //тухайн буудалтанд оногдсон онгоцны тоог нэгээр ахь
                    last_shot_pl = pl; //тухайн буудалтанд оногдсон сүүлийн онгоцыг бүртгэ
                }
            }

            if (probable_plane_shot_count == 1 && last_shot_pl != null)//хэрвээ оновчтой буудалтанд оногдсон онгоц нэг л байвал
            {
                last_shot_pl.Probability = 1;
                removeOverlappedProbablePlanes(possible_enemy_planes, last_shot_pl); //remove overlapping probable planes
                enemies_left--;
            }

            enemies_left -= findUniqueShotsAndIdentifyPlanes(actual_board, possible_enemy_planes);
            //*
            if (enemies_left > 0)
            {
                ProbablePlane[] old_list = new ProbablePlane[possible_enemy_planes.Count];
                possible_enemy_planes.CopyTo(old_list, 0);
#if DEBUG
                int plane_num = 0;
#endif
                foreach (ProbablePlane pl in old_list)
                {
                    if (pl.Probability < 1)
                    {
#if DEBUG
                        //System.Diagnostics.Debug.WriteLine("evaluating plane: " + plane_num++);
#endif
                        pl.BeingEvaluated = true;
                        if (evaluateProbablePlane(possible_enemy_planes, pl, enemies_left, total_planes, actual_board) == 0)
                        {
                            possible_enemy_planes.Remove(pl);
                        }
                        pl.BeingEvaluated = false;
                    }
                }
            }
            //*/
            possible_enemy_planes.Sort();
        }

        public static int findUniqueShotsAndIdentifyPlanes(byte[] actual_board, IList<ProbablePlane> possible_enemy_planes){
            int ret_val = 0;
            for (int i = 0; i < actual_board.Length; i++) { //бүх нүдийг шалга
                if (actual_board[i] > 4) { //хэрвээ хоосон биш нүд байвал
                    int count = 0;
                    ProbablePlane last_known = null;
                    foreach (ProbablePlane pl in possible_enemy_planes) {
                        if((pl.Fields[i] & 3) == (actual_board[i] & 3)) {
                            if (++count > 1)   {
                                break;
                            }
                            last_known = pl;
                        }
                    }

                    if (count == 1 && last_known.Probability<1)
                    {
                        ret_val++;
                        last_known.Probability = 1;
                        removeOverlappedProbablePlanes(possible_enemy_planes, last_known); //remove overlapping probable planes
                    }
                }
            }
            return ret_val;
        }

        public static void removeProbablePlanesNotMatchingCell(List<ProbablePlane> possible_enemy_planes, int index, int cell)
        {
            ProbablePlane[] temp = new ProbablePlane[possible_enemy_planes.Count];
            possible_enemy_planes.CopyTo(temp);
            foreach(ProbablePlane pl in temp)
            {
                if(Array.IndexOf(pl.OccupiedCells, index) > -1 && (cell & 3) != (pl.Fields[index] & 3))
                {
                    possible_enemy_planes.Remove(pl);
                }
            }
            temp = null;
        }

        public static void removeOverlappedProbablePlanes(IList<ProbablePlane> possible_enemy_planes, ProbablePlane probable_plane)
        {
            ProbablePlane[] old_list = new ProbablePlane[possible_enemy_planes.Count];
            possible_enemy_planes.CopyTo(old_list, 0);
            foreach (ProbablePlane pl in old_list) //бүх байж болох онгоцуудын
            {
                if (pl != probable_plane)
                {
                    foreach (int index in probable_plane.OccupiedCells) //тодорхой болсон онгоцын бүх цэгүүд дээр
                    {    
                        if (Array.IndexOf(pl.OccupiedCells, index) > -1)
                        {
                            possible_enemy_planes.Remove(pl);
                        }
                    }
                }
            }
            old_list = null;
        }

        public static float evaluateProbablePlane(List<ProbablePlane> existing_possible_enemy_planes, ProbablePlane probable_plane, int enemies_left, int total_planes, byte[] actual_board)
        {
            List<ProbablePlane> new_list = existing_possible_enemy_planes.Clone<ProbablePlane>();
#if DEBUG
            String msg = "evaluateProbablePlane";
            String ret_msg = "return ";
            //System.Diagnostics.Debug.WriteLine(msg.PadLeft(msg.Length + 2 * (total_planes - enemies_left), ' ') + ": pl(" + probable_plane.Coordinates.X + "," + probable_plane.Coordinates.Y + "), dir: "+probable_plane.Direction+" count: "+new_list.Count);
#endif

            ProbablePlane probable_plane_copy = new_list
                .Where(pl => pl.Direction == probable_plane.Direction
                && pl.Coordinates == probable_plane.Coordinates)
                .SingleOrDefault();


            probable_plane_copy.Probability = 1;//assume probable plane is 100 percent
            removeOverlappedProbablePlanes(new_list, probable_plane_copy); //remove overlapping probable planes
            enemies_left--;
            


            if (total_planes > new_list.Count)
            {
                return 0;
            }

            
            if (enemies_left == 0)
            { //аль ч онгоцонд ноогдоогүй шарх болон сөнөсөн байгаа эсэхийг шалгах
                for (int index = 0; index < actual_board.Length; index++) //бүх нүдний
                {
                    if (actual_board[index] == 5 || actual_board[index] == 6) //шарх эсвэл сөнөсөн байвал
                    {
                        bool is_valid_cell = false;
                        foreach (ProbablePlane pl in new_list) //бүх боломжит онгоцны
                        {
                            if (pl.Probability == 1 && Array.IndexOf(pl.OccupiedCells, index) > -1 && (actual_board[index] & 3) == (pl.Fields[index] & 3))
                            {
                                is_valid_cell = true;
                                break;
                            }
                        }

                        if (!is_valid_cell) //нэг оногдсон нүд (шарх, сөнөсөн) буруу буюу байж болох онгоцнуудад аль ч онгоцонд хамаарагдахгүй
                        {
#if DEBUG
                            //System.Diagnostics.Debug.WriteLine(ret_msg.PadLeft(ret_msg.Length + 2 * (total_planes - enemies_left), ' ')+"0; //invalid cell");
#endif
                            return 0;
                        }
                    }
                }
#if DEBUG
                //System.Diagnostics.Debug.WriteLine(ret_msg.PadLeft(ret_msg.Length + 2 * (total_planes - enemies_left), ' ') + 1 + "; valid cell");
#endif
                return 1;
            }
            else
            {
                foreach (ProbablePlane pl in new_list)
                {
                    if (pl.Probability < 1)
                    {
                        float is_valid_pl = 0;
                        if ((is_valid_pl = evaluateProbablePlane(new_list, pl, enemies_left, total_planes, actual_board)) > 0)
                        {
#if DEBUG
                            //System.Diagnostics.Debug.WriteLine(ret_msg.PadLeft(ret_msg.Length + 2 * (total_planes - enemies_left), ' ') + is_valid_pl);
#endif
                            return is_valid_pl; //TODO evaluate probability;
                        }
                    }
                }

                new_list.Clear();
                new_list = null;

#if DEBUG
                //System.Diagnostics.Debug.WriteLine(ret_msg.PadLeft(ret_msg.Length + 2 * (total_planes - enemies_left), ' ') + '0');
#endif
                return 0;
            }

        }
    }

    static class Extensions
    {
        public static List<T> Clone<T>(this List<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
    }
}
