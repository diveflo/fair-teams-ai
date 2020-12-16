import 'package:flutter/widgets.dart';
import 'package:frontend/model/player.dart';

class GameConfigState {
  final List<Player> candidates;

  GameConfigState({@required this.candidates});

  factory GameConfigState.initial() {
    return GameConfigState(
      candidates: [
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
      ],
    );
  }

  GameConfigState copyWith({List<Player> candidates}) {
    return new GameConfigState(
      candidates: candidates ?? this.candidates,
    );
  }
}
