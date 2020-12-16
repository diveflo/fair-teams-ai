import 'package:flutter/widgets.dart';
import 'package:frontend/model/player.dart';
import 'package:frontend/model/team.dart';

class GameState {
  final List<Player> candidates;
  final Team t;
  final Team ct;

  GameState({@required this.candidates, @required this.t, @required this.ct});

  factory GameState.initial() {
    return GameState(candidates: [
      Player(name: "Flo", steamID: "76561197973591119"),
      Player(name: "Hubi", steamID: "76561198258023370"),
      Player(name: "Alex", steamID: "76561198011775117"),
      Player(name: "Sandy", steamID: "76561198011654217"),
      Player(name: "Markus", steamID: "76561197984050254"),
      Player(name: "Andi", steamID: "76561199045573415"),
      Player(name: "Martin", steamID: "76561197978519504"),
      Player(name: "Ferdy", steamID: "76561198031200891"),
      Player(name: "Niggo", steamID: "76561197995643389"),
      Player(name: "Chris", steamID: "76561197976972561"),
      Player(name: "Stefan", steamID: "76561198058595736"),
      Player(name: "Uwe", steamID: "76561198053826525"),
    ], t: Team(), ct: Team());
  }

  GameState copyWith({List<Player> candidates}) {
    return new GameState(
        candidates: candidates ?? this.candidates,
        t: t ?? this.t,
        ct: ct ?? this.ct);
  }

  static GameState fromJson(Map<String, dynamic> json) {
    Map<String, dynamic> jsonT = json["terrorists"];
    Map<String, dynamic> jsonCT = json["counterTerrorists"];

    var t = Team.fromJson(jsonT);
    var ct = Team.fromJson(jsonCT);

    return json != null
        ? GameState(candidates: null, t: t, ct: ct)
        : GameState.initial();
  }
}
