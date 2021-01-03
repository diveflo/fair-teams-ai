import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/model/candidate.dart';
import 'package:frontend/reducer/gameConfigReducer.dart';
import 'package:frontend/state/gameConfigState.dart';

void main() {
  test('AddPlayerAction', () {
    GameConfigState inputState = GameConfigState(candidates: []);
    Candidate newCandidate = Candidate(name: "player1", steamID: "player1id");
    var addAction = AddPlayerAction(newCandidate);

    GameConfigState outputState = gameConfigReducer(inputState, addAction);

    expect(outputState.candidates.length, 1);
  });
  test('TogglePlayerSelectionAction', () {
    GameConfigState inputState = GameConfigState(candidates: [
      Candidate(name: "player1", steamID: "player1id", isSelected: false)
    ]);
    var toggleAction = TogglePlayerSelectionAction(
        Candidate(name: "player1", steamID: "player1id"));

    GameConfigState outputState = gameConfigReducer(inputState, toggleAction);

    expect(outputState.candidates.first.isSelected, true);
  });
}
