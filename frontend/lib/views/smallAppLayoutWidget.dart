import 'package:flutter/material.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:no_cry_babies/state/appState.dart';
import 'package:no_cry_babies/views/botsWidget.dart';
import 'package:no_cry_babies/views/candidates/candidatesWidget.dart';
import 'package:no_cry_babies/views/scrambleWidget.dart';
import 'package:no_cry_babies/views/teams/teamWidget.dart';

import 'configWidget.dart';

class SmallAppLayoutWidget extends StatelessWidget {
  const SmallAppLayoutWidget({
    Key key,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return StoreConnector<AppState, AppState>(
        converter: (store) => store.state,
        builder: (context, game) {
          return Column(
            children: [
              Visibility(
                visible: game.gameConfigState.isConfigVisible,
                child: Expanded(
                  flex: 1,
                  child: CandidatesWidget(),
                ),
              ),
              Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  ConfigWidget(),
                  BotsWidget(),
                  ScrambleWidget(),
                ],
              ),
              Expanded(
                flex: 2,
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.spaceAround,
                  children: [
                    Expanded(
                      flex: 1,
                      child: TeamWidget(
                        imagePath: 'assets/t.png',
                        team: game.gameState.t.players,
                        name: "Terrorists",
                        color: Colors.orange,
                      ),
                    ),
                    Expanded(
                      flex: 1,
                      child: TeamWidget(
                        imagePath: 'assets/ct.png',
                        team: game.gameState.ct.players,
                        name: "Counter Terrorists",
                        color: Colors.blueGrey,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          );
        });
  }
}
