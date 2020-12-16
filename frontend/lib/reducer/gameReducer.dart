import 'package:frontend/model/player.dart';
import 'package:frontend/state/gameState.dart';
import 'package:redux/redux.dart';

final gameReducer = combineReducers<GameState>([
  TypedReducer<GameState, AddPlayerAction>(_addPlayer),
  TypedReducer<GameState, TogglePlayerSelectionAction>(_togglePlayerSelection),
]);

class AddPlayerAction {
  final Player player;
  AddPlayerAction(this.player);
}

class TogglePlayerSelectionAction {
  final Player player;
  TogglePlayerSelectionAction(this.player);
}

GameState _addPlayer(GameState state, AddPlayerAction action) {
  List<Player> newCandidates = state.candidates;
  newCandidates.add(action.player);
  return state.copyWith(candidates: newCandidates);
}

GameState _togglePlayerSelection(
    GameState state, TogglePlayerSelectionAction action) {
  List<Player> updatedCandidates = state.candidates;
  updatedCandidates.forEach((element) {
    if (element.steamID == action.player.steamID) {
      element.isSelected = !element.isSelected;
    }
  });
  return state.copyWith(candidates: updatedCandidates);
}
