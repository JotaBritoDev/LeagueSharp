namespace KoreanZed
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using System.Linq;
    using System;
    using System.Collections.Generic;

    using KoreanZed.Enumerators;

    class ZedShadows
    {
        private readonly ZedMenu zedMainMenu;

        private readonly ZedSpell q;

        private readonly ZedSpell w;

        private readonly ZedSpell e;

        public bool CanCast
        {
            get
            {
                return (!ObjectManager.Player.HasBuff("zedwhandler") 
                        && Game.Time > lastTimeCast + 0.3F
                        && Game.Time > buffTime + 1F);
            }
        }

        private float lastTimeCast;

        private float buffTime;

        public ZedShadows(ZedMenu mainMenu, ZedSpells spells)
        {
            zedMainMenu = mainMenu;
            q = spells.Q;
            w = spells.W;
            e = spells.E;

            Game.OnUpdate += Game_OnUpdate;
        }

        private void Game_OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.HasBuff("zedwhandler"))
            {
                buffTime = Game.Time;
            }
        }

        public void Cast(Obj_AI_Hero target)
        {
            if (target == null)
            {
                return;
            }

            int currentShadows = GetShadows().Count();
            if (CanCast && w.IsReady() && w.Instance.ToggleState == 0
                && !ObjectManager.Player.HasBuff("zedwhandler")
                && ((ObjectManager.Player.HasBuff("zedr2") && currentShadows == 1) || currentShadows == 0))
            {
                w.Cast(target.Position);
                lastTimeCast = Game.Time;
            }
        }

        public List<Obj_AI_Base> GetShadows()
        {
            List<Obj_AI_Base> resultList = new List<Obj_AI_Base>();
            foreach (
                Obj_AI_Base objAiBase in
                    ObjectManager.Get<Obj_AI_Base>().Where(obj => obj.SkinName.ToLowerInvariant().Contains("shadow") && !obj.IsDead))
            {
                resultList.Add(objAiBase);
            }
            return resultList;
        }

        public void Harass()
        {
            List<Obj_AI_Base> shadows = GetShadows();

            if (!shadows.Any() 
                || (!q.UseOnHarass && !e.UseOnHarass)
                || (!q.IsReady() && !e.IsReady()))
            {
                return;
            }
           
            List<Obj_AI_Hero> blackList = zedMainMenu.GetBlockList(BlockListType.Harass);

            foreach (Obj_AI_Base objAiBase in shadows)
            {
                if (((q.UseOnHarass && !q.IsReady()) || !q.UseOnHarass)
                    && ((e.UseOnHarass && !e.IsReady()) || !e.UseOnHarass))
                {
                    break;
                }

                if (q.UseOnHarass && q.IsReady())
                {
                    Obj_AI_Hero target = TargetSelector.GetTarget(
                        q.Range,
                        q.DamageType,
                        true,
                        blackList,
                        objAiBase.Position);

                    if (target != null)
                    {
                        PredictionInput predictionInput = new PredictionInput();
                        predictionInput.Range = q.Range;
                        predictionInput.RangeCheckFrom = objAiBase.Position;
                        predictionInput.From = objAiBase.Position;
                        predictionInput.Delay = q.Delay;
                        predictionInput.Speed = q.Speed;
                        predictionInput.Unit = target;
                        predictionInput.Type = SkillshotType.SkillshotLine;
                        predictionInput.Collision = false;

                        PredictionOutput predictionOutput = Prediction.GetPrediction(predictionInput);

                        if (predictionOutput.Hitchance >= HitChance.High)
                        {
                            q.Cast(predictionOutput.CastPosition);
                        }
                    }
                }

                if (e.UseOnHarass && e.UseOnHarass)
                {
                    Obj_AI_Hero target = TargetSelector.GetTarget(e.Range, e.DamageType, true, blackList, objAiBase.Position);

                    if (target != null)
                    {
                        e.Cast();
                    }
                }
            }
        }
    }
}
