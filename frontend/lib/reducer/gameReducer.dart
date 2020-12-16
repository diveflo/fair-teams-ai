import 'package:frontend/model/team.dart';
import 'package:frontend/state/gameState.dart';
import 'package:redux/redux.dart';

final gameReducer = combineReducers<GameState>([
  TypedReducer<GameState, SetTeamsFromJsonAction>(_setTeamsFromJson),
  TypedReducer<GameState, SetTeamsAction>(_setTeams),
]);

class SetTeamsFromJsonAction {
  final dynamic json;
  SetTeamsFromJsonAction(this.json);
}

class SetTeamsAction {
  final Team t;
  final Team ct;
  SetTeamsAction(this.t, this.ct);
}

GameState _setTeamsFromJson(GameState state, SetTeamsFromJsonAction action) {
  return GameState.fromJson(action.json);
}

GameState _setTeams(GameState state, SetTeamsAction action) {
  return state.copyWith(t: action.t, ct: action.ct);
}
