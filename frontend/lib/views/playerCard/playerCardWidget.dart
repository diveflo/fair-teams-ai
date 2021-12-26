import 'package:flutter/material.dart';
import 'package:no_cry_babies/model/player.dart';
import 'package:no_cry_babies/views/playerCard/skillWidget.dart';

class PlayerCardWidget extends StatelessWidget {
  const PlayerCardWidget({
    Key key,
    @required this.color,
    @required this.team,
    @required this.playerIndex,
  }) : super(key: key);

  final Color color;
  final List<Player> team;
  final int playerIndex;

  @override
  Widget build(BuildContext context) {
    return Card(
      shape: RoundedRectangleBorder(
          side: BorderSide(color: color, width: 2),
          borderRadius: BorderRadius.circular(8)),
      child: Column(
        children: [
          ListTile(
            leading: Image(
              image:
                  AssetImage("assets/" + team[playerIndex].skill.rank + ".png"),
            ),
            title: Text(
              team[playerIndex].name,
              style: TextStyle(fontSize: 20),
            ),
            subtitle: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(team[playerIndex].steamName,
                    style:
                        TextStyle(color: color, fontWeight: FontWeight.bold)),
                SkillWidget(skill: team[playerIndex].skill),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
