using System;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using Collision = SharpDX.Collision;

namespace Ezreal
{
    internal class Ezreal
    {
        private static Spell.Skillshot _q;
        private static Spell.Skillshot _w;
        private static Spell.Skillshot _r;
        private static Menu _ezrealMenu, _comboMenu, _harassMenu, _drawMenu, _laneClearMenu, _jungleClearMenu;

        public static void OnLoad()
        {
            if (Player.Instance.ChampionName != "Ezreal")
            {
                return;
            }
            TargetSelector.Init();
            Bootstrap.Init(null);
            //SkillShots
            _q = new Spell.Skillshot(SpellSlot.Q, 1190, SkillShotType.Linear, 250, null, 60);
            _w = new Spell.Skillshot(SpellSlot.W, 990, SkillShotType.Linear, 250, null, 80);
            _r = new Spell.Skillshot(SpellSlot.R, 2000, SkillShotType.Linear, 1, null, 160);

            //Menus
            _ezrealMenu = MainMenu.AddMenu("Ezreal", "Ezreal");
            _ezrealMenu.AddGroupLabel("Ezreal 1.0");
            _ezrealMenu.AddSeparator();
            _ezrealMenu.AddLabel("Made by xxx");

            _comboMenu = _ezrealMenu.AddSubMenu("Combo", "Combo");
            _comboMenu.Add("combo.q", new CheckBox("Use Q"));
            _comboMenu.Add("combo.w", new CheckBox("Use W"));
            _comboMenu.Add("combo.r", new CheckBox("Use R"));

            _harassMenu = _ezrealMenu.AddSubMenu("Harass", "Harass");
            _harassMenu.Add("Harass.q", new CheckBox("Use Q"));
            _harassMenu.Add("Harass.w", new CheckBox("Use W"));
            _harassMenu.Add("Harass.e", new CheckBox("Use E"));

            _laneClearMenu = _ezrealMenu.AddSubMenu("Laneclear", "Laneclear");
            _laneClearMenu.Add("laneclear.q", new CheckBox("Use Q"));
            _laneClearMenu.Add("laneclear.r", new CheckBox("Use R"));

            _jungleClearMenu = _ezrealMenu.AddSubMenu("Jungleclear", "Jungleclear");
            _jungleClearMenu.Add("jungleclear.q", new CheckBox("Use Q"));
            _jungleClearMenu.Add("jungleclear.w", new CheckBox("Use W"));
            _jungleClearMenu.Add("jungleclear.e", new CheckBox("Use E"));

            _drawMenu = _ezrealMenu.AddSubMenu("Draws", "Draws");
            _harassMenu.Add("draw.q", new CheckBox("Draw Q"));
            _harassMenu.Add("draw.w", new CheckBox("Draw W"));
            _harassMenu.Add("draw.e", new CheckBox("Draw E"));

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += OnDraw;
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo(false);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Combo(true);
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (!ObjectManager.Player.IsDead)
            {
                return;
            }
            var drawQ = _drawMenu["draw.q"].Cast<CheckBox>().CurrentValue;
            var drawW = _drawMenu["draw.w"].Cast<CheckBox>().CurrentValue;

            if (drawQ && _q.IsReady())
            {
                new Circle { Color = Color.Red, BorderWidth = 1, Radius = _q.Range }.Draw(ObjectManager.Player.Position);
            }
            if (drawW && _w.IsReady())
            {
                new Circle { Color = Color.Black, BorderWidth = 1, Radius = _w.Range }.Draw(ObjectManager.Player.Position);
            }
        }

        private static void Combo(bool harass)
        {
            var useQ = _comboMenu["combo.q"].Cast<CheckBox>().CurrentValue;
            var useW = _comboMenu["combo.w"].Cast<CheckBox>().CurrentValue;
            var useR = _comboMenu["combo.r"].Cast<CheckBox>().CurrentValue;
            var target = TargetSelector.GetTarget(_r.Range, DamageType.Magical);
            if (!harass)
            {
                if (useR && _r.IsReady() && target != null && !target.IsDead && !target.IsZombie && _r.GetPrediction(target).HitChance >= HitChance.High)
                {
                    _r.Cast(target.ServerPosition);
                }
            }
            if (useQ && _q.IsReady() && !target.IsDead && !target.IsZombie && _q.GetPrediction(target).HitChance >= HitChance.High && target.IsValidTarget(_q.Range))
            {
                _q.Cast(target.ServerPosition);
            }
            if (useW && _w.IsReady() && !target.IsDead && !target.IsZombie && _w.GetPrediction(target).HitChance >= HitChance.High && target.IsValidTarget(_w.Range))
            {
                _w.Cast(target.ServerPosition);
            }
        }

        private static void LaneClear()
        {
            var qUse = _laneClearMenu["laneclear.q"].Cast<CheckBox>().CurrentValue;
            var rUse = _laneClearMenu["laneclear.r"].Cast<CheckBox>().CurrentValue;
            var minion =
                ObjectManager.Get<Obj_AI_Minion>()
                    .FirstOrDefault(
                        x =>
                            x.IsValidTarget(_q.Range) && !x.IsDead &&
                            ObjectManager.Player.GetSpellDamage(x, SpellSlot.Q) >= x.Health);
            var minionlocation = Prediction.Position.PredictLinearMissile(minion, 1190, 250, 0, 60, 0);
            var minionsHit = minionlocation.CollisionObjects.Count(x => x.IsMinion);
            if (qUse && _q.IsReady())
            {
                if (minion != null) _q.Cast(minionlocation.CastPosition);
            }
            if (rUse && _r.IsReady() && minionsHit >= 8)
            {
                if (minion != null) _r.Cast(minionlocation.CastPosition);
            }
        }

    }
}