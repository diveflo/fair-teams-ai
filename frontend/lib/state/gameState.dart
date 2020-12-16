import 'package:flutter/widgets.dart';
import 'package:frontend/model/team.dart';

class GameState {
  final Team t;
  final Team ct;

  GameState({@required this.t, @required this.ct});

  factory GameState.initial() {
    return GameState(ct: Team.empy(), t: Team.empy());
  }

  factory GameState.fromJson(dynamic json) {
    Map<String, dynamic> jsonT = json["terrorists"];
    Map<String, dynamic> jsonCT = json["counterTerrorists"];

    var t = Team.fromJson(jsonT);
    var ct = Team.fromJson(jsonCT);

    return json != null ? GameState(ct: ct, t: t) : GameState.initial();
  }

  GameState copyWith({Team t, Team ct}) {
    return GameState(ct: ct ?? this.ct, t: t ?? this.t);
  }
}
