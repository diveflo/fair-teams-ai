import 'package:flutter/material.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:frontend/state/appState.dart';
import 'package:frontend/state/gameState.dart';
import 'package:frontend/views/candidatesWidget.dart';
import 'package:frontend/views/teamWidget.dart';
import 'package:frontend/views/mapPoolWidget.dart';
import 'package:frontend/views/newPlayerWidget.dart';
import 'package:frontend/views/scrambleWidget.dart';

import 'botsWidget.dart';

class AppLayoutWidget extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(20),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.spaceEvenly,
        children: [
          Expanded(
            flex: 2,
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceEvenly,
              children: [
                Expanded(
                  flex: 5,
                  child: Image.asset("cs.jpg", fit: BoxFit.fitHeight),
                ),
                Expanded(
                  flex: 4,
                  child: CandidatesWidget(),
                ),
                Expanded(
                  flex: 4,
                  child: Column(children: [
                    NewPlayerWidget(),
                    SizedBox(
                      height: 30,
                    ),
                    BotsWidget(),
                    ScrambleWidget(),
                  ]),
                )
              ],
            ),
          ),
          SizedBox(
            height: 30,
          ),
          Expanded(
            flex: 2,
            child: StoreConnector<AppState, GameState>(
              converter: (store) => store.state.gameState,
              builder: (context, game) {
                return Row(
                  mainAxisAlignment: MainAxisAlignment.spaceAround,
                  children: [
                    Expanded(
                      flex: 1,
                      child: TeamWidget(
                        imagePath: 't.png',
                        team: game.t.players,
                        name: "Terrorists",
                        color: Colors.orange,
                      ),
                    ),
                    Expanded(
                      flex: 1,
                      child: TeamWidget(
                        imagePath: 'ct.png',
                        team: game.ct.players,
                        name: "Counter Terrorists",
                        color: Colors.blueGrey,
                      ),
                    ),
                    Expanded(
                      flex: 1,
                      child: MapPoolWidget(),
                    )
                  ],
                );
              },
            ),
          )
        ],
      ),
    );
  }
}
