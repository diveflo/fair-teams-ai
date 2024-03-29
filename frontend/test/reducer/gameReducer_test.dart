import 'package:flutter_test/flutter_test.dart';
import 'package:no_cry_babies/model/player.dart';
import 'package:no_cry_babies/model/skill.dart';
import 'package:no_cry_babies/model/team.dart';
import 'package:no_cry_babies/reducer/gameReducer.dart';
import 'package:no_cry_babies/state/gameState.dart';

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
            skill: Skill(skillScore: 1),
          )
        ],
        "t",
      );
      Team ct = Team(
        [
          Player(
            name: 'player2',
            steamName: 'player2',
            steamID: "player2id",
            skill: Skill(skillScore: 2),
          )
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

  test("SwapTeamsAction", () {
    Team t = Team(
      [
        Player(
          name: 'playert',
          steamName: 'playert',
          steamID: "playertid",
          skill: Skill(skillScore: 1),
        ),
        Player(
          name: 'playert2',
          steamName: 'playert2',
          steamID: "playert2id",
          skill: Skill(skillScore: 1),
        )
      ],
      "t",
    );
    Team ct = Team(
      [
        Player(
          name: 'playerct',
          steamName: 'playerct',
          steamID: "playerctid",
          skill: Skill(skillScore: 2),
        )
      ],
      "ct",
    );
    GameState inputState = GameState(isLoading: false, ct: ct, t: t);
    var swapTeamsAction = SwapTeamsAction();

    GameState outputState = gameReducer(inputState, swapTeamsAction);

    expect(outputState.ct.players.length, 2);
    expect(outputState.t.players.length, 1);
    expect(outputState.ct.players.first.name, "playert");
    expect(outputState.ct.players.last.name, "playert2");
    expect(outputState.t.players.first.name, "playerct");
  });
}
