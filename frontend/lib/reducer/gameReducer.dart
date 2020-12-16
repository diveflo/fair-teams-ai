import 'package:frontend/model/player.dart';
import 'package:frontend/state/gameConfigState.dart';
import 'package:redux/redux.dart';

final gameReducer = combineReducers<GameConfigState>([
  TypedReducer<GameConfigState, AddPlayerAction>(_addPlayer),
  TypedReducer<GameConfigState, TogglePlayerSelectionAction>(
      _togglePlayerSelection),
]);

class AddPlayerAction {
  final Player player;
  AddPlayerAction(this.player);
}

class TogglePlayerSelectionAction {
  final Player player;
  TogglePlayerSelectionAction(this.player);
}

GameConfigState _addPlayer(GameConfigState state, AddPlayerAction action) {
  List<Player> newCandidates = state.candidates;
  newCandidates.add(action.player);
  return state.copyWith(candidates: newCandidates);
}

GameConfigState _togglePlayerSelection(
    GameConfigState state, TogglePlayerSelectionAction action) {
  List<Player> updatedCandidates = state.candidates;
  updatedCandidates.forEach((element) {
    if (element.steamID == action.player.steamID) {
      element.isSelected = !element.isSelected;
    }
  });
  return state.copyWith(candidates: updatedCandidates);
}
