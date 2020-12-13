import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter/widgets.dart';
import 'package:frontend/apiService.dart';
import 'package:frontend/model/player.dart';

class PlayerSelection extends StatefulWidget {
  @override
  _PlayerSelectionState createState() => _PlayerSelectionState();
}

class _PlayerSelectionState extends State<PlayerSelection> {
  List<Player> players;
  List<Player> team1;
  List<Player> team2;
  bool isLoading;
  bool _isValid;

  TextEditingController _nameController;
  TextEditingController _steamIdController;

  final ScrollController _scrollController = ScrollController();

  @override
  void initState() {
    players = List<Player>();
    team1 = List<Player>();
    team2 = List<Player>();
    players.add(Player(name: "Flo", steamID: "76561197973591119"));
    players.add(Player(name: "Hubi", steamID: "76561198258023370"));
    players.add(Player(name: "Alex", steamID: "76561198011775117"));
    players.add(Player(name: "Sandy", steamID: "76561198011654217"));
    players.add(Player(name: "Markus", steamID: "76561197984050254"));
    players.add(Player(name: "Andi", steamID: "76561199045573415"));
    players.add(Player(name: "Martin", steamID: "76561197978519504"));
    players.add(Player(name: "Ferdy", steamID: "76561198031200891"));
    // players.add(Player(name: "Niggo"));
    players.add(Player(name: "Stefan", steamID: "76561198058595736"));
    players.add(Player(name: "Uwe", steamID: ""));

    isLoading = false;
    _nameController = TextEditingController();
    _steamIdController = TextEditingController();
    _steamIdController.addListener(_validateSteamID);
    _isValid = false;

    super.initState();
  }

  _validateSteamID() {
    setState(() {
      if (_steamIdController.value.text.length == 17) {
        _isValid = true;
      } else {
        _isValid = false;
      }
    });
  }

  _addPlayer() {
    if (_isValid) {
      setState(() {
        players.add(Player(
            name: _nameController.text, steamID: _steamIdController.text));
        _nameController.clear();
        _steamIdController.clear();
      });
    }
  }

  @override
  void dispose() {
    _nameController.dispose();
    _steamIdController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      mainAxisAlignment: MainAxisAlignment.spaceEvenly,
      children: [
        SizedBox(
          height: 30,
        ),
        Expanded(
          flex: 2,
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceEvenly,
            children: [
              Image.asset("cs.jpg"),
              Container(
                width: 200,
                child: Scrollbar(
                  isAlwaysShown: true,
                  controller: _scrollController,
                  child: ListView.builder(
                    controller: _scrollController,
                    itemCount: players.length,
                    itemBuilder: (BuildContext context, int index) {
                      return new CheckboxListTile(
                        title: Text(players[index].name,
                            style:
                                Theme.of(context).primaryTextTheme.bodyText1),
                        value: players[index].isSelected,
                        onChanged: (bool value) {
                          setState(() {
                            players[index].isSelected = value;
                          });
                        },
                      );
                    },
                  ),
                ),
              ),
              Container(
                width: 300,
                child: Column(
                  children: [
                    Padding(
                      padding: EdgeInsets.symmetric(vertical: 5),
                      child: TextField(
                        controller: _nameController,
                        decoration: InputDecoration(
                            border: OutlineInputBorder(), labelText: "Name"),
                      ),
                    ),
                    Padding(
                      padding: EdgeInsets.symmetric(vertical: 5),
                      child: TextField(
                        controller: _steamIdController,
                        keyboardType: TextInputType.number,
                        inputFormatters: [
                          FilteringTextInputFormatter.digitsOnly
                        ],
                        decoration: InputDecoration(
                            border: OutlineInputBorder(),
                            labelText: "SteamID",
                            errorText: _isValid ? null : "invalid steam ID"),
                      ),
                    ),
                    Padding(
                      padding: EdgeInsets.symmetric(vertical: 5),
                      child: RaisedButton(
                        child: Text("Add Player"),
                        onPressed: _addPlayer,
                        color: Colors.purple,
                        shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(20)),
                      ),
                    ),
                  ],
                ),
              )
            ],
          ),
        ),
        SizedBox(
          height: 30,
        ),
        Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Padding(
              padding: EdgeInsets.symmetric(horizontal: 10),
              child: RaisedButton(
                color: Colors.lime,
                shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(20)),
                child: Padding(
                  padding: const EdgeInsets.all(10.0),
                  child: Text(
                    "Scramble",
                    style: TextStyle(fontSize: 20),
                  ),
                ),
                onPressed: () {
                  setState(() {
                    List<Player> activePlayers =
                        players.where((element) => element.isSelected).toList();
                    if (activePlayers.length > 0) {
                      List<Player> _team1 = List<Player>();
                      List<Player> _team2 = List<Player>();
                      activePlayers.shuffle();
                      for (int i = 0; i < activePlayers.length; i++) {
                        if (i % 2 == 0) {
                          _team1.add(activePlayers[i]);
                        } else {
                          _team2.add(activePlayers[i]);
                        }
                      }
                      team1 = _team1;
                      team2 = _team2;
                    }
                  });
                },
              ),
            ),
            isLoading ? CircularProgressIndicator() : Container(),
            Padding(
              padding: EdgeInsets.symmetric(horizontal: 10),
              child: RaisedButton(
                color: Colors.lime,
                shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(20)),
                child: Padding(
                  padding: const EdgeInsets.all(10.0),
                  child: Text(
                    "ScrambleAPI",
                    style: TextStyle(fontSize: 20),
                  ),
                ),
                onPressed: () {
                  PlayerApi api = PlayerApi();

                  List<Player> activePlayers =
                      players.where((element) => element.isSelected).toList();

                  setState(() {
                    isLoading = true;
                  });

                  api.fetchScrambledTeams(activePlayers).then((game) {
                    setState(() {
                      team1 = game.t.players;
                      team2 = game.ct.players;
                      isLoading = false;
                    });
                  });
                },
              ),
            ),
          ],
        ),
        Expanded(
          flex: 2,
          child: Container(
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceAround,
              children: [
                Expanded(
                  flex: 1,
                  child: Team(
                    imagePath: 't.png',
                    team: team1,
                    name: "Terrors",
                    color: Colors.red,
                  ),
                ),
                Expanded(
                  flex: 1,
                  child: Team(
                    imagePath: 'ct.jpg',
                    team: team2,
                    name: "CTs",
                    color: Colors.blue,
                  ),
                )
              ],
            ),
          ),
        )
      ],
    );
  }
}

class Team extends StatelessWidget {
  const Team({
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
                  child: Column(
                    children: [
                      ListTile(
                        leading: Image(
                          image: AssetImage(imagePath),
                        ),
                        title: Text(
                          team[index].name,
                          style: TextStyle(fontSize: 25),
                        ),
                        subtitle: Text(
                          team[index].steamName,
                          style: TextStyle(
                              color: color, fontWeight: FontWeight.bold),
                        ),
                      ),
                      Align(
                          alignment: Alignment.bottomRight,
                          child: Text(
                            team[index].skillScore.toString(),
                            style: TextStyle(
                                fontStyle: FontStyle.italic,
                                color: Colors.purple),
                          ))
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
