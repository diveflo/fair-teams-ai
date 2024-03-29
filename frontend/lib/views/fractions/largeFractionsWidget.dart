import 'package:flutter/material.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:no_cry_babies/state/appState.dart';
import 'package:no_cry_babies/state/gameState.dart';
import 'package:no_cry_babies/views/fractions/teams/teamWidget.dart';

/// This class creates the fractions layout for large screen sizes
class LargeFractionsWidget extends StatelessWidget {
  const LargeFractionsWidget({
    Key key,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return StoreConnector<AppState, GameState>(
      converter: (store) => store.state.gameState,
      builder: (context, game) {
        return Row(
          mainAxisAlignment: MainAxisAlignment.spaceAround,
          children: [
            Expanded(
              flex: 1,
              child: TeamWidget(
                imagePath: 'assets/t.png',
                team: game.t.players,
                name: "Terrorists",
                color: Theme.of(context).highlightColor,
              ),
            ),
            Expanded(
              flex: 1,
              child: TeamWidget(
                imagePath: 'assets/ct.png',
                team: game.ct.players,
                name: "Counter Terrorists",
                color: Colors.blueGrey,
              ),
            ),
          ],
        );
      },
    );
  }
}
