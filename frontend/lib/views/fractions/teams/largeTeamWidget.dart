import 'package:flutter/material.dart';
import 'package:no_cry_babies/model/player.dart';
import 'package:no_cry_babies/views/fractions/teams/fractionIdentifierWidget.dart';
import 'package:no_cry_babies/views/fractions/teams/playerCard/playerCardWidget.dart';

class LargeTeamWidget extends StatelessWidget {
  const LargeTeamWidget({
    Key key,
    @required this.imagePath,
    @required this.name,
    @required this.color,
    @required this.team,
  }) : super(key: key);

  final String imagePath;
  final String name;
  final Color color;
  final List<Player> team;

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        FractionIdentifierWidget(
            imagePath: imagePath, name: name, color: color),
        Expanded(
          child: Container(
            child: ListView.builder(
              controller: ScrollController(),
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
