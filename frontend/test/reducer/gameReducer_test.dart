import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/model/player.dart';
import 'package:frontend/model/team.dart';
import 'package:frontend/reducer/gameReducer.dart';
import 'package:frontend/state/gameState.dart';

void main() {
  var toggleAction = ToggleIsLoadingAction();
  group("ToggleIsLoadingAction", () {
    test("from true to false", () {
      GameState inputState = GameState(isLoading: true, ct: null, t: null);

      GameState outputState = gameReducer(inputState, toggleAction);

      expect(outputState.isLoading, false);
    });
    test("from false to true", () {
      GameState inputState = GameState(isLoading: false, ct: null, t: null);

      GameState outputState = gameReducer(inputState, toggleAction);

      expect(outputState.isLoading, true);
    });
  });

  group("SetTeamsAction", () {
    test("sets teams", () {
      GameState inputState = GameState(isLoading: false, ct: null, t: null);
      Team t = Team(
        [
          Player(
              name: 'player1',
              steamName: 'player1',
              steamID: "player1id",
              skillScore: 1)
        ],
        "t",
      );
      Team ct = Team(
        [
          Player(
              name: 'player2',
              steamName: 'player2',
              steamID: "player2id",
              skillScore: 2)
        ],
        "ct",
      );
      var setTeamsAction = SetTeamsAction(t, ct);

      GameState outputState = gameReducer(inputState, setTeamsAction);

      expect(outputState.ct.players.length, 1);
      expect(outputState.ct.players.first.name, "player2");
      expect(outputState.ct.players.first.steamID, "player2id");
      expect(outputState.ct.players.first.steamName, "player2");

      expect(outputState.t.players.length, 1);
      expect(outputState.t.players.first.name, "player1");
      expect(outputState.t.players.first.steamID, "player1id");
      expect(outputState.t.players.first.steamName, "player1");
    });
  });
}
