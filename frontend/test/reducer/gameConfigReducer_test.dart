import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/model/candidate.dart';
import 'package:frontend/reducer/gameConfigReducer.dart';
import 'package:frontend/state/gameConfigState.dart';

void main() {
  test('AddPlayerAction', () {
    GameConfigState inputState = GameConfigState(candidates: [], mapPool: null);
    Candidate newCandidate = Candidate(name: "player1", steamID: "player1id");
    var addAction = AddPlayerAction(newCandidate);

    GameConfigState outputState = gameConfigReducer(inputState, addAction);

    expect(outputState.candidates.length, 1);
  });

  group('TogglePlayerSelectionAction', () {
    test('from false to true', () {
      GameConfigState inputState = GameConfigState(candidates: [
        Candidate(name: "player1", steamID: "player1id", isSelected: false)
      ], mapPool: null);
      var toggleAction = TogglePlayerSelectionAction(
          Candidate(name: "player1", steamID: "player1id"));

      GameConfigState outputState = gameConfigReducer(inputState, toggleAction);

      expect(outputState.candidates.first.isSelected, true);
    });
    test('from true to false', () {
      GameConfigState inputState = GameConfigState(candidates: [
        Candidate(name: "player1", steamID: "player1id", isSelected: true)
      ], mapPool: null);
      var toggleAction = TogglePlayerSelectionAction(
          Candidate(name: "player1", steamID: "player1id"));

      GameConfigState outputState = gameConfigReducer(inputState, toggleAction);

      expect(outputState.candidates.first.isSelected, false);
    });
  });
}
