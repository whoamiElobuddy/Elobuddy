
using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using Color = System.Drawing.Color;
namespace Ezreal
{
    class Ezreal
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
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
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
                new Circle() { Color = Color.Red, BorderWidth = 1, Radius = _q.Range }.Draw(ObjectManager.Player.Position);
            }
            if (drawW && _w.IsReady())
            {
                new Circle() { Color = Color.Black, BorderWidth = 1, Radius = _w.Range }.Draw(ObjectManager.Player.Position);
            }
        }

        private static void Harass()
        {
            var useQ = _comboMenu["combo.q"].Cast<CheckBox>().CurrentValue;
            var useW = _comboMenu["combo.w"].Cast<CheckBox>().CurrentValue;
            if (useQ && _q.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(_q.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (_q.GetPrediction(target).HitChance >= HitChance.High)
                    {
                        _q.Cast(target);
                    }
                }
            }
            if (useW && _w.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(_w.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (_w.GetPrediction(target).HitChance >= HitChance.High)
                    {
                        _w.Cast(target);
                    }
                }
            }
        }

        private static void Combo()
        {
            var useQ = _comboMenu["combo.q"].Cast<CheckBox>().CurrentValue;
            var useW = _comboMenu["combo.w"].Cast<CheckBox>().CurrentValue;
            var useR = _comboMenu["combo.r"].Cast<CheckBox>().CurrentValue;

            if (useQ && _q.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(_q.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (_q.GetPrediction(target).HitChance >= HitChance.High)
                    {
                        _q.Cast(target);
                    }
                }
            }
            if (useW && _w.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(_w.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (_w.GetPrediction(target).HitChance >= HitChance.High)
                    {
                        _w.Cast(target);
                    }
                }
            }
            if (useR && _r.IsReady())
            {
                foreach (var target in HeroManager.Enemies.Where(x => x.IsValidTarget(_r.Range) && !x.IsDead && !x.IsZombie))
                {
                    if (_r.GetPrediction(target).HitChance >= HitChance.High)
                    {
                        _r.Cast(target);
                    }
                }
            }
        }

        private static void LaneClear()
        {
            var qUse = _laneClearMenu["laneclear.q"].Cast<CheckBox>().CurrentValue;
            if (qUse && _q.IsReady())
            {
                foreach (var farm in ObjectManager.Get<Obj_AI_Minion>().Where(x=> x.IsValidTarget(_q.Range) && ObjectManager.Player.GetSpellDamage(x, SpellSlot.Q) >= x.Health))
                {
                    _q.Cast(farm.ServerPosition);
                }
            }
        }
    }
}
