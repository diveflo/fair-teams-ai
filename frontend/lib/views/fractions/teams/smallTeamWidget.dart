import 'package:flutter/material.dart';
import 'package:no_cry_babies/model/player.dart';
import 'package:no_cry_babies/views/fractions/teams/playerCard/playerCardWidget.dart';

class SmallTeamWidget extends StatelessWidget {
  const SmallTeamWidget({
    Key key,
    @required this.team,
    @required this.color,
  }) : super(key: key);

  final List<Player> team;
  final Color color;

  @override
  Widget build(BuildContext context) {
    return Container(
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
    );
  }
}
