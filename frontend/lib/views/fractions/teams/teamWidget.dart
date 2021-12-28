import 'package:flutter/material.dart';
import 'package:no_cry_babies/model/player.dart';
import 'package:no_cry_babies/views/fractions/teams/largeTeamWidget.dart';
import 'package:no_cry_babies/views/fractions/teams/smallTeamWidget.dart';

/// This class creates the layout of one team / fraction
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
    return LayoutBuilder(
      builder: (BuildContext context, BoxConstraints constraints) {
        if (constraints.maxHeight > 400) {
          return LargeTeamWidget(
            imagePath: imagePath,
            name: name,
            color: color,
            team: team,
          );
        }
        return SmallTeamWidget(
          team: team,
          color: color,
        );
      },
    );
  }
}
