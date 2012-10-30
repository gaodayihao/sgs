﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Sanguosha.Core.Triggers;
using Sanguosha.Core.Cards;
using Sanguosha.Core.UI;
using Sanguosha.Core.Skills;
using Sanguosha.Expansions.Basic.Cards;
using Sanguosha.Core.Games;
using Sanguosha.Core.Players;
using Sanguosha.Core.Exceptions;

namespace Sanguosha.Expansions.Basic.Skills
{
    /// <summary>
    /// 咆哮-出牌阶段，你可以使用任意数量的【杀】。
    /// </summary>
    public class PaoXiao : PassiveSkill
    {
        class PaoXiaoTrigger : Trigger
        {
            public override void Run(GameEvent gameEvent, GameEventArgs eventArgs)
            {
                ShaEventArgs args = (ShaEventArgs)eventArgs;
                Trace.Assert(args != null);
                if (args.Source != Owner)
                {
                    return;
                }
                args.TargetApproval[0] = true;
            }
            public PaoXiaoTrigger(Player p)
            {
                Owner = p;
            }
        }

        class PaoXiaoAlwaysShaTrigger : Trigger
        {
            public override void Run(GameEvent gameEvent, GameEventArgs eventArgs)
            {
                if (eventArgs.Source == Owner)
                {
                    throw new TriggerResultException(TriggerResult.Success);
                }
            }
            public PaoXiaoAlwaysShaTrigger(Player p)
            {
                Owner = p;
            }
        }

        protected override void InstallTriggers(Sanguosha.Core.Players.Player owner)
        {
            Game.CurrentGame.RegisterTrigger(Sha.PlayerShaTargetValidation, new PaoXiaoTrigger(owner));
            Game.CurrentGame.RegisterTrigger(Sha.PlayerNumberOfShaCheck, new PaoXiaoAlwaysShaTrigger(owner));
        }
    }
}