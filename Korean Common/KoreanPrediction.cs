using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace KoreanCommon
{
    using System.Diagnostics;

    public static class KoreanPrediction
    {
        static private Vector3 predictedPosition;

        static public void Cast(Spell spell, Obj_AI_Hero target, KoreanPredictionTypes type = KoreanPredictionTypes.Slow)
        {
            if (spell.IsReady())
            {
                Vector3 position = GetPredictedPosition(spell, target, type);
                if (spell.IsInRange(position))
                {
                    spell.Cast(position);
                }
            }
        }

        static private void PrintVector3(Vector3 vector, bool debugging = false)
        {
            if (!debugging)
            {
                Console.WriteLine("X = {0}, Y = {1}, Z = {2}", vector.X, vector.Y, vector.Z);
            }
        }

        static public Vector3 GetPredictedPosition(Spell spell, Obj_AI_Hero target, KoreanPredictionTypes type, bool debug = false)
        {
            Vector3 newPred1 = CheckPredictedPosition(target, spell, debug);
            PrintVector3(newPred1);
            Utility.DelayAction.Add(200, () => predictedPosition = CheckPredictedPosition(target, spell, debug));
            Vector3 newPred2 = predictedPosition;
            PrintVector3(newPred2);

            if (newPred1.Distance(newPred2) < spell.Width)
            {
                return newPred2;
            }
            else
            {
                return Vector3.Zero;
            }
        }

        static private Vector3 CheckPredictedPosition(Obj_AI_Hero target, Spell spell, bool debug)
        {
            Vector3 newPoint = target.ServerPosition;

            if (!target.IsMoving)
            {
                newPoint = target.Position;
            }
            else
            {
                Vector3[] path = target.GetPath(target.GetWaypoints().Last().To3D());

                if (path.Count() <= 1)
                {
                    return Vector3.Zero;
                }

                newPoint = path.Last(p => p.Distance(target.Position) < target.MoveSpeed * spell.Delay);

                int i = 0;
                while ((newPoint.Distance(target.Position) < (target.MoveSpeed * spell.Delay) + (newPoint.Distance(ObjectManager.Player.Position) / spell.Speed)) || i < 10)
                {
                    i++;
                    newPoint = ((newPoint - target.Position) * 1.05f) + target.Position;

                    if (newPoint.Distance(target.Position).Equals(0))
                    {
                        return Vector3.Zero;
                    }
                }
            }

            if (debug)
            {
                Render.Circle.DrawCircle(newPoint, 10, System.Drawing.Color.DarkBlue, 30);
            }

            if ((!debug) && !spell.IsInRange(target) && newPoint.Distance(ObjectManager.Player.Position) > spell.Range - 50)
            {
                newPoint = new Vector3(0, 0, 0);
            }

            return newPoint;
        }
    }
}
