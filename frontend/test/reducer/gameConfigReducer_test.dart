import 'package:flutter_test/flutter_test.dart';
import 'package:NoCrybabies/model/candidate.dart';
import 'package:NoCrybabies/model/map.dart';
import 'package:NoCrybabies/reducer/gameConfigReducer.dart';
import 'package:NoCrybabies/state/gameConfigState.dart';

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

  group('ToggleMapSelectionAction', () {
    test('from false to true', () {
      GameConfigState inputState = GameConfigState(
          mapPool: MapPool.fromMaps([
            CsMap(name: "inferno", imagePath: "i.png", isChecked: false),
            CsMap(name: "nuke", imagePath: "n.png", isChecked: false)
          ]),
          candidates: []);

      var toggleAction = ToggleMapSelectionAction(
          CsMap(name: "inferno", imagePath: "i.png", isChecked: true));

      GameConfigState outputState = gameConfigReducer(inputState, toggleAction);

      expect(outputState.mapPool.maps.first.isChecked, true);
    });
    test('from true to false', () {
      GameConfigState inputState = GameConfigState(
          mapPool: MapPool.fromMaps([
            CsMap(name: "inferno", imagePath: "i.png", isChecked: true),
            CsMap(name: "nuke", imagePath: "n.png", isChecked: true)
          ]),
          candidates: []);

      var toggleAction = ToggleMapSelectionAction(
          CsMap(name: "inferno", imagePath: "i.png", isChecked: true));

      GameConfigState outputState = gameConfigReducer(inputState, toggleAction);

      expect(outputState.mapPool.maps.first.isChecked, false);
    });
  });

  group('ToggleincludeBotAction', () {
    test('from false to true', () {
      GameConfigState inputState = GameConfigState(
          mapPool: MapPool(), includeBot: false, candidates: []);

      var toggleAction = ToggleincludeBotAction();

      GameConfigState outputState = gameConfigReducer(inputState, toggleAction);

      expect(outputState.includeBot, true);
    });
    test('from true to false', () {
      GameConfigState inputState =
          GameConfigState(mapPool: MapPool(), includeBot: true, candidates: []);

      var toggleAction = ToggleincludeBotAction();

      GameConfigState outputState = gameConfigReducer(inputState, toggleAction);

      expect(outputState.includeBot, false);
    });
  });
}
