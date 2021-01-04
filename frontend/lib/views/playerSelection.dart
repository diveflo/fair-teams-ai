import 'package:flutter/material.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:frontend/model/candidate.dart';
import 'package:frontend/reducer/gameConfigReducer.dart';
import 'package:frontend/state/appState.dart';
import 'package:frontend/state/gameState.dart';
import 'package:frontend/views/finalTeam.dart';
import 'package:frontend/views/mapPool.dart';
import 'package:frontend/views/newPlayerWidget.dart';

class PlayerSelection extends StatelessWidget {
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
                  child: CandidatesColumnWidget(),
                ),
                Expanded(
                  flex: 4,
                  child: NewPlayerColumnWidget(),
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
                      child: FinalTeamWidget(
                        imagePath: 't.png',
                        team: game.t.players,
                        name: "Terrorists",
                        color: Colors.orange,
                      ),
                    ),
                    Expanded(
                      flex: 1,
                      child: FinalTeamWidget(
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

class CandidatesColumnWidget extends StatefulWidget {
  @override
  _CandidatesColumnWidgetState createState() => _CandidatesColumnWidgetState();
}

class _CandidatesColumnWidgetState extends State<CandidatesColumnWidget> {
  ScrollController _scrollController;

  int _getActivePlayer(List<Candidate> players) {
    int count = 0;
    players.forEach((element) {
      if (element.isSelected) {
        count++;
      }
    });
    return count;
  }

  @override
  initState() {
    _scrollController = ScrollController();
    super.initState();
  }

  @override
  void dispose() {
    _scrollController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 60),
      child: Column(
        children: [
          Padding(
            padding: const EdgeInsets.only(bottom: 10),
            child: StoreConnector<AppState, List<Candidate>>(
                converter: (store) => store.state.gameConfigState.candidates,
                builder: (context, count) {
                  return Text(
                    "Players: ${_getActivePlayer(count)}",
                    style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
                  );
                }),
          ),
          Expanded(
            child: Container(
              child: Scrollbar(
                isAlwaysShown: true,
                controller: _scrollController,
                child: StoreConnector<AppState, List<Candidate>>(
                  converter: (store) => store.state.gameConfigState.candidates,
                  builder: (context, players) {
                    return ListView.builder(
                      controller: _scrollController,
                      itemCount: players.length,
                      itemBuilder: (BuildContext context, int index) {
                        return new CheckboxListTile(
                          title: Text(players[index].name,
                              style:
                                  Theme.of(context).primaryTextTheme.bodyText1),
                          value: players[index].isSelected,
                          onChanged: (bool value) {
                            StoreProvider.of<AppState>(context).dispatch(
                                TogglePlayerSelectionAction(players[index]));
                          },
                        );
                      },
                    );
                  },
                ),
              ),
            ),
          )
        ],
      ),
    );
  }
}

class MyButton extends StatelessWidget {
  const MyButton({
    Key key,
    @required this.onPressed,
    @required this.color,
    @required this.buttonText,
  }) : super(key: key);

  final VoidCallback onPressed;
  final Color color;
  final String buttonText;

  @override
  Widget build(BuildContext context) {
    return RaisedButton(
      child: Text(
        buttonText,
      ),
      onPressed: onPressed,
      color: color,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
    );
  }
}
