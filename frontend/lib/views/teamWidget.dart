import 'package:flutter/material.dart';
import 'package:no_cry_babies/model/player.dart';
import 'package:no_cry_babies/views/playerCard/playerCardWidget.dart';
import 'fractionIdentifierWidget.dart';

class TeamWidget extends StatelessWidget {
  const TeamWidget({
    Key key,
    @required this.team,
    @required this.color,
    @required this.name,
    @required this.imagePath,
  }) : super(key: key);

  final List<Player> team;
  final Color color;
  final String name;
  final String imagePath;

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        FractionIdentifierWidget(
            imagePath: imagePath, name: name, color: color),
        Expanded(
          child: Container(
            child: ListView.builder(
              itemCount: team.length,
              itemBuilder: (BuildContext context, int index) {
                return PlayerCardWidget(
                  color: color,
                  team: team,
                  playerIndex: index,
                );
              },
            ),
          ),
        )
      ],
    );
  }
}
