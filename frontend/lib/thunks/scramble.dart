import 'package:frontend/model/player.dart';
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
