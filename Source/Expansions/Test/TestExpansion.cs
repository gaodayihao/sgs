﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sanguosha.Core.Players;
using Sanguosha.Core.Cards;
using Sanguosha.Core.Games;
using Sanguosha.Expansions.Fire.Skills;
using Sanguosha.Expansions.Basic.Skills;
using Sanguosha.Expansions.OverKnightFame11.Skills;
using Sanguosha.Expansions.SP.Skills;
using Sanguosha.Expansions.Woods.Skills;
using Sanguosha.Core.Heroes;
using Sanguosha.Expansions.Hills.Skills;
using Sanguosha.Expansions.Wind.Skills;


namespace Sanguosha.Expansions.Test
{
    public class TestExpansion : Expansion
    {
        public TestExpansion()
        {
            CardSet = new List<Card>();
            CardSet.Add(new Card(SuitType.None, -1, new HeroCardHandler(new Hero("Test", false, Allegiance.Wei, 7, new GuHuo(), new LuanWu(), new KuRou(), new JiJiang()))));
            CardSet.Add(new Card(SuitType.None, -1, new HeroCardHandler(new Hero("Test2", true, Allegiance.Shu, 7, new WuShuang(), new LongDan(), new PaoXiao(), new FanKui(), new LiJian(), new QianXun(), new QingGuo(), new GangLie(), new GuiCai(), new LianYing(), new RenDe()))));
            CardSet.Add(new Card(SuitType.None, -1, new HeroCardHandler(new Hero("Test3", true, Allegiance.Shu, 7, new GuiCai(), new FangZhu(), new JueQing(), new FanKui(), new GangLie(), new JianXiong(), new YingZi(), new TuXi(), new LiJian(), new RenDe(), new HuoJi(), new KanPo(), new JiLei(), new LuoYing(), new LongDan()))));
            CardSet.Add(new Card(SuitType.None, -1, new HeroCardHandler(new Hero("Test4", true, Allegiance.Shu, 7, new QuHu(), new FanKui(), new GangLie(), new JianXiong(), new LiuLi(), new YingZi(), new TuXi(), new GuiCai(), new LiJian(), new RenDe(), new HuoJi(), new KanPo(), new JiLei(), new LongDan()))));
            CardSet.Add(new Card(SuitType.None, -1, new HeroCardHandler(new Hero("Test5", true, Allegiance.Shu, 7, new GuiCai(), new QuHu(), new WuShuang(), new FanKui(), new GangLie(), new JianXiong(), new LiuLi(), new LiJian(), new RenDe(), new HuoJi(), new KanPo(), new JiLei(), new LongDan()))));
            CardSet.Add(new Card(SuitType.None, -1, new HeroCardHandler(new Hero("Test6", true, Allegiance.Wei, 7, new GuiCai(), new QuHu(), new WuShuang(), new FanKui(), new GangLie(), new JianXiong(), new LiuLi(), new LiJian(), new RenDe(), new HuoJi(), new KanPo(), new JiLei(), new LongDan()))));
            CardSet.Add(new Card(SuitType.None, -1, new HeroCardHandler(new Hero("Test7", true, Allegiance.Wei, 7, new QuHu(), new WuShuang(), new FanKui(), new GangLie(), new JianXiong(), new LiuLi(), new LiJian(), new RenDe(), new HuoJi(), new KanPo(), new JiLei(), new LongDan()))));
        }
    }
}
