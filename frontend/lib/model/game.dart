import 'package:no_cry_babies/model/player.dart';
import 'package:no_cry_babies/model/team.dart';

class Game {
  Team t;
  Team ct;
  List<Player> candidates;

  Game.fromJson(Map<String, dynamic> json) {
    Map<String, dynamic> jsonT = json["terrorists"];
    Map<String, dynamic> jsonCT = json["counterTerrorists"];

    t = Team.fromJson(jsonT);
    ct = Team.fromJson(jsonCT);
  }
}
