import 'package:frontend/model/candidate.dart';
import 'package:frontend/state/gameConfigState.dart';
import 'package:redux/redux.dart';

final gameConfigReducer = combineReducers<GameConfigState>([
  TypedReducer<GameConfigState, AddPlayerAction>(_addPlayer),
  TypedReducer<GameConfigState, TogglePlayerSelectionAction>(
      _togglePlayerSelection),
]);

class AddPlayerAction {
  final Candidate player;
  AddPlayerAction(this.player);
}

class TogglePlayerSelectionAction {
  final Candidate player;
  TogglePlayerSelectionAction(this.player);
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
