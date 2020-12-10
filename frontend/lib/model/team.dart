import 'package:frontend/model/player.dart';

class Team {
  List<Player> players;
  String name;

  Team.fromJson(Map<String, dynamic> json) {
    List<Map<String, dynamic>> playersJson = json["players"];
    players = playersJson.map((player) => Player.fromJson(player)).toList();
    name = json["name"];
  }
}
