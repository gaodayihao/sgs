﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sanguosha.Core.Cards;
using Sanguosha.Core.Players;
using Sanguosha.Core.Skills;
using Sanguosha.Core.Games;
using System.Threading;
using System.Diagnostics;

namespace Sanguosha.Core.UI
{
    public class GlobalServerUiProxy : IGlobalUiProxy
    {
        Dictionary<Player, ServerNetworkUiProxy> proxy;
        Dictionary<Player, Thread> proxyListener;
        Semaphore semAccess;
        Semaphore semWake;
        Semaphore semDone;
        ISkill answerSkill;
        List<Card> answerCard;
        List<Player> answerPlayer;
        Player responder;
        Game game;

        private struct UsageListenerThreadParameters
        {
            public ServerNetworkUiProxy proxy;
            public Prompt prompt;
            public ICardUsageVerifier verifier;
        }

        private struct ChoiceListenerThreadParameters
        {
            public ServerNetworkUiProxy proxy;
            public ICardChoiceVerifier verifier;
            public Player player;
            public List<DeckPlace> places;
            public List<int> resultMax;
            public List<bool> rearrangeable;
        }

        public bool AskForCardUsage(Prompt prompt, ICardUsageVerifier verifier, out ISkill skill, out List<Card> cards, out List<Player> players, out Player respondingPlayer)
        {
            proxyListener = new Dictionary<Player, Thread>();
            semAccess = new Semaphore(1, 1);
            semWake = new Semaphore(0, 2);
            semDone = new Semaphore(proxy.Count - 1, proxy.Count - 1);
            answerSkill = null;
            answerCard = null;
            answerPlayer = null;
            respondingPlayer = null;
            foreach (var player in Game.CurrentGame.Players)
            {
                if (!proxy.ContainsKey(player))
                {
                    continue;
                }
                UsageListenerThreadParameters para = new UsageListenerThreadParameters();
                para.prompt = prompt;
                para.proxy = proxy[player];
                para.verifier = verifier;
                Thread t = new Thread(
                    (ParameterizedThreadStart)
                    ((p) => { 
                        UsageProxyListenerThread((UsageListenerThreadParameters)p);
                    })) { IsBackground = true };
                t.Start(para);
                proxyListener.Add(player, t);
            }
            bool ret = true;
            if (!semWake.WaitOne(TimeOutSeconds * 1000))
            {
                semAccess.WaitOne(0);
                skill = null;
                cards = null;
                players = null;
                respondingPlayer = null;
                ret = false;
            }
            else
            {
                skill = answerSkill;
                cards = answerCard;
                players = answerPlayer;
                respondingPlayer = responder;
            }
            //if it didn't change, then semDone was triggered
            if (skill == null && cards == null && players == null)
            {
                ret = false;
            }
            foreach (var pair in proxyListener)
            {
                pair.Value.Abort();
                proxy[pair.Key].NextQuestion();
            }
            foreach (var player in Game.CurrentGame.Players)
            {
                if (!proxy.ContainsKey(player))
                {
                    continue;
                }
                else
                {
                    proxy[player].SendCardUsage(skill, cards, players);
                    break;
                }
            }

            return ret;
        }

        private void UsageProxyListenerThread(UsageListenerThreadParameters para)
        {
            game.RegisterCurrentThread();
            ISkill skill;
            List<Card> cards;
            List<Player> players;
            if (para.proxy.TryAskForCardUsage(para.prompt, para.verifier, out skill, out cards, out players))
            {

                semAccess.WaitOne();
                answerSkill = skill;
                answerCard = cards;
                answerPlayer = players;
                responder = para.proxy.HostPlayer;
                semWake.Release(1);
            }
            if (!semDone.WaitOne(0))
            {
                Trace.TraceInformation("All done");
                semWake.Release(1);
            }
        }

        public GlobalServerUiProxy(Game g, Dictionary<Player, IUiProxy> p)
        {
            game = g;
            proxy = new Dictionary<Player, ServerNetworkUiProxy>();
            foreach (var v in p)
            {
                if (!(v.Value is ServerNetworkUiProxy))
                {
                    Trace.TraceWarning("Some of my proxies are not server network proxies!");
                    continue;
                }
                proxy.Add(v.Key, v.Value as ServerNetworkUiProxy);
            }
        }

        public int TimeOutSeconds { get; set; }

        Dictionary<Player, Card> answerHero;

        public void AskForHeroChoice(Dictionary<Player, List<Card>> restDraw, Dictionary<Player, Card> heroSelection)
        {
            proxyListener = new Dictionary<Player, Thread>();
            semAccess = new Semaphore(1, 1);
            semWake = new Semaphore(0, 2);
            semDone = new Semaphore(proxy.Count - 2, proxy.Count - 1);
            answerHero = heroSelection;
            DeckType temp = new DeckType("Temp");
            foreach (var player in Game.CurrentGame.Players)
            {
                if (!proxy.ContainsKey(player) || player.Role == Role.Ruler)
                {
                    continue;
                }
                ChoiceListenerThreadParameters para = new ChoiceListenerThreadParameters();
                para.proxy = proxy[player];
                para.verifier = null;
                para.player = player;
                para.places = new List<DeckPlace>() { new DeckPlace(player, temp) };
                para.rearrangeable = new List<bool>() { false };
                para.resultMax = new List<int> { 1 };
                Game.CurrentGame.Decks[player, temp].AddRange(restDraw[player]);
                Thread t = new Thread(
                    (ParameterizedThreadStart)
                    ((p) =>
                    {
                        ChoiceProxyListenerThread((ChoiceListenerThreadParameters)p);
                    })) { IsBackground = true };
                t.Start(para);
                proxyListener.Add(player, t);
            }
            if (!semWake.WaitOne(TimeOutSeconds * 1000))
            {
                semAccess.WaitOne(0);
            }

            foreach (var pair in proxyListener)
            {
                pair.Value.Abort();
            }

        }

        private void ChoiceProxyListenerThread(ChoiceListenerThreadParameters para)
        {
            game.RegisterCurrentThread();
            List<List<Card>> answer;
            if (para.proxy.TryAskForCardChoice(para.places, para.resultMax, new AlwaysTrueChoiceVerifier(), out answer, para.rearrangeable, null))
            {
                semAccess.WaitOne();
                if (answer != null && answer.Count != 0 && answer[0] != null && answer[0].Count != 0)
                {
                    answerHero.Add(para.player, answer[0][0]);
                }
                semAccess.Release();
            }
            if (!semDone.WaitOne(0))
            {
                Trace.TraceInformation("All done");
                semWake.Release(1);
            }
        }

    }
}