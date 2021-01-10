import 'package:frontend/model/player.dart';

class Team {
  List<Player> players;
  String name;

  Team.empy() {
    players = [];
    name = "";
  }

  Team(List<Player> players, String name) {
    this.players = players;
    this.name = name;
  }

  Team.fromJson(Map<String, dynamic> json) {
    List<dynamic> playersJson = json["players"];
    players = playersJson.map((player) => Player.fromJson(player)).toList();
    name = json["name"];
  }

  dynamic toJson() => {
        "players": this.players.map((player) => player.toJson()).toList(),
        "name": name,
      };
}
