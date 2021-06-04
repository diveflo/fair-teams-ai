import 'package:NoCrybabies/model/team.dart';
import 'package:NoCrybabies/state/gameState.dart';
import 'package:redux/redux.dart';

final gameReducer = combineReducers<GameState>([
  TypedReducer<GameState, SetTeamsFromJsonAction>(_setTeamsFromJson),
  TypedReducer<GameState, SetTeamsAction>(_setTeams),
  TypedReducer<GameState, ToggleIsLoadingAction>(_toggleIsLoading),
  TypedReducer<GameState, SwapTeamsAction>(_swapTeams)
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

class ToggleIsLoadingAction {
  ToggleIsLoadingAction();
}

class SwapTeamsAction {
  SwapTeamsAction();
}

GameState _setTeamsFromJson(GameState state, SetTeamsFromJsonAction action) {
  return GameState.fromJson(action.json);
}

GameState _setTeams(GameState state, SetTeamsAction action) {
  return state.copyWith(t: action.t, ct: action.ct);
}

GameState _toggleIsLoading(GameState state, ToggleIsLoadingAction action) {
  return state.copyWith(isLoading: !state.isLoading);
}

GameState _swapTeams(GameState state, SwapTeamsAction action) {
  return state.copyWith(ct: state.t, t: state.ct);
}
