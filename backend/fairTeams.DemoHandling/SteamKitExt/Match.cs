using fairTeams.Core;
using Microsoft.Extensions.Logging;
using SteamKit2.GC;
using SteamKit2.GC.CSGO.Internal;
using System;

namespace fairTeams.DemoHandling.SteamKitExt
{
    public partial class CsgoClient
    {
        public void MatchmakingStatsRequest(Action<CMsgGCCStrike15_v2_MatchmakingGC2ClientHello> callback)
        {
            myCallbackStore.Add((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchmakingGC2ClientHello,
                msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchmakingGC2ClientHello>(msg).Body));

            myLogger.LogTrace("Requesting Matchmaking stats");

            var clientGcMsgProtobuf =
                new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchmakingClient2GCHello>(
                    (uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchmakingClient2GCHello);

            myGameCoordinator.Send(clientGcMsgProtobuf, CsgoAppid);
        }

        public void RequestCurrentLiveGames(Action<CMsgGCCStrike15_v2_MatchList> callback)
        {
            myCallbackStore.Add((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchList,
                msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchList>(msg).Body));

            var clientGcMsgProtobuf = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchListRequestCurrentLiveGames>(
                (uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchListRequestCurrentLiveGames);

            myGameCoordinator.Send(clientGcMsgProtobuf, CsgoAppid);
        }

        public void RequestLiveGameForUser(uint accountId, Action<CMsgGCCStrike15_v2_MatchList> callback)
        {
            myCallbackStore.Add((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchList,
                msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchList>(msg).Body));

            var clientGcMsgProtobuf = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchListRequestLiveGameForUser>(
                (uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchListRequestLiveGameForUser)
            {
                Body =
                {
                    accountid = accountId
                }
            };

            myGameCoordinator.Send(clientGcMsgProtobuf, CsgoAppid);
        }

        public void RequestGame(GameRequest request, Action<CMsgGCCStrike15_v2_MatchList> callback)
        {
            if (myCallbackStore == null)
            {
                myLogger.LogError("Callbackstore unexpectedly is null");
                throw new GameCoordinatorException("Callbackstore unexpectedly is null while trying to request game");
            }

            myCallbackStore.Add((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchList,
                msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchList>(msg).Body));

            var clientGcMsgProtobuf = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchListRequestFullGameInfo>((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchListRequestFullGameInfo);

            clientGcMsgProtobuf.Body.token = request.Token;
            clientGcMsgProtobuf.Body.matchid = request.MatchId;
            clientGcMsgProtobuf.Body.outcomeid = request.OutcomeId;

            if (myGameCoordinator == null)
            {
                myLogger.LogError("GameCoordinator unexpectedly is null");
                throw new GameCoordinatorException("GameCoordinator unexpectedly is null while trying to request game");
            }

            myGameCoordinator.Send(clientGcMsgProtobuf, CsgoAppid);
        }

        public void RequestRecentGames(uint accountId, Action<CMsgGCCStrike15_v2_MatchList> callback)
        {
            myCallbackStore.Add((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchList,
                msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchList>(msg).Body));

            var clientGcMsgProtobuf = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchListRequestRecentUserGames>(
                (uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchListRequestRecentUserGames)
            {
                Body =
                {
                    accountid = accountId
                }
            };

            myGameCoordinator.Send(clientGcMsgProtobuf, CsgoAppid);
        }

        public void RequestRecentGames(Action<CMsgGCCStrike15_v2_MatchList> callback)
        {
            RequestRecentGames(mySteamUser.SteamID.AccountID, callback);
        }
    }
}