import 'package:frontend/model/player.dart';
import 'package:frontend/model/team.dart';
import 'package:frontend/reducer/gameReducer.dart';
import 'package:redux/redux.dart';
import 'package:redux_thunk/redux_thunk.dart';
import 'package:frontend/apiService.dart';

PlayerApi api = PlayerApi();

ThunkAction scrambleTeams() {
  return (Store store) async {
    Future(() {
      store.dispatch(ToggleIsLoadingAction());
      List<Player> candidates = store.state.gameConfigState.candidates;
      List<Player> activeCandidates =
          candidates.where((element) => element.isSelected).toList();

      api.fetchScrambledTeams(activeCandidates).then((game) {
        store.dispatch(ToggleIsLoadingAction());
        store.dispatch(SetTeamsAction(game.t, game.ct));
      });
    });
  };
}

ThunkAction scrambleTeamsRandom() {
  return (Store store) {
    List<Player> candidates = store.state.gameConfigState.candidates;

    List<Player> activePlayers =
        candidates.where((element) => element.isSelected).toList();
    if (activePlayers.length > 0) {
      List<Player> _team1 = List<Player>();
      List<Player> _team2 = List<Player>();
      activePlayers.shuffle();
      for (int i = 0; i < activePlayers.length; i++) {
        if (i % 2 == 0) {
          _team1.add(activePlayers[i]);
        } else {
          _team2.add(activePlayers[i]);
        }
      }
      Team a = Team(_team1, "tt");
      Team b = Team(_team2, "ct");
      store.dispatch(SetTeamsAction(a, b));
    }
  };
}
