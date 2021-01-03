import 'package:flutter/widgets.dart';
import 'package:frontend/model/team.dart';

class GameState {
  final Team t;
  final Team ct;
  final bool isLoading;

  GameState({@required this.t, @required this.ct, @required this.isLoading});

  factory GameState.initial() {
    return GameState(
      ct: Team.empy(),
      t: Team.empy(),
      isLoading: false,
    );
  }

  factory GameState.fromJson(dynamic json) {
    if (json != null) {
      Map<String, dynamic> jsonT = json["terrorists"];
      Map<String, dynamic> jsonCT = json["counterTerrorists"];

      var t = Team.fromJson(jsonT);
      var ct = Team.fromJson(jsonCT);
      return GameState(ct: ct, t: t, isLoading: false);
    }

    return GameState.initial();
  }

  dynamic toJson() => {
        "terrorists": this.t.toJson(),
        "counterTerrorists": this.ct.toJson(),
      };

  GameState copyWith({Team t, Team ct, bool isLoading}) {
    return GameState(
        ct: ct ?? this.ct,
        t: t ?? this.t,
        isLoading: isLoading ?? this.isLoading);
  }
}
