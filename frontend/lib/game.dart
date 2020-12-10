import 'package:frontend/team.dart';

class Game {
  Team t;
  Team ct;

  Game.fromJson(Map<String, dynamic> json) {
    Map<String, dynamic> jsonT = json["terrorists"];
    Map<String, dynamic> jsonCT = json["counterTerrorists"];

    t = Team.fromJson(jsonT);
    ct = Team.fromJson(jsonCT);
  }
}
