import 'package:no_cry_babies/model/candidate.dart';
import 'package:no_cry_babies/model/map.dart';
import 'package:no_cry_babies/state/gameConfigState.dart';
import 'package:redux/redux.dart';

final gameConfigReducer = combineReducers<GameConfigState>([
  TypedReducer<GameConfigState, AddPlayerAction>(_addPlayer),
  TypedReducer<GameConfigState, TogglePlayerSelectionAction>(
      _togglePlayerSelection),
  TypedReducer<GameConfigState, ToggleMapSelectionAction>(_toggleMapSelection),
  TypedReducer<GameConfigState, ToggleincludeBotAction>(_toggleincludeBot),
  TypedReducer<GameConfigState, ToggleIsConfigVisibleAction>(
      _toggleIsConfigVisible)
]);

class AddPlayerAction {
  final Candidate player;
  AddPlayerAction(this.player);
}

class TogglePlayerSelectionAction {
  final Candidate player;
  TogglePlayerSelectionAction(this.player);
}

class ToggleMapSelectionAction {
  final CsMap map;
  ToggleMapSelectionAction(this.map);
}

class ToggleincludeBotAction {
  ToggleincludeBotAction();
}

class ToggleIsConfigVisibleAction {
  ToggleIsConfigVisibleAction();
}

GameConfigState _addPlayer(GameConfigState state, AddPlayerAction action) {
  List<Candidate> newCandidates = state.candidates;
  newCandidates.add(action.player);
  return state.copyWith(candidates: newCandidates);
}

GameConfigState _togglePlayerSelection(
    GameConfigState state, TogglePlayerSelectionAction action) {
  List<Candidate> updatedCandidates = state.candidates;
  updatedCandidates.forEach((element) {
    if (element.steamID == action.player.steamID) {
      element.isSelected = !element.isSelected;
    }
  });
  return state.copyWith(candidates: updatedCandidates);
}

GameConfigState _toggleMapSelection(
    GameConfigState state, ToggleMapSelectionAction action) {
  MapPool updatedMapPool = state.mapPool;
  updatedMapPool.maps.forEach((element) {
    if (element.name == action.map.name) {
      element.isChecked = !element.isChecked;
    }
  });
  return state.copyWith(mapPool: updatedMapPool);
}

GameConfigState _toggleincludeBot(
    GameConfigState state, ToggleincludeBotAction action) {
  return state.copyWith(includeBot: !state.includeBot);
}

GameConfigState _toggleIsConfigVisible(
    GameConfigState state, ToggleIsConfigVisibleAction action) {
  return state.copyWith(isConfigVisible: !state.isConfigVisible);
}
