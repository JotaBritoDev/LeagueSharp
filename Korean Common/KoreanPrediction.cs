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
    public static class KoreanPrediction
    {
        static private Vector3 predictedPosition;
        static private Vector3 zeroVector = new Vector3(0, 0, 0);

        static public void Cast(Spell spell, Obj_AI_Hero target, KoreanPredictionTypes type = KoreanPredictionTypes.Slow)
        {
            if (spell.IsReady())
            {
                Vector3 predictedPosition = GetPredictedPosition(spell, target, type);
                if (spell.IsInRange(predictedPosition))
                {
                    spell.Cast(predictedPosition);
                }
            }
        }

        static public Vector3 GetPredictedPosition(Spell spell, Obj_AI_Hero target, KoreanPredictionTypes type, bool debug = false)
        {
            Vector3 newPred1 = CheckPredictedPosition(target, spell, debug);
            Utility.DelayAction.Add(150, () => predictedPosition = CheckPredictedPosition(target, spell, debug));
            Vector3 newPred2 = predictedPosition;

            if (type == KoreanPredictionTypes.Fast)
            {
                if (newPred1.Distance(newPred2) < spell.Width)
                {
                    return newPred2;
                }
                else
                {
                    return zeroVector;
                }
            }

            Utility.DelayAction.Add(150, () => predictedPosition = CheckPredictedPosition(target, spell, debug));
            Vector3 newPred3 = predictedPosition;
            Utility.DelayAction.Add(150, () => predictedPosition = CheckPredictedPosition(target, spell, debug));
            Vector3 newPred4 = predictedPosition;
            Utility.DelayAction.Add(50, () => predictedPosition = CheckPredictedPosition(target, spell, debug));
            Vector3 newPred5 = predictedPosition;

            if (type == KoreanPredictionTypes.Medium)
            {
                if (newPred1.Distance(newPred2) < spell.Width
                    && newPred2.Distance(newPred3) < spell.Width
                    && newPred3.Distance(newPred4) < spell.Width
                    && newPred4.Distance(newPred5) < spell.Width)
                {
                    return newPred5;
                }
                else
                {
                    return zeroVector;
                }
            }

            Utility.DelayAction.Add(150, () => predictedPosition = CheckPredictedPosition(target, spell, debug));
            Vector3 newPred6 = predictedPosition;
            Utility.DelayAction.Add(100, () => predictedPosition = CheckPredictedPosition(target, spell, debug));
            Vector3 newPred7 = predictedPosition;
            Utility.DelayAction.Add(150, () => predictedPosition = CheckPredictedPosition(target, spell, debug));
            Vector3 newPred8 = predictedPosition;
            Utility.DelayAction.Add(100, () => predictedPosition = CheckPredictedPosition(target, spell, debug));
            Vector3 newPred9 = predictedPosition;
            Utility.DelayAction.Add(150, () => predictedPosition = CheckPredictedPosition(target, spell, debug));
            Vector3 newPred10 = predictedPosition;

            if (type == KoreanPredictionTypes.Slow)
            {
                if (newPred1.Distance(newPred2) < spell.Width
                    && newPred2.Distance(newPred3) < spell.Width
                    && newPred3.Distance(newPred4) < spell.Width
                    && newPred4.Distance(newPred5) < spell.Width
                    && newPred5.Distance(newPred6) < spell.Width
                    && newPred6.Distance(newPred7) < spell.Width
                    && newPred7.Distance(newPred8) < spell.Width
                    && newPred8.Distance(newPred9) < spell.Width
                    && newPred9.Distance(newPred10) < spell.Width)
                {
                    Console.WriteLine("PERFECT HIT!");
                    return newPred10;
                }
                else
                {
                    Console.WriteLine("PERFECT ======");
                    return zeroVector;
                }
            }

            return zeroVector;
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
                    return new Vector3(0, 0, 0);
                }

                newPoint = path.Last(p => p.Distance(target.Position) < target.MoveSpeed * spell.Delay);

                if (newPoint == null)
                {
                    return new Vector3(0, 0, 0);
                }

                int i = 0;
                while ((newPoint.Distance(target.Position) < (target.MoveSpeed * spell.Delay) + (newPoint.Distance(ObjectManager.Player.Position) / spell.Speed)) || i < 10)
                {
                    i++;
                    newPoint = ((newPoint - target.Position) * 1.05f) + target.Position;

                    if (newPoint.Distance(target.Position) == 0)
                    {
                        return new Vector3(0, 0, 0);
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
