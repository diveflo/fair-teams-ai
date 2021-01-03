using fairTeams.Core;
using Microsoft.Extensions.Logging;
using SteamKit2.GC;
using SteamKit2.GC.CSGO.Internal;
using System;

namespace fairTeams.DemoHandling.SteamKitExt
{
    public partial class CsgoClient
    {
        /// <summary>
        ///     Request MatchmakingStats from the game coordinator.
        /// </summary>
        /// <param name="callback">The callback to be executed when the operation finishes.</param>
        public void MatchmakingStatsRequest(Action<CMsgGCCStrike15_v2_MatchmakingGC2ClientHello> callback)
        {
            _gcMap.Add((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchmakingGC2ClientHello,
                msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchmakingGC2ClientHello>(msg).Body));

            myLogger.LogTrace("Requesting Matchmaking stats");

            var clientGcMsgProtobuf =
                new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchmakingClient2GCHello>(
                    (uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchmakingClient2GCHello);

            _gameCoordinator.Send(clientGcMsgProtobuf, CsgoAppid);
        }

        /// <summary>
        ///     Request the list of currently live games
        /// </summary>
        /// <param name="callback">The callback to be executed when the operation finishes.</param>
        public void RequestCurrentLiveGames(Action<CMsgGCCStrike15_v2_MatchList> callback)
        {
            _gcMap.Add((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchList,
                msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchList>(msg).Body));

            var clientGcMsgProtobuf = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchListRequestCurrentLiveGames>(
                (uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchListRequestCurrentLiveGames);

            _gameCoordinator.Send(clientGcMsgProtobuf, CsgoAppid);
        }

        /// <summary>
        ///     Requests current live game info for given user.
        /// </summary>
        /// <param name="accountId">Account to request</param>
        /// <param name="callback">The callback to be executed when the operation finishes.</param>
        public void RequestLiveGameForUser(uint accountId, Action<CMsgGCCStrike15_v2_MatchList> callback)
        {
            _gcMap.Add((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchList,
                msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchList>(msg).Body));

            var clientGcMsgProtobuf = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchListRequestLiveGameForUser>(
                (uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchListRequestLiveGameForUser)
            {
                Body =
                {
                    accountid = accountId
                }
            };

            _gameCoordinator.Send(clientGcMsgProtobuf, CsgoAppid);
        }

        /// <summary>
        ///     Requests info about game given a matchId, outcomeId, and token for a game.
        /// </summary>
        /// <param name="request">Request parameters</param>
        /// <param name="callback">The callback to be executed when the operation finishes.</param>
        public void RequestGame(GameRequest request, Action<CMsgGCCStrike15_v2_MatchList> callback)
        {
            _gcMap.Add((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchList,
                msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchList>(msg).Body));

            var clientGcMsgProtobuf = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchListRequestFullGameInfo>((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchListRequestFullGameInfo);

            clientGcMsgProtobuf.Body.token = request.Token;
            clientGcMsgProtobuf.Body.matchid = request.MatchId;
            clientGcMsgProtobuf.Body.outcomeid = request.OutcomeId;

            _gameCoordinator.Send(clientGcMsgProtobuf, CsgoAppid);
        }

        /// <summary>
        ///     Requests a list of recent games for the given account id
        /// </summary>
        /// <param name="accountId">Account IDd for the request</param>
        /// <param name="callback">The callback to be executed when the operation finishes.</param>
        public void RequestRecentGames(uint accountId, Action<CMsgGCCStrike15_v2_MatchList> callback)
        {
            _gcMap.Add((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchList,
                msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchList>(msg).Body));

            var clientGcMsgProtobuf = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchListRequestRecentUserGames>(
                (uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchListRequestRecentUserGames)
            {
                Body =
                {
                    accountid = accountId
                }
            };

            _gameCoordinator.Send(clientGcMsgProtobuf, CsgoAppid);
        }

        /// <summary>
        ///     Requests a list of recent games for the given account id
        /// </summary>
        /// <param name="callback">The callback to be executed when the operation finishes.</param>
        public void RequestRecentGames(Action<CMsgGCCStrike15_v2_MatchList> callback)
        {
            RequestRecentGames(_steamUser.SteamID.AccountID, callback);
        }
    }
}