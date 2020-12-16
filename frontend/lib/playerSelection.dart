import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_redux/flutter_redux.dart';
import 'package:frontend/model/player.dart';
import 'package:frontend/model/team.dart';
import 'package:frontend/reducer/gameReducer.dart';
import 'package:frontend/reducer/gameConfigReducer.dart';
import 'package:frontend/state/appState.dart';
import 'package:frontend/state/gameState.dart';
import 'package:frontend/thunks/scramble.dart';

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
                  flex: 1,
                  child: Image.asset("cs.jpg", fit: BoxFit.fitHeight),
                ),
                Expanded(
                  flex: 1,
                  child: CandidatesColumnWidget(),
                ),
                Expanded(
                  flex: 1,
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
                        imagePath: 'ct.jpg',
                        team: game.ct.players,
                        name: "Counter Terrorists",
                        color: Colors.blueGrey,
                      ),
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

class NewPlayerColumnWidget extends StatefulWidget {
  @override
  _NewPlayerColumnWidgetState createState() => _NewPlayerColumnWidgetState();
}

class _NewPlayerColumnWidgetState extends State<NewPlayerColumnWidget> {
  bool _isValid;

  TextEditingController _nameController;
  TextEditingController _steamIdController;

  @override
  void initState() {
    _nameController = TextEditingController();
    _steamIdController = TextEditingController();
    _steamIdController.addListener(_validateSteamID);
    _isValid = false;

    super.initState();
  }

  void _validateSteamID() {
    setState(() {
      if (_steamIdController.value.text.length == 17) {
        _isValid = true;
      } else {
        _isValid = false;
      }
    });
  }

  void _addPlayer() {
    if (_isValid) {
      StoreProvider.of<AppState>(context).dispatch(AddPlayerAction(Player(
          name: _nameController.text, steamID: _steamIdController.text)));
      setState(() {
        _nameController.clear();
        _steamIdController.clear();
      });
    }
  }

  void _scramblePlayers() {
    StoreProvider.of<AppState>(context).dispatch(scrambleTeamsRandom());
  }

  void _scrambleApi() {
    StoreProvider.of<AppState>(context).dispatch(scrambleTeams());
  }

  @override
  void dispose() {
    _nameController.dispose();
    _steamIdController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.start,
        children: [
          Padding(
            padding: EdgeInsets.only(bottom: 10),
            child: Text(
              "New Player",
              style: TextStyle(fontWeight: FontWeight.bold, fontSize: 20),
            ),
          ),
          Padding(
            padding: EdgeInsets.only(bottom: 10),
            child: TextField(
              controller: _nameController,
              decoration: InputDecoration(
                  border: OutlineInputBorder(), labelText: "Name"),
            ),
          ),
          TextField(
            controller: _steamIdController,
            keyboardType: TextInputType.number,
            inputFormatters: [FilteringTextInputFormatter.digitsOnly],
            decoration: InputDecoration(
                border: OutlineInputBorder(),
                labelText: "SteamID",
                errorText: _isValid ? null : "invalid steam ID"),
          ),
          Expanded(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.spaceEvenly,
              children: [
                MyButton(
                  onPressed: _addPlayer,
                  color: Colors.pink,
                  buttonText: "Add Player",
                ),
                StoreConnector<AppState, bool>(
                  converter: (store) => store.state.gameState.isLoading,
                  builder: (context, isLoading) {
                    return Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        Padding(
                          padding: EdgeInsets.symmetric(horizontal: 10),
                          child: MyButton(
                            buttonText: "Scramble",
                            onPressed: _scramblePlayers,
                            color: Colors.lime,
                          ),
                        ),
                        isLoading ? CircularProgressIndicator() : Container(),
                        Padding(
                          padding: EdgeInsets.symmetric(horizontal: 10),
                          child: MyButton(
                            buttonText: "ScrambleApi",
                            onPressed: _scrambleApi,
                            color: Colors.lime,
                          ),
                        ),
                      ],
                    );
                  },
                ),
              ],
            ),
          ),
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
  final ScrollController _scrollController = ScrollController();

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 10),
      child: Column(
        children: [
          Padding(
            padding: const EdgeInsets.only(bottom: 10),
            child: Text(
              "Players",
              style: TextStyle(fontWeight: FontWeight.bold, fontSize: 20),
            ),
          ),
          Expanded(
            child: Container(
              child: Scrollbar(
                isAlwaysShown: true,
                controller: _scrollController,
                child: StoreConnector<AppState, List<Player>>(
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
        Text(
          name,
          style: TextStyle(
              color: color, fontWeight: FontWeight.bold, fontSize: 30),
        ),
        Expanded(
          child: Container(
            child: ListView.builder(
              itemCount: team.length,
              itemBuilder: (BuildContext context, int index) {
                return Card(
                  shape: RoundedRectangleBorder(
                      side: BorderSide(color: color, width: 2),
                      borderRadius: BorderRadius.circular(8)),
                  child: Column(
                    children: [
                      ListTile(
                        leading: Image(
                          image: AssetImage(imagePath),
                        ),
                        title: Text(
                          team[index].name,
                          style: TextStyle(fontSize: 20),
                        ),
                        subtitle: Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            Text(team[index].steamName,
                                style: TextStyle(
                                    color: color, fontWeight: FontWeight.bold)),
                            Text(
                              team[index].skillScore.toString(),
                              style: TextStyle(
                                  fontStyle: FontStyle.italic,
                                  color: Colors.purple),
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                );
              },
            ),
          ),
        )
      ],
    );
  }
}
