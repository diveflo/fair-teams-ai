import 'package:flutter/material.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:no_cry_babies/state/appState.dart';
import 'package:no_cry_babies/state/gameState.dart';
import 'package:no_cry_babies/views/botsWidget.dart';
import 'package:no_cry_babies/views/candidates/candidatesWidget.dart';
import 'package:no_cry_babies/views/scrambleWidget.dart';
import 'package:no_cry_babies/views/teamWidget.dart';

class SmallAppLayoutWidget extends StatelessWidget {
  const SmallAppLayoutWidget({
    Key key,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Expanded(
          flex: 1,
          child: CandidatesWidget(),
        ),
        Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            BotsWidget(),
            ScrambleWidget(),
          ],
        ),
        Expanded(
          flex: 2,
          child: StoreConnector<AppState, GameState>(
            converter: (store) => store.state.gameState,
            builder: (context, game) {
              return Column(
                mainAxisAlignment: MainAxisAlignment.spaceAround,
                children: [
                  Expanded(
                    flex: 1,
                    child: TeamWidget(
                      imagePath: 'assets/t.png',
                      team: game.t.players,
                      name: "Terrorists",
                      color: Colors.orange,
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
          ),
        ),
      ],
    );
  }
}
